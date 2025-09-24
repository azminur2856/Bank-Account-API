using DAL.EF;
using DAL.EF.Tables;
using DAL.Enums;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repos
{
    internal class AddressRepo : IRepo<Address, int, bool>, IAddressFeatures
    {
        BANKContext db;
        public AddressRepo()
        {
            db = new BANKContext();
        }

        public bool Create(Address obj)
        {
            db.Addresses.Add(obj);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            db.Addresses.Remove(exobj);
            return db.SaveChanges() > 0;
        }

        public Address Get(int id)
        {
            return db.Addresses.Find(id);
        }

        public List<Address> Get()
        {
            return db.Addresses.ToList();
        }

        public List<Address> GetByUserId(int userId)
        {
            return (from a in db.Addresses
                    where a.UserId == userId
                    select a).ToList();
        }

        public Address GetByUserIdAndType(int userId, AddressType type)
        {
            return (from a in db.Addresses
                   where a.UserId == userId &&
                   a.Type == type
                   select a).FirstOrDefault();
        }

        public bool Update(Address obj)
        {
            var exobj = Get(obj.AddressId);
            db.Entry(exobj).CurrentValues.SetValues(obj);
            return db.SaveChanges() > 0;
        }

    }
}
