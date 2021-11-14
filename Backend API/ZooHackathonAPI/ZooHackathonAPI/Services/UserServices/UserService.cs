﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ZooHackathonAPI.Entities;
using ZooHackathonAPI.Models.User;
using ZooHackathonAPI.Repository.UserRepo;
using ZooHackathonAPI.Requests;
using ZooHackathonAPI.Responses;
using ZooHackathonAPI.Services.BaseServices;
using ZooHackathonAPI.Services.ReportServices;
using ZooHackathonAPI.UnitOfWorks;
using ZooHackathonAPI.Utilities;
using ZooHackathonAPI.ViewModels;

namespace ZooHackathonAPI.Services.UserServices
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly AutoMapper.IConfigurationProvider _mapper;

        private readonly IReportService _reportService;

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper,
            IUserRepository repository, 
            IReportService reportService) : base(unitOfWork, repository)
        {
            _configuration = configuration;
            _mapper = mapper.ConfigurationProvider;

            _reportService = reportService;
        }

        public async Task<UserResponse> Login(string email, string password)
        {
            var user = await Get()
                .Where(tempUser => tempUser.Email.Equals(email) && tempUser.Password.Equals(password))
                .ProjectTo<UserDTO>(_mapper)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                var userReports = _reportService.GetReportsByUserId(user.Id);

                string token = TokenUtil.GenerateJWTToken(user, _configuration);

                var response = new UserResponse
                {
                    Token = token,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsHideInfo = user.IsHideInfo,
                    ReportCount = userReports != null ? userReports.Count : 0
                };

                return await Task.Run(() => response);
            }

            throw new ErrorResponse((int)HttpStatusCode.Forbidden, "Wrong email or password.");
        }

        public async Task<UserResponse> Register(string email, string password, int role, bool isHideInfo)
        {
            var existUser = await Get()
                .Where(tempUser => tempUser.Email.Equals(email))
                .FirstOrDefaultAsync();

            if (existUser != null)
                throw new ErrorResponse((int)HttpStatusCode.Forbidden, "This email existed.");

            string fullName = email.Split('@')[0];

            var user = new UserDTO
            {
                Email = email,
                Password = password,
                FullName = fullName,
                Role = role,
                IsHideInfo = isHideInfo,
                CreateDate = DateTime.Now
            };

            var targetUser = _mapper.CreateMapper().Map<User>(user);

            await CreateAsync(targetUser);

            var token = TokenUtil.GenerateJWTToken(user, _configuration);

            var response = new UserResponse
            {
                Token = token,
                Email = targetUser.Email,
                FullName = targetUser.Fullname, 
                IsHideInfo = targetUser.IsHideInfo, 
                ReportCount = 0
            };

            return await Task.Run(() => response);
        }

        public async Task<User> UpdateUser(UpdateUserRequest request)
        {
            User targetUser = _mapper.CreateMapper().Map<User>(request);

            await UpdateAsync(targetUser);

            return await Task.Run(() => targetUser);
        }
    }
}
