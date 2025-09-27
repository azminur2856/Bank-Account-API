using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class UserAddressesAccountsDTO : UserDTO
    {
        public List<AddressDTO> Addresses { get; set; }
        public List<AccountDTO> Accounts { get; set; }

        public UserAddressesAccountsDTO()
        {
            Addresses = new List<AddressDTO>();
            Accounts = new List<AccountDTO>();
        }
    }
}
