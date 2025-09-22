using DAL.EF;
using DAL.EF.Tables;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repos
{
    internal class UserRepo : IRepo<User, int, bool>, IUserFeatures
    {
        BANKContext db;
        public UserRepo()
        {
            db = new BANKContext(); 
        }
        public bool Create(User obj)
        {
            db.Users.Add(obj);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var exobj = Get(id);
            db.Users.Remove(exobj);
            return db.SaveChanges() > 0;
        }

        public User Get(int id)
        {
            return db.Users.Find(id);
        }

        public List<User> Get()
        {
            return db.Users.ToList();
        }

        public User GetByEmail(string email)
        {
            return (from u in db.Users
                   where u.Email.Equals(email)
                   select u).FirstOrDefault();
        }

        public bool Update(User obj)
        {
            var exobj = Get(obj.UserId);
            db.Entry(exobj).CurrentValues.SetValues(obj);
            return db.SaveChanges() > 0;
        }
    }
}
