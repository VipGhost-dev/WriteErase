using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WriteErase.Classes;
using WriteErase.Pages;

namespace WriteErase.Windows
{
    /// <summary>
    /// Логика взаимодействия для HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
            ClassFrame.Mfrm = mainF;
            UserBox.Text = "Гость";
            ClassFrame.Mfrm.Navigate(new ProductListPage());
            CartBtn.Visibility = Visibility.Collapsed;
        }

        public HomeWindow(User user)
        {
            InitializeComponent();
            ClassFrame.Mfrm = mainF;
            UserBox.Text = user.UserName + " " + user.UserSurname;
            ClassFrame.Mfrm.Navigate(new ProductListPage());
            if (user.UserRole == 3)
            {
                CartBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void ProductListBtn_Click(object sender, RoutedEventArgs e)
        {
            ClassFrame.Mfrm.Navigate(new ProductListPage());
        }

        private void CartBtn_Click(object sender, RoutedEventArgs e)
        {
            ClassFrame.Mfrm.Navigate(new OrdersListPage());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Values.products.Clear();
            Values.selectedPoint = 0;
            Values.user = null;
        }
    }
}
