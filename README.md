## 1.简介
>🐷 Zero.Core是一个轻量级的Asp.Net Core 业务开发框架。  
> 内置了常规的用户角色权限管理功能，开箱即用！  
> 并且会将我个人在实际业务中用到的一些比较好的案例。  
>关于这个框架其实大部分我都没有记录或者写博客，因为我写的代码中有很多的注释，基本很好明白。  
>但是我还是会将一些我个人不好理解的东西记录博客。  
>我的博客园主页:[前往](https://www.cnblogs.com/aqgy12138/)  
>当然我这个框架还是搭配了前端一起使用。  
>前端仓库:[前往](https://github.com/QQ2287991080/Zero.Core.Admin)
## 2.预览
> 施工中... 
## 3.开发  
>| **介绍** | **说明** |
>| :---- | :----|  
>|开发环境|Win10|
>|运行环境（部署）|Win10，Linux（未来一定会去做Linux的）|
>|开发工具|vs2019,SqlServer...更多请看蓝图|
>|.Net Core Sdk|[3.1.7](https://dotnet.microsoft.com/download/dotnet-core/3.1)|
>
## 4.蓝图
>对于这个框架的技术方向的整理，基本都是我未来想去学习或者已经学习了的技术，如图：  
>![image](docs/TechnologyDesign/Zero.Core.Technology.png)  
## 5.数据库设计
>数据库表设计主要为用户表、角色表、菜单表、权限表、字典数据表，同时附带的一些中间表  
>![image](docs/SqlDesign/SqlDesign.png)
>这个模型使用PowerDesign设计的，文件[地址](https://github.com/QQ2287991080/Zero.Core/tree/master/docs/SqlDesign) .pdm后缀名。    
>ps:*需要注意的是我的Pd的版本是16.6的，要不然是打不开的*
## 6.功能  
>✅用户管理  
>✅角色管理  
>✅权限管理  
>✅菜单管理  
>✅字典数据管理  
>✅EfCore+SqlServer  
>❎EfCore+MySql  
>✅Jwt用户权限  
>✅Redis+Jwt用户登录保护  
>✅Autofac依赖注入  
>❎SignalR实时推送全局错误日志  
>✅Log4net 记录程序日志  
>✅Swagger API接口文档  
>✅AutoMapper 数据映射  
>❎Quartz.Net任务调度  
## 7.交流
>欢迎各位高手，进群友好讨论学习技术！  
>QQ群：925362372
感谢您的支持，如果您喜欢请给个Star⭐吧！（在页面的右上角！🤭）
## 8.致谢

## 