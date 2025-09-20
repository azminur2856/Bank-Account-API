using AutoMapper;
using BLL.DTOs;
using DAL;
using DAL.EF.Tables;
using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService
    {
        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>().ReverseMap();
            });
            return new Mapper(config);
        }

        public static bool Create(UserDTO user)
        {
            user.Role = UserRole.Customer;
            user.CreatedAt = DateTime.Now;
            var result = true; //DataAccessFactory.UserData().Create(GetMapper().Map<User>(user));
            if(result) return true;
            return false;
        }
    }
}
