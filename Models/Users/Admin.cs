using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BidUp_App.Models.Users
{
    public class Admin : User
    {
        public override string m_role => "Admin";

        public override void displayDasboard()
        {
            var dashboard = new BidUp_App.Views.Admin.AdminDashboard(this);
            dashboard.Show();
        }
    }
}
