using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BidUp_App.Models.Interfaces
{
    public interface IUser
    {
        int m_userID { get; set; }
        string m_fullName { get; set; }
        string m_email { get; set; }
        DateTime m_BirthDate { get; set; }
        string m_role { get; }

        string m_password { get; set; }
        public string ProfilePicturePath { get; set; }
        // lista
        //
        //
        //
        //
        //
        // cari
        // card
        void displayDasboard();
    }
}
