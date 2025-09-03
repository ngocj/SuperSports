using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.UserDto
{
    public class UserUpdateDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
    }
}
