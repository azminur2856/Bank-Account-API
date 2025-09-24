using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class UserAddressDTO : UserDTO
    {
        public List<AddressDTO> Addresses { get; set; }
        public UserAddressDTO() {
            Addresses = new List<AddressDTO>();
        }
    }
}
