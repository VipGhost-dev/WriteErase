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

namespace WriteErase.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersListPage.xaml
    /// </summary>
    public partial class OrdersListPage : Page
    {
        public OrdersListPage()
        {
            InitializeComponent();
            refreshList();
            OrderLV.ItemsSource = Values.orders;
            SortBox.SelectedIndex = 0;
            FilterBox.SelectedIndex = 0;
        }
        
        /// <summary>
        /// Обновление листа с заказами
        /// </summary>
        void refreshList()
        {
            foreach (var item in ClassBase.BASE.Order.ToList())
            {
                OrderProductList opl = new OrderProductList();
                opl.order = item;
                double sum = 0, sumDis = 0;
                double discount = 0;
                foreach (var item2 in ClassBase.BASE.OrderProduct.Where(x => x.OrderID == item.OrderID))
                {
                    sum += (double)item2.Product.ProductCost * item2.CountProduct;
                    sumDis += (double)(item2.Product.ProductCost - (item2.Product.ProductCost / 100 * item2.Product.ProductDiscountAmount)) * item2.CountProduct;
                }
                discount = (sum - sumDis) / (sum / 100);
                opl.cost = Convert.ToInt32(sum);
                opl.discount = Convert.ToInt32(discount);
                Values.orders.Add(opl);
            }
        }

        /// <summary>
        /// Общая стоимость заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TotalCostBox_Loaded(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as TextBlock).Uid);
            List<OrderProduct> op = ClassBase.BASE.OrderProduct.Where(x => x.OrderID == id).ToList();
            int totalSum = 0;
            foreach (var item in op)
            {
                totalSum += (int)item.Product.ProductCost * item.CountProduct;
            }
            (sender as TextBlock).Text = $"Общая сумма заказа: {totalSum} руб.";
        }

        /// <summary>
        /// Общая скидка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TotalDiscountBox_Loaded(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as TextBlock).Uid);
            List<OrderProduct> op = ClassBase.BASE.OrderProduct.Where(x => x.OrderID == id).ToList();
            int totalDiscount = 0;
            foreach (var item in op)
            {
                totalDiscount += (int)(item.Product.ProductCost - (item.Product.ProductCost / 100 * item.Product.ProductDiscountAmount)) * item.CountProduct;
            }
           (sender as TextBlock).Text = $"Сумма заказа со скидкой: {totalDiscount} руб.";
        }

        /// <summary>
        /// Заполнение поля авторизированного пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserNameBox_Loaded(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as TextBlock).Uid);
            User user = ClassBase.BASE.Order.FirstOrDefault(x => x.OrderID == id).User;

            if (user != null)
                (sender as TextBlock).Text = $"Пользователь: {user.userFullName}";
            else
                (sender as TextBlock).Text = "Пользователь: Гость";
        }

        /// <summary>
        /// Заполнение листа заказа товарами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderListLV_Loaded(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as ItemsControl).Uid);
            (sender as ItemsControl).ItemsSource = ClassBase.BASE.OrderProduct.Where(x => x.OrderID == id).ToList();
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Border).Uid);
            List<OrderProduct> op = ClassBase.BASE.OrderProduct.Where(x => x.OrderID == id).ToList();

            bool isProductEnough = true;
            foreach (var item in op)
            {
                if (item.CountProduct + 3 < item.Product.ProductQuantityInStock)
                {
                    isProductEnough = true;
                }
                else
                {
                    isProductEnough = false;
                    break;
                }
            }

            if (isProductEnough)
            {
                (sender as Border).BorderBrush = new SolidColorBrush(Color.FromRgb(32, 178, 170));
            }
            else
            {
                (sender as Border).BorderBrush = new SolidColorBrush(Color.FromRgb(255, 140, 0));
            }
        }

        /// <summary>
        /// Заполнение статуса заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderStatusBox_Loaded(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as ComboBox).Uid);
            Order order = ClassBase.BASE.Order.FirstOrDefault(x => x.OrderID == id);
            (sender as ComboBox).ItemsSource = ClassBase.BASE.OrderStatus.ToList();
            (sender as ComboBox).SelectedValuePath = "StatusID";
            (sender as ComboBox).DisplayMemberPath = "StatusName";
            (sender as ComboBox).SelectedValue = order.OrderStatus;
        }

        /// <summary>
        /// Изменение статуса заказа и сохранение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderStatusBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = Convert.ToInt32((sender as ComboBox).Uid);
            Order order = ClassBase.BASE.Order.FirstOrDefault(x => x.OrderID == id);
            order.OrderStatus = Convert.ToInt32((sender as ComboBox).SelectedValue);
            ClassBase.BASE.SaveChanges();
        }

        private void deliveryDateDP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = Convert.ToInt32((sender as DatePicker).Uid);
            Order order = ClassBase.BASE.Order.FirstOrDefault(x => x.OrderID == id);

            order.OrderDeliveryDate = Convert.ToDateTime((sender as DatePicker).SelectedDate);
            ClassBase.BASE.SaveChanges();
        }

        private void deliveryDateDP_Loaded(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as DatePicker).Uid);
            Order order = ClassBase.BASE.Order.FirstOrDefault(x => x.OrderID == id);

            (sender as DatePicker).SelectedDate = order.OrderDeliveryDate;
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        void Filter()
        {
            Values.orders.Clear();
            refreshList();

            if (SortBox.SelectedIndex != 0)
            {
                switch (SortBox.SelectedIndex)
                {
                    case 1:
                        Values.orders = Values.orders.OrderBy(x => x.cost).ToList();
                        break;
                    case 2:
                        Values.orders = Values.orders.OrderByDescending(x => x.cost).ToList();
                        break;
                }
            }

            if (FilterBox.SelectedIndex != 0)
            {
                switch (FilterBox.SelectedIndex)
                {
                    case 1:
                        Values.orders = Values.orders.Where(x => x.discount <= 10).ToList();
                        break;
                    case 2:
                        Values.orders = Values.orders.Where(x => x.discount > 10 && x.discount <= 14).ToList();
                        break;
                    case 3:
                        Values.orders = Values.orders.Where(x => x.discount >= 15).ToList();
                        break;
                }
            }

            OrderLV.ItemsSource = Values.orders;
        }
    }
}
