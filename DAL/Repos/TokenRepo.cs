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
    internal class TokenRepo : IRepo<Token, string, Token>, ITokenFeatures
    {
        BANKContext db;
        public TokenRepo()
        {
            db = new BANKContext();
        }

        public Token Create(Token obj)
        {
            db.Tokens.Add(obj);
            db.SaveChanges();
            return obj;
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public bool ExpireAllByUserId(int userId)
        {
            var tokens = (from t in db.Tokens
                          where t.UserId == userId
                          && t.ExpireAt == null
                          select t).ToList();
            if(!tokens.Any())
            {
                return true;
            }
            foreach (var token in tokens)
            {
                token.ExpireAt = DateTime.Now;
            }
            return db.SaveChanges() > 0;
        }

        public Token Get(string id)
        {
            var token = (from t in db.Tokens
                     where t.TokenKey.Equals(id)
                     select t).SingleOrDefault();
            return token;
        }

        public List<Token> Get()
        {
            throw new NotImplementedException();
        }

        public Token Update(Token obj)
        {
            var exobj = Get(obj.TokenKey);
            db.Entry(exobj).CurrentValues.SetValues(obj);
            db.SaveChanges();
            return obj;
        }
    }
}
