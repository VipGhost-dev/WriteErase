using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WriteErase.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangeProductWindow.xaml
    /// </summary>
    public partial class ChangeProductWindow : Window
    {
        public ChangeProductWindow()
        {
            InitializeComponent();
            uploadFileds();
            ActionBtn.Content = "Добавить";
        }

        Product product;
        bool isEditing = false;

        void uploadFileds()
        {
            ProductCategoryBox.ItemsSource = ClassBase.BASE.ProductCategory.ToList();
            ProductCategoryBox.SelectedValuePath = "CategoryID";
            ProductCategoryBox.DisplayMemberPath = "CategoryName";

            ProductManufacturerBox.ItemsSource = ClassBase.BASE.Manufacturer.ToList();
            ProductManufacturerBox.SelectedValuePath = "ManufacturerID";
            ProductManufacturerBox.DisplayMemberPath = "ManufacturerName";

            ProductProviderBox.ItemsSource = ClassBase.BASE.Provider.ToList();
            ProductProviderBox.SelectedValuePath = "ProviderID";
            ProductProviderBox.DisplayMemberPath = "ProviderName";

            ProductUnitBox.ItemsSource = ClassBase.BASE.Unit.ToList();
            ProductUnitBox.SelectedValuePath = "UnitID";
            ProductUnitBox.DisplayMemberPath = "UnitName";

            ProductCategoryBox.SelectedIndex = 0;
            ProductManufacturerBox.SelectedIndex = 0;
            ProductProviderBox.SelectedIndex = 0;
            ProductUnitBox.SelectedIndex = 0;
        }

        string file = "";

        public ChangeProductWindow(Product product)
        {
            InitializeComponent();
            uploadFileds();
            this.product = product;
            isEditing = true;

            DeleteBtn.Visibility = Visibility.Visible;
            ActionBtn.Content = "Изменить";

            ProductNameBox.Text = product.ProductName;
            ProductCategoryBox.SelectedValue = product.ProductCategory;
            ProductManufacturerBox.SelectedValue = product.ProductManufacturer;
            ProductProviderBox.SelectedValue = product.ProductProvider;
            ProductCostBox.Text = product.ProductCost.ToString();
            ProductDiscountAmountBox.Text = product.ProductDiscountAmount.ToString();
            ProductQuantityInStockBox.Text = product.ProductQuantityInStock.ToString();
            ProductUnitBox.SelectedValue = product.ProductUnit;
            ProductDescriptionBox.Text = product.ProductDescription;

            file = product.ProductPhoto;


            if (!String.IsNullOrEmpty(product.ProductPhoto))
            {
                string path = Environment.CurrentDirectory.Replace("bin\\Debug", $"Resources\\{file}");
                productImage.Source = new BitmapImage(new Uri(path));
            }
            else
            {
                string path = Environment.CurrentDirectory.Replace("bin\\Debug", $"Resources\\picture.png");
                productImage.Source = new BitmapImage(new Uri(path));
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Данный продукт удалится из всех существующих заказов.\nВы уверены что хотите удалить продукт?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                List<Order> orders = new List<Order>();
                foreach (var item in ClassBase.BASE.OrderProduct.Where(x => x.Product.ProductArticleNumber == product.ProductArticleNumber))
                {
                    orders.Add(item.Order);
                    ClassBase.BASE.OrderProduct.Remove(item);
                }
                ClassBase.BASE.SaveChanges();

                foreach (var item in orders)
                {
                    int count = 0;
                    foreach (var item2 in ClassBase.BASE.OrderProduct.Where(x => x.OrderID == item.OrderID))
                    {
                        count++;
                    }
                    if (count == 0)
                    {
                        ClassBase.BASE.Order.Remove(item);
                    }
                }

                ClassBase.BASE.Product.Remove(product);
                ClassBase.BASE.SaveChanges();
                MessageBox.Show("Продукт успешно удален!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }
        private void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(ProductNameBox.Text) && !String.IsNullOrEmpty(ProductCostBox.Text) && !String.IsNullOrEmpty(ProductDiscountAmountBox.Text) && !String.IsNullOrEmpty(ProductQuantityInStockBox.Text))
            {
                if (Regex.IsMatch(ProductCostBox.Text, @"^[0-9 ]+$") && Regex.IsMatch(ProductDiscountAmountBox.Text, @"^[0-9 ]+$") && Regex.IsMatch(ProductQuantityInStockBox.Text, @"^[0-9 ]+$"))
                {
                    if (Convert.ToInt32(ProductDiscountAmountBox.Text) <= 100)
                    {
                        if (isEditing)
                        {
                            product.ProductName = ProductNameBox.Text;
                            product.ProductCategory = Convert.ToInt32(ProductCategoryBox.SelectedValue);
                            product.ProductManufacturer = Convert.ToInt32(ProductManufacturerBox.SelectedValue);
                            product.ProductProvider = Convert.ToInt32(ProductProviderBox.SelectedValue);
                            product.ProductCost = Convert.ToDecimal(ProductCostBox.Text);
                            product.ProductDiscountAmount = Convert.ToByte(ProductDiscountAmountBox.Text);
                            product.ProductQuantityInStock = Convert.ToInt32(ProductQuantityInStockBox.Text);
                            product.ProductUnit = Convert.ToInt32(ProductUnitBox.SelectedValue);
                            product.ProductDescription = ProductDescriptionBox.Text;
                            product.ProductPhoto = file;

                            ClassBase.BASE.SaveChanges();
                            MessageBox.Show("Изменения успешно внесены", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                        else
                        {
                            Product p = new Product();
                            Random rnd = new Random();

                            string article = "";
                            article += (char)rnd.Next('A', 'Z');
                            article += rnd.Next(100, 1000);
                            article += (char)rnd.Next('A', 'Z');
                            article += rnd.Next(1, 10);

                            p.ProductArticleNumber = article;
                            p.ProductName = ProductNameBox.Text;
                            p.ProductDescription = ProductDescriptionBox.Text;
                            p.ProductCategory = Convert.ToInt32(ProductCategoryBox.SelectedValue);
                            p.ProductPhoto = file;
                            p.ProductManufacturer = Convert.ToInt32(ProductManufacturerBox.SelectedValue);
                            p.ProductProvider = Convert.ToInt32(ProductProviderBox.SelectedValue);
                            p.ProductCost = Convert.ToDecimal(ProductCostBox.Text);
                            p.ProductDiscountAmount = Convert.ToByte(ProductDiscountAmountBox.Text);
                            p.ProductQuantityInStock = Convert.ToInt32(ProductQuantityInStockBox.Text);
                            p.ProductStatus = "";
                            p.ProductUnit = Convert.ToInt32(ProductUnitBox.SelectedValue);

                            ClassBase.BASE.Product.Add(p);
                            ClassBase.BASE.SaveChanges();
                            MessageBox.Show("Продукт успешно добавлен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Скидка не может быть больше 100", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Проверьте правильность данных в полях \"Цена\", \"Скидка\", \"Количество на складе\"", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполнены не все необходимые поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteImageBtn_Click(object sender, RoutedEventArgs e)
        {
            file = "";

            string path = Environment.CurrentDirectory.Replace("bin\\Debug", $"Resources\\picture.png");
            productImage.Source = new BitmapImage(new Uri(path));
        }

        private void AddImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.ShowDialog();
                file = ofd.FileName.Substring(ofd.FileName.LastIndexOf('\\') + 1);

                string path = Environment.CurrentDirectory.Replace("bin\\Debug", $"Resources");
                if (!File.Exists(path += $"\\{file}"))
                {
                    File.Copy(ofd.FileName, path);
                }
                productImage.Source = new BitmapImage(new Uri(path));
            }
            catch
            {

            }
        }
    }
}
