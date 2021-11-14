﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZooHackathonAPI.Entities;
using ZooHackathonAPI.Models.Statistics;

namespace ZooHackathonAPI.Repository.UserRepo
{
    public interface IUserRepository
    {
        List<UserReportDTO> GetStatisticUserReport();
    }
}
