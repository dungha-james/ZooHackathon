﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZooHackathonAPI.Services.UserServices;

namespace ZooHackathonAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string email, string password, string fullName, int role, bool isHideInfo)
        {
            var response = await _userService.Register(email, password, fullName, role, isHideInfo);

            return await Task.Run(() => Ok(response));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var response = await _userService.Login(email, password);

            return await Task.Run(() => Ok(response));
        }
    }
}