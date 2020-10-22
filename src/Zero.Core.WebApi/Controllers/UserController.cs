﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zero.Core.Common.Result;
using Zero.Core.Common.Units;
using Zero.Core.Common.User;
using Zero.Core.Domain.Dtos.User;
using Zero.Core.Domain.Entities;
using Zero.Core.IServices;

namespace Zero.Core.WebApi.Controllers
{
    [Route("api/[controller]")]
    [SwaggerTag("用户管理")]
    [Authorize]
    public class UserController : ControllerBase
    {
        readonly IUserService _userService;
        readonly IUserProvider _userProvider;
        public UserController(
            IUserService userService,
            IUserProvider userProvider
            )
        {
            _userService = userService;
            _userProvider = userProvider;
        }


        #region Account
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("Login"),AllowAnonymous]
        public async Task<JsonResult> Login(LoginDto dto )
        {
            var user = await _userService.FirstAsync(f => dto.UserName == dto.UserName);
            if (user == null)
            {
                return AjaxHelper.Seed(System.Net.HttpStatusCode.BadRequest, "用户名错误");
            }
            if (user.Password != dto.Password)
            {
                return AjaxHelper.Seed(System.Net.HttpStatusCode.BadRequest, "密码错误");
            }
            //创建token
            var token = _userProvider.CreateJwtToken(new JwtInput() { UserName = dto.UserName });
            //保存用户信息
            await _userProvider.SetToken(dto.UserName, token.Token, token.Time);
            return AjaxHelper.Seed(System.Net.HttpStatusCode.OK, "", new { token.Token });
        }
        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns></returns>
        [HttpGet("LoginOut")]
        public async Task<JsonResult> LoginOut()
        {
            await _userProvider.Clear();
            return AjaxHelper.Seed(System.Net.HttpStatusCode.OK, "");
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUserInfo")]
        public async Task<JsonResult> GetUserInfo()
        {
            var info = await _userService.GetUserInfo();
            return AjaxHelper.Seed(Ajax.Ok, info);
        }

        #endregion

        #region User
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("GetDataList")]
        public async Task<JsonResult> GetDataList(UserCondition condition)
        {
            var data = await _userService.GetDataList(condition);
            return AjaxHelper.Seed(Ajax.Ok, data);
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<JsonResult> Add(User user)
        {
            //判断用户是否已存在
            var any = await _userService.IsUserNameExists(user.UserName);
            if (any)
                return AjaxHelper.Seed(Ajax.Bad,"该用户名已存在！");
            var entity = await _userService.AddAsync(user);
            return AjaxHelper.Seed(Ajax.Ok, entity);
        }
        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="id">修改时传id</param>
        /// <returns></returns>
        [HttpGet("IsUserNameExists")]
        public async Task<JsonResult> IsUserNameExists(string userName, int id = 0)
        {
            var any = await _userService.IsUserNameExists(userName, id);
            return AjaxHelper.Seed(Ajax.Ok, any);
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public async Task<JsonResult> Update(User user)
        {
            var info = await _userService.FirstAsync(f => f.Id == user.Id);
            if (info == null)
                return AjaxHelper.Seed(Ajax.Bad, "用户已不存在，请刷新！");
            if (await _userService.IsUserNameExists(user.UserName,user.Id))
                return AjaxHelper.Seed(Ajax.Bad, "用户名已存在，请重新输入！");
            //更新用户
            await _userService.UpdateAsync(user);
            return AjaxHelper.Seed(Ajax.Ok);
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Details")]
        public async Task<JsonResult> Details(int id)
        {
            var info = await _userService.FirstAsync(f => f.Id == id);
            if (info == null)
                return AjaxHelper.Seed(Ajax.Bad, "用户已不存在，请刷新！");
            return AjaxHelper.Seed(Ajax.Ok, info);
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Delete")]
        public async Task<JsonResult> Delete(int id)
        {
            var info = await _userService.FirstAsync(f => f.Id == id);
            if (info == null)
                return AjaxHelper.Seed(Ajax.Bad, "用户已不存在，请刷新！");
            //根据id删除实体（软删除）
            await _userService.DeleteAsync(id);
            return AjaxHelper.Seed(Ajax.Ok, info);
        }
       
        #endregion
    }
}