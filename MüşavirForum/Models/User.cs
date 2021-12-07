using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace MüşavirForum.Models
{

    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [StringLength(36, MinimumLength = 6)]
        [Required]
        public string Username { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Token { get; set; }
        public int Status { get; set; }
        public int Point { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<Content> Content { get; set; }
    }





}