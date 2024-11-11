using System.Linq;
using System.Windows;

namespace BidUp_App.Views.Seller
{
    public partial class ViewAuctionsWindow : Window
    {
        private readonly int _sellerId;
        private readonly DataContextDataContext _dbContext;

        public ViewAuctionsWindow(int sellerId)
        {
            InitializeComponent();
            _sellerId = sellerId;
            _dbContext = new DataContextDataContext();

            LoadAuctions();
        }

        private void LoadAuctions()
        {
            // Obține licitațiile asociate Seller-ului curent
            var auctions = _dbContext.Auctions
                .Where(a => a.SellerID == _sellerId)
                .Select(a => new
                {
                    a.AuctionID,
                    a.Product.ProductName,
                    a.CurrentPrice,
                    a.StartTime,
                    a.EndTime
                }).ToList();

            // Populează DataGrid-ul
            AuctionsDataGrid.ItemsSource = auctions;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Găsim utilizatorul curent din baza de date folosind ID-ul seller-ului
            var linqUser = _dbContext.Users.FirstOrDefault(u => u.UserID == _sellerId);

            if (linqUser != null)
            {
                // Convertim obiectul LINQ User într-un obiect de tip Seller
                var sellerUser = new BidUp_App.Models.Users.Seller
                {
                    m_userID = linqUser.UserID,
                    m_fullName = linqUser.FullName,
                    m_email = linqUser.Email,
                    m_BirthDate = linqUser.BirthDate,
                    m_password = linqUser.PasswordHash,
                    ProfilePicturePath = linqUser.ProfilePicturePath
                };

                // Navigăm înapoi la `SellerDashboard` cu obiectul `Seller`
                var sellerDashboard = new SellerDashboard(sellerUser);
                this.Close(); // Închide fereastra curentă
                sellerDashboard.Show();
            }
            else
            {
                MessageBox.Show("Could not retrieve seller information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
