using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HuniCafe.Models
{
    public class Users
    {
        // Constructor tự động khởi tạo danh sách Orders tránh lỗi Null
        public Users()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int UserID { get; set; }

        public string Username { get; set; }

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

        public string Phone { get; set; }

        public string Address { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}