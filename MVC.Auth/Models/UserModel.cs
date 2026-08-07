using System;
using System.Collections.Generic;

namespace MvcAuthDemo.Models
{
    public class UserModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        
        public int FailedLoginAttempts { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
    }

    public static class InMemoryUserStore
    {
        public static List<UserModel> Users = new List<UserModel>
        {
            new UserModel
            {
                Email = "admin@store.com",
                Password = "Password123!",
                FullName = "System Admin"
            }
        };
    }
}