using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BidUp_App.Models.Users
{
    public class AuctionViewModel
    {
        public int AuctionID { get; set; }
        public string ProductName { get; set; }
        public double StartingPrice { get; set; }
        public double CurrentPrice { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ProductImagePath { get; set; }

        public string RemainingTime { get; set; } // Timpul rămas
    }

}
