using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HuniCafe.Models
{
    public class User
    {
        // Constructor tự động khởi tạo danh sách Orders tránh lỗi Null
        public User()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Admin hoặc Customer

        public virtual ICollection<Order> Orders { get; set; }
    }
}