using BidUp_App.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BidUp_App.Models.Users
{
    public abstract class User : IUser
    {
        public int m_userID { get; set; }
        public string m_fullName { get; set; }
        public string m_email { get; set; }
        public DateTime m_BirthDate { get; set; }
        public abstract string m_role { get; }
        public string ProfilePicturePath { get; set; }
        public string m_password { get; set; }
        public User()
        {
            ProfilePicturePath = ""; // Set the path to your default image
        }

        public abstract void displayDasboard();
    }
}
