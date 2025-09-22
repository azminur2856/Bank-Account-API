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
    internal class VerificationRepo : IRepo<Verification, string, bool>
    {
        BANKContext db;

        public VerificationRepo()
        {
            db = new BANKContext();
        }

        public bool Create(Verification obj)
        {
            db.Verifications.Add(obj);
            return db.SaveChanges() > 0;
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Verification Get(string id)
        {
            var verification = (from v in db.Verifications
                                where v.Code.Equals(id) 
                                    && v.ExpireAt > DateTime.UtcNow 
                                    && !v.IsUsed
                                select v).SingleOrDefault();
            return verification;
        }

        public List<Verification> Get()
        {
            throw new NotImplementedException();
        }

        public bool Update(Verification obj)
        {
            var exobj = (from v in db.Verifications
                        where v.Code.Equals(obj.Code)
                        select v).SingleOrDefault();

            if (exobj == null) return false;
            exobj.IsUsed = obj.IsUsed;
            return db.SaveChanges() > 0;
        }
    }
}
