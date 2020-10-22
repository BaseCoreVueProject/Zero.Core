using Autofac;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Reflection;
using Zero.Core.Common.User;
using Zero.Core.WebApi.Filters;
using Zero.Core.WebApi.Middlewares;
using Zero.Core.WebApi.ServiceConfig;
using Zero.Core.WebApi.ServiceExtensions;
using Zero.Core.WebApi.StartupExtensions;
#if DEBUG
//[assembly:ApiController]
#else
[assembly:ApiController]
#endif
namespace Zero.Core.WebApi
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();
            Configuration = builder.Build();
            Logger = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(Logger, new FileInfo("log4net.config"));
        }
        public static ILoggerRepository Logger { get; set; }
        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*
            *httpcontext ����
            *https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/http-context?view=aspnetcore-3.1
            *IHttpContextAccessor
            */
            services.AddHttpContextAccessor();
            //��������ע��
            services.AddService();
            #region Framework
            //����������
            services.AddControllers()
            #region Newtonsoft Configure
            .AddNewtonsoftJson(
               option =>
               {
                   option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//����ѭ������
                   option.SerializerSettings.NullValueHandling = NullValueHandling.Include;//�Ƿ���Կ�ֵ����
                   option.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();//�շ�����
                   option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";//ʱ���ʽ��
               });
            #endregion
           
            /*
             *Ϊ��������������������֤��
             */
            services.AddMvcCore(options =>
            {
                //ȫ���쳣������
                options.Filters.Add(typeof(SysExceptionFilter));
                //������Ӧ���ݸ�ʽ
                options.Filters.Add(new ProducesAttribute("application/json"));
                //������֤�ж�
                var build = services.BuildServiceProvider();
                //��ȡʵ��
                var userProvider = build.GetService(typeof(IUserProvider)) as IUserProvider;
                options.Conventions.Add(new CustomAuthorizeFilter(userProvider));
                //options.Filters.Add(typeof(AuthorizationFilter));
                //����������֤����
                //options.Filters.Add(typeof(AuthorizeAttribute));
                //ȫ��������Ȩ��֤
                //var policy = new AuthorizationPolicyBuilder()
                //.RequireAuthenticatedUser()
                //.Build();
                //options.Filters.Add(new AuthorizeFilter());
            });
            #endregion

            #region Extension

            //ef 
            services.AddEfDbContext();
            //swagger
            services.AddSwaggerDocs();
            //jwt
            services.AddJwtToken();
            //����
            services.AddZeroCors();
            //automapper
            services.AddZeroAutoMapper();
            #endregion
        }
        /// <summary>
        /// autofac �ӹ� ioc
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModule());
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            //log4  Is't must
            var lf = loggerFactory.AddLog4Net();
            //������־
            app.UseRequestLog();

            app.UseRouting();
            //����
            app.UseCors(CorsExtension.PolicyName);
            /*
             *������Ȩ��֤
             *���� ʹ��jwt֮�����Ҫ�������ø��м��
             *������޷���֤
             */
            app.UseAuthentication();
            /*
             * ������Ȩ
             * ������ô���⣺���������ʹ��Authorize������Ȩ��֤
             * �������м��û�������Ļ����ǻ���ִ���ģ�
             * Authorizaton ���м���ܵ��е�λ����Authentication ֮���
             * ��Ҳ��Ϊʲô��Ҫע����Confiure�д����˳������
             */
            app.UseAuthorization();
            /*
             *  ��Ҫ��useRouing һ��ʹ��
             */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors(CorsExtension.PolicyName);
                //SignalR
            });
            //swagger in  middleware
            app.UseSwaggerDocs();

        }
    }
}