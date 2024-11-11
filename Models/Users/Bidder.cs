using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BidUp_App.Models.Users
{
    public class Bidder : User
    {
        public override string m_role => "Bidder";
        public Card card { get; set; }
        public override void displayDasboard()
        {
            var dashboard = new BidUp_App.Views.Bidder.BidderDashboard(this);
            dashboard.Show();
            
        }
    }
}
