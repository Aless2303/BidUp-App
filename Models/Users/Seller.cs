using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BidUp_App.Models.Users
{
    public class Seller : User
    {
        public override string m_role => "Seller";
        public Card card { get; set; }
        public override void displayDasboard()
        {
            var dashboard = new BidUp_App.Views.Seller.SellerDashboard(this);
            dashboard.Show();
        }
    }
}
