using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BidUp_App.Models;

namespace BidUp_App.Views.Admin
{
    public partial class UserHistoryView : UserControl
    {
        private readonly int _userId;
        private readonly DataContextDataContext _dbContext;

        public UserHistoryView(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _dbContext = new DataContextDataContext();
            LoadUserHistory();
        }

        private void LoadUserHistory()
        {
            // Filtrăm log-urile pentru utilizatorul respectiv
            var logs = _dbContext.Logs
                .Where(log => log.DynamicData.Contains($"\"BidderID\":{_userId}") || log.DynamicData.Contains($"\"SellerID\":{_userId}"))
                .Select(log => new
                {
                    log.Timestamp,
                    log.EventType,
                    log.Message
                })
                .ToList();

            // Atribuim datele DataGrid-ului
            LogsDataGrid.ItemsSource = logs;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserID == _userId);
            if (user != null)
            {
                var userDetailsView = new UserDetailsView(user);
                var parentWindow = Application.Current.Windows.OfType<AdminDashboard>().FirstOrDefault();
                if (parentWindow != null)
                {
                    parentWindow.MainContent.Content = userDetailsView;
                }
            }
            else
            {
                MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
