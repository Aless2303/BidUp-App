using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BidUp_App.Models.Loguri
{
    internal class Logs
    {
            public int LogID { get; set; }             // Cheia primară
            public DateTime Timestamp { get; set; }   // Timpul înregistrării evenimentului
            public string EventType { get; set; }     // Tipul evenimentului (ex: 'Bid', 'AuctionStart')
            public string Message { get; set; }       // Mesaj descriptiv
            public string DynamicData { get; set; }   // Date adiționale în format JSON
    }
}
