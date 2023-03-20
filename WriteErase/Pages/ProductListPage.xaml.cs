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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WriteErase.Classes;
using WriteErase.Windows;

namespace WriteErase.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductListPage.xaml
    /// </summary>
    public partial class ProductListPage : Page
    {
        public ProductListPage()
        {
            InitializeComponent();
            ProductLV.ItemsSource = ClassBase.BASE.Product.ToList();
            ProductLV.SelectedValuePath = "ProductArticleNumber";
            SortBox.SelectedIndex = 0;
            DiscountFilterBox.SelectedIndex = 0;

            if (Values.user != null)
            {
                if (Values.user.UserRole == 3 || Values.user.UserRole == 2)
                {
                    addProductBtn.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                addProductBtn.Visibility = Visibility.Collapsed;
            }

            int count = ClassBase.BASE.Product.Count();
            RecordsCounterBox.Text = $"{count} из {count}";
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            string artcl = (sender as System.Windows.Controls.Image).Uid.ToString();

            Product p = ClassBase.BASE.Product.FirstOrDefault(x => x.ProductArticleNumber.Equals(artcl));
            if (!String.IsNullOrEmpty(p.ProductPhoto))
            {
                string path = Environment.CurrentDirectory.Replace("bin\\Debug", $"Resources\\{p.ProductPhoto}");
                (sender as System.Windows.Controls.Image).Source = new BitmapImage(new Uri(path));
            }
            else
            {
                string path = Environment.CurrentDirectory.Replace("bin\\Debug", $"Resources\\picture.png");
                (sender as System.Windows.Controls.Image).Source = new BitmapImage(new Uri(path));
            }
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            string article = (sender as TextBlock).Uid.ToString();
            Product p = ClassBase.BASE.Product.FirstOrDefault(x => x.ProductArticleNumber == article);
            if (!String.IsNullOrEmpty(p.ProductDiscountAmount.ToString()))
            {
                (sender as TextBlock).Text = $"Скидка {p.ProductDiscountAmount}%";
            }
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            string article = (sender as Border).Uid.ToString();
            Product p = ClassBase.BASE.Product.FirstOrDefault(x => x.ProductArticleNumber == article);
            if (p.ProductDiscountAmount > 15)
            {
                (sender as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(127, 255, 0));
            }
        }

        private void TextBlock_Loaded_1(object sender, RoutedEventArgs e)
        {
            string article = (sender as TextBlock).Uid.ToString();
            Product p = ClassBase.BASE.Product.FirstOrDefault(x => x.ProductArticleNumber == article);
            if (!String.IsNullOrEmpty(p.ProductDiscountAmount.ToString()))
            {
                (sender as TextBlock).Text = Convert.ToInt32(p.ProductCost).ToString() + " руб.";
                (sender as TextBlock).TextDecorations = TextDecorations.Strikethrough;
            }
            else
            {
                (sender as TextBlock).Text = Convert.ToInt32(p.ProductCost).ToString() + " руб.";
            }
        }

        private void TextBlock_Loaded_2(object sender, RoutedEventArgs e)
        {
            string article = (sender as TextBlock).Uid.ToString();
            Product p = ClassBase.BASE.Product.FirstOrDefault(x => x.ProductArticleNumber == article);
            if (!String.IsNullOrEmpty(p.ProductDiscountAmount.ToString()))
            {
                (sender as TextBlock).Text = Math.Floor(Convert.ToDecimal(p.ProductCost - (p.ProductCost / 100 * p.ProductDiscountAmount))).ToString() + " руб.";
            }
        }

        void Filter()
        {
            List<Product> products = ClassBase.BASE.Product.ToList();
            int countAll = products.Count;

            switch (SortBox.SelectedIndex)
            {
                case 1:
                    products = products.OrderBy(x => x.ProductCost).ToList();
                    break;
                case 2:
                    products = products.OrderByDescending(x => x.ProductCost).ToList();
                    break;
            }

            switch (DiscountFilterBox.SelectedIndex)
            {
                case 1:
                    products = products.Where(x => x.ProductDiscountAmount < 10).ToList();
                    break;
                case 2:
                    products = products.Where(x => x.ProductDiscountAmount >= 10 && x.ProductDiscountAmount < 15).ToList();
                    break;
                case 3:
                    products = products.Where(x => x.ProductDiscountAmount >= 15).ToList();
                    break;
            }

            if (!String.IsNullOrEmpty(SearchBox.Text))
            {
                products = products.Where(x => x.ProductName.ToLower().Contains(SearchBox.Text.ToLower())).ToList();
            }

            ProductLV.ItemsSource = products;
            RecordsCounterBox.Text = $"{products.Count} из {countAll}";
        }

        private void DiscountFilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void addToOrderMI_Click(object sender, RoutedEventArgs e)
        {
            Product p = ClassBase.BASE.Product.FirstOrDefault(x => x.ProductArticleNumber.Equals(ProductLV.SelectedValue.ToString()));
            ProductsForOrder pfo = new ProductsForOrder()
            {
                product = p,
                count = 1
            };

            if (Values.products.Where(x => x.product == p).Count() > 0)
            {
                MessageBox.Show("Такой продукт уже добавлен в заказ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Values.products.Add(pfo);
            }

            if (Values.products.Count > 0)
            {
                formAnOrderBtn.Visibility = Visibility.Visible;
            }
            else
            {
                formAnOrderBtn.Visibility = Visibility.Collapsed;
            }
        }

        bool isWindowOpened = false;

        private void formAnOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            FormAnOrder window = new FormAnOrder();
            if (isWindowOpened)
            {
                MessageBox.Show("Окно уже открыто", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                window.Show();
                isWindowOpened = true;
            }

            window.Closing += (obj, args) =>
            {
                isWindowOpened = false;
                if (Values.products.Count == 0)
                {
                    formAnOrderBtn.Visibility = Visibility.Collapsed;
                }
            };
        }
        private void addProductBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeProductWindow window = new ChangeProductWindow();
            if (isWindowOpened)
            {
                MessageBox.Show("Окно уже открыто", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                window.Show();
                isWindowOpened = true;
            }

            window.Closing += (obj, args) =>
            {
                isWindowOpened = false;
                ProductLV.ItemsSource = ClassBase.BASE.Product.ToList();
                ProductLV.SelectedValuePath = "ProductArticleNumber";

                SortBox.SelectedIndex = 0;
                DiscountFilterBox.SelectedIndex = 0;
                SearchBox.Text = "";
            };
        }

        private void UpdateProductBtn_Loaded(object sender, RoutedEventArgs e)
        {
            if (Values.user != null)
            {
                if (Values.user.UserRole == 3 || Values.user.UserRole == 2)
                {
                    (sender as Button).Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                (sender as Button).Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateProductBtn_Click(object sender, RoutedEventArgs e)
        {
            string id = (sender as Button).Uid.ToString();
            Product p = ClassBase.BASE.Product.FirstOrDefault(x => x.ProductArticleNumber.Equals(id));
            ChangeProductWindow window = new ChangeProductWindow(p);

            if (isWindowOpened)
            {
                MessageBox.Show("Окно уже открыто", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                window.Show();
                isWindowOpened = true;
            }

            window.Closing += (obj, args) =>
            {
                isWindowOpened = false;
                ProductLV.ItemsSource = ClassBase.BASE.Product.ToList();
                ProductLV.SelectedValuePath = "ProductArticleNumber";

                SortBox.SelectedIndex = 0;
                DiscountFilterBox.SelectedIndex = 0;
                SearchBox.Text = "";
            };
        }
    }
}
