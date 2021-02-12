namespace AuthorizationServer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    public class AuthUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [MinLength(6)]
        [Required]
        public string Password { get; set; }
        [RegularExpression("^[a-z]([a-zA-z0-9_.]){5,18}$")]
        [Required]
        [MaxLength(18)]
        [MinLength(5)]
        public string UserName { get; set; }
    }
}
