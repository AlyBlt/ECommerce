using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Role
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int UserCount { get; set; } // Bu rolde kaç kullanıcı var? (Admin için faydalı bilgi)
    }
}
