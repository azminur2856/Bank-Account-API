using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRepo<CLASS,PK,RET>
    {
        RET Create(CLASS obj);
        RET Update(CLASS obj);
        bool Delete(PK id);
        CLASS Get(PK id);
        List<CLASS> Get();
    }
}
