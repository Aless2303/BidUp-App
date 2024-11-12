using System;
using System.Linq;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace BidUp_App.Views.Bidder
{
    public partial class LastBids : UserControl
    {
        private readonly int _bidderId;
        private readonly DataContextDataContext _dbContext;

        public LastBids(int bidderId)
        {
            InitializeComponent();
            _bidderId = bidderId;
            _dbContext = new DataContextDataContext();
            LoadLastBids();
        }

        private void LoadLastBids()
        {
            try
            {
                // Obține toate log-urile cu EventType "Bid" și BidderID corespunzător
                var bids = _dbContext.Logs
                    .Where(log => log.EventType == "Bid" && log.DynamicData != null)
                    .AsEnumerable() // Adu datele în memorie pentru a permite operații dinamice
                    .Where(log =>
                    {
                        var dynamicData = JsonConvert.DeserializeObject<dynamic>(log.DynamicData);
                        return dynamicData.BidderID == _bidderId;
                    })
                    .Select(log =>
                    {
                        var dynamicData = JsonConvert.DeserializeObject<dynamic>(log.DynamicData);
                        int productId = (int)dynamicData.ProductID;
                        string productName = GetProductNameById(productId);
                        return new
                        {
                            ProductName = productName,
                            BidAmount = (decimal)dynamicData.BidAmount,
                            Timestamp = log.Timestamp
                        };
                    })
                    .ToList();

                // Atribuie datele DataGrid-ului
                BidsDataGrid.ItemsSource = bids;
            }
            catch (Exception ex)
            {
                // Gestionare eroare
                Console.WriteLine($"An error occurred while loading last bids: {ex.Message}");
            }
        }

        private string GetProductNameById(int productId)
        {
            // Găsește produsul după ProductID și returnează numele acestuia
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductID == productId);
            return product?.ProductName ?? "Unknown Product";
        }
    }
}
