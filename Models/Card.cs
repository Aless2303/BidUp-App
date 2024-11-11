using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BidUp_App.Models
{
    public class Card
    {
        public int CardID { get; set; }               // Primary key for the card
        public string CardNumber { get; set; }        // Card number (typically masked for security)
        public string CardHolderName { get; set; }    // Name of the cardholder
        public DateTime ExpiryDate { get; set; }      // Expiry date of the card
        public string CVV { get; set; }       // CVV (for demonstration; usually not stored in production)
        public float Balance { get; set; }
        public int OwnerUserID { get; set; }
    }
}
