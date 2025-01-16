using System;

namespace BlogAdminPanel.Models
{
    public class LoginHistory
    {
        public int Id { get; set; } // Primary Key
        public string UserEmail { get; set; } // User's email for login attempt
        public DateTime LoginDate { get; set; } // Timestamp of the login attempt
        public string IPAddress { get; set; } // IP address of the user
        public string UserAgent { get; set; } // User's browser info (User-Agent)
    }
}
