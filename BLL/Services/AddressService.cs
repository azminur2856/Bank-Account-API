using AutoMapper;
using BLL.DTOs;
using BLL.Utility;
using DAL;
using DAL.EF.Tables;
using DAL.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AddressService
    {
        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Address, AddressDTO>().ReverseMap();
            });
            return new Mapper(config);
        }

        public static bool AddAddress(string tokenKey, AddressAddDTO address)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return false;

            var (isValid, parsedEnum, validNames) = Helper.ParseEnum<AddressType>(address.Type);

            if (!isValid)
            {
                throw new ArgumentException(
                    $"Invalid address type '{address.Type}'. Allowed values: {string.Join(", ", validNames)}"
                );
            }

            var addresRepo = DataAccessFactory.AddressFeaturesData();

            var existingAddressOfType = addresRepo.GetByUserIdAndType(token.UserId, parsedEnum);

            if (existingAddressOfType != null)
            {
                throw new InvalidOperationException($"A {address.Type} address already exists for this user.");
            }

            var userAddresses = addresRepo.GetByUserId(token.UserId);

            if (userAddresses.Count >= 2)
            {
                throw new InvalidOperationException("A user can only have a maximum of two addresses.");
            }

            var newAddress = new Address
            {
                UserId = token.UserId,
                StreetAddress = address.StreetAddress,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                Type = parsedEnum,
                IsVerified = false,
                CreatedAt = DateTime.Now
            };
            
            var isAdded = DataAccessFactory.AddressData().Create(GetMapper().Map<Address>(newAddress));

            if (isAdded)
            {
                var auditLogType = parsedEnum == AddressType.Present
                    ? AuditLogType.PresentAddressAdded
                    : AuditLogType.PermanentAddressAdded;

                var auditLog = new AuditLogDTO()
                {
                    UserId = token.UserId,
                    Type = auditLogType,
                    Details = $"Address of type {address.Type} added for user {token.UserId}.",
                };
                AuditLogService.LogActivity(auditLog);
            }
            return isAdded;
        }

        public static bool UpdatePresentAddress(string tokenKey, AddressDTO address)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return false;

            var addresRepo = DataAccessFactory.AddressFeaturesData();

            var existingAddress = addresRepo.GetByUserIdAndType(token.UserId, AddressType.Present);

            if (existingAddress != null &&
                existingAddress.StreetAddress == address.StreetAddress &&
                existingAddress.City == address.City &&
                existingAddress.State == address.State &&
                existingAddress.PostalCode == address.PostalCode &&
                existingAddress.Country == address.Country)
            {
                throw new InvalidOperationException("No changes detected. The present address is already up to date.");
            }

            if (existingAddress == null)
            {
                throw new InvalidOperationException("No present address found to update.");
            }

            address.AddressId = existingAddress.AddressId;
            address.UserId = token.UserId;
            address.Type = AddressType.Present;
            address.IsVerified = false;
            address.CreatedAt = existingAddress.CreatedAt;
            address.UpdatedAt = DateTime.Now;

            var IsUpdated = DataAccessFactory.AddressData().Update(GetMapper().Map<Address>(address));

            if (IsUpdated)
            {
                var auditLog = new AuditLogDTO()
                {
                    UserId = token.UserId,
                    Type = AuditLogType.PresentAddressUpdated,
                    Details = $"Present address updated for user {token.UserId}.",
                };
                AuditLogService.LogActivity(auditLog);
            }
            return IsUpdated;
        }

        public static List<AddressDTO> GetUserAddresses(string tokenKey)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return null;

            var addresses = DataAccessFactory.AddressFeaturesData().GetByUserId(token.UserId);
            return GetMapper().Map<List<AddressDTO>>(addresses);
        }

        public static bool VerifyAddress(string tokenKey, int addressId)
        {
            var token = DataAccessFactory.TokenData().Get(tokenKey);
            if (token == null || token.ExpireAt != null) return false;

            var address = DataAccessFactory.AddressData().Get(addressId);
            
            if (address == null)
            {
                throw new KeyNotFoundException("Address not found.");
            }

            if (address.IsVerified) return true;

            address.IsVerified = true;
            address.UpdatedAt = DateTime.Now;

            var isVerifiedA = DataAccessFactory.AddressData().Update(address);

            if (isVerifiedA)
            {
                var auditLog = new AuditLogDTO()
                {
                    UserId = token.UserId,
                    Type = AuditLogType.AddressVerified,
                    Details = $"Address with ID {addressId} verified by {token.User.FullName} (ID: {token.UserId}).",
                };
                AuditLogService.LogActivity(auditLog);
            }
            return isVerifiedA;
        }
    }
}
