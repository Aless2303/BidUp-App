using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BidUp_App.Models.Users
{
    public static class UserFactory
    {
        public static User CreateUser(string Role)
        {
            return Role switch
            {
                "Bidder" => new Bidder(),
                "Seller" => new Seller(),
                "Admin" => new Admin(),
                _ => throw new ArgumentException("Invalid role specified!")
            };
        }
    }
}
