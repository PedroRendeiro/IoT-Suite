using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IoTSuite.Shared
{
    public class BasicAuthUser
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }

        public Guid? Salt { get; set; }
    }

    public class BasicAuthUserDTO
    {

        [Required]
        public string Username { get; set; }
        
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
