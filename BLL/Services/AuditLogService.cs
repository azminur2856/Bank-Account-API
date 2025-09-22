using AutoMapper;
using BLL.DTOs;
using BLL.Utility;
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
    public class AuditLogService
    {
        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AuditLog, AuditLogDTO>().ReverseMap();
            });
            return new Mapper(config);
        }

        public static bool LogActivity(AuditLogDTO auditLogDTO)
        {
            var auditLog = GetMapper().Map<AuditLog>(auditLogDTO);
            auditLog.Timestamp = DateTime.Now;
            return DataAccessFactory.AuditLogData().Create(auditLog);
        }

        public static List<AuditLogDTO> Get()
        {
            return GetMapper().Map<List<AuditLogDTO>>(DataAccessFactory.AuditLogData().Get());
        }

        public static (bool isValid, List<AuditLogDTO> logs, List<string> valueTypes) GetByType(string type)
        { 
            var parsResult = Helper.ParseEnum<AuditLogType>(type);
            
            if (!parsResult.isValid)
            {
                return (false, null, parsResult.validNames);
            }
            
            var logs = GetMapper().Map<List<AuditLogDTO>>(DataAccessFactory.AuditLogFeaturesData().GetByType(parsResult.parsedEnum));
            
            return (true, logs, null);
        }

        public static List<AuditLogDTO> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            var logs = DataAccessFactory.AuditLogFeaturesData().GetByDateRange(startDate, endDate);
            return GetMapper().Map<List<AuditLogDTO>>(logs);
        }

        public static List<AuditLogDTO> GetByUserEmail(string email)
        {
            var logs = DataAccessFactory.AuditLogFeaturesData().GetByUserEmail(email);
            return GetMapper().Map<List<AuditLogDTO>>(logs);
        }
    }
}
