using BidUp_App;
using System;
using System.Linq;


namespace BidUp_App.Models
{

    public class Wallet
    {
        public int WalletID { get; set; }
        public int UserID { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }


        public decimal GetWalletBalance(int userId)
        {
            using (var context = new DataContextDataContext())
            {
                var wallet = context.Wallets.FirstOrDefault(w => w.UserID == userId);
                return wallet?.Balance ?? 0.00m;
            }
        }


        public void AddFundsToWallet(int userId, decimal amount)
        {
            using (var context = new DataContextDataContext())
            {
                var wallet = context.Wallets.FirstOrDefault(w => w.UserID == userId);

                if (wallet != null)
                {
                    wallet.Balance += amount;
                    wallet.LastUpdated = DateTime.Now;
                }
                else
                {
                    // Creează un portofel nou dacă utilizatorul nu are unul
                    wallet = new BidUp_App.Wallet
                    {
                        UserID = userId,
                        Balance = amount,
                        LastUpdated = DateTime.Now
                    };
                    context.Wallets.InsertOnSubmit(wallet);
                }

                context.SubmitChanges();
            }
        }


        public bool TakeFundsFromWallet(int userId, decimal amount)
        {
            using (var context = new DataContextDataContext())
            {
                var wallet = context.Wallets.FirstOrDefault(w => w.UserID == userId);

                if (wallet != null && wallet.Balance >= amount)
                {
                    wallet.Balance -= amount;
                    wallet.LastUpdated = DateTime.Now;
                    context.SubmitChanges();
                    return true; // Tranzacția a reușit
                }

                return false; // Fonduri insuficiente
            }
        }

    }

}
