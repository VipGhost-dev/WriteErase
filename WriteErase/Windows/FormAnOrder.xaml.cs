using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using System.Xml.Linq;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using System.Text.RegularExpressions;
using WriteErase.Classes;

namespace WriteErase.Windows
{
    /// <summary>
    /// Логика взаимодействия для FormAnOrder.xaml
    /// </summary>
    public partial class FormAnOrder : Window
    {
        public FormAnOrder()
        {
            InitializeComponent();
            PickupPointBox.ItemsSource = ClassBase.BASE.PickupPoint.ToList();
            PickupPointBox.SelectedValuePath = "PickupPointID";
            PickupPointBox.DisplayMemberPath = "fullNameOfPoint";
            if (String.IsNullOrEmpty(Values.selectedPoint.ToString()))
            {
                PickupPointBox.SelectedIndex = 0;
            }
            else
            {
                PickupPointBox.SelectedIndex = Values.selectedPoint;
            }
            RefreshLV();
            if (Values.user != null)
            {
                UserNameBox.Text = "Пользователь: " + Values.user.UserSurname + " " + Values.user.UserName + " " + Values.user.UserPatronymic;
            }
        }

        void RefreshLV()
        {
            List<Product> p = new List<Product>();

            foreach (var item in Values.products)
            {
                p.Add(item.product);
            }

            RefreshCost();

            ProductLV.ItemsSource = p;
            ProductLV.SelectedValuePath = "ProductArticleNumber";

            if (Values.products.Count == 0)
            {
                this.Close();
            }
        }

        void RefreshCost()
        {
            try
            {
                double sumWithoutDiscount = 0, sumWithDiscount = 0, discountAmount = 0;

                foreach (var item in Values.products)
                {
                    sumWithoutDiscount += (int)item.product.ProductCost * item.count;
                    sumWithDiscount += Convert.ToDouble(Math.Floor(Convert.ToDecimal(item.product.ProductCost - (item.product.ProductCost / 100 * item.product.ProductDiscountAmount)) * item.count));
                }
                discountAmount = (sumWithoutDiscount - sumWithDiscount) / (sumWithoutDiscount / 100);

                SumWithoutDiscountBox.Text = $"Стоимость без скидки: {Convert.ToInt32(sumWithoutDiscount)} руб.";
                SumWithDiscountBox.Text = $"Стоимость со скидкой: {Convert.ToInt32(sumWithDiscount)} руб.";
                DiscountAmountBox.Text = $"Размер скидки: {Convert.ToInt32(discountAmount)}%";
            }
            catch
            {

            }
        }

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            string article = (sender as Border).Uid.ToString();
            Product p = ClassBase.BASE.Product.FirstOrDefault(x => x.ProductArticleNumber == article);
            if (p.ProductDiscountAmount > 15)
            {
                (sender as Border).Background = new SolidColorBrush(Color.FromRgb(127, 255, 0));
            }
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            string artcl = (sender as Image).Uid.ToString();

            Product p = ClassBase.BASE.Product.FirstOrDefault(x => x.ProductArticleNumber.Equals(artcl));
            if (!String.IsNullOrEmpty(p.ProductPhoto))
            {
                string path = Environment.CurrentDirectory.Replace("bin\\Debug", $"Resources\\{p.ProductPhoto}");
                (sender as Image).Source = new BitmapImage(new Uri(path));
            }
            else
            {
                string path = Environment.CurrentDirectory.Replace("bin\\Debug", $"Resources\\picture.png");
                (sender as Image).Source = new BitmapImage(new Uri(path));
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

        private void DeleteProductMI_Click(object sender, RoutedEventArgs e)
        {
            string article = ProductLV.SelectedValue.ToString();
            ProductsForOrder p = Values.products.FirstOrDefault(x => x.product.ProductArticleNumber == article);
            Values.products.Remove(p);

            RefreshLV();
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            string id = (sender as TextBox).Uid.ToString();
            ProductsForOrder p = Values.products.FirstOrDefault(x => x.product.ProductArticleNumber == id);
            (sender as TextBox).Text = p.count.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Regex.IsMatch((sender as TextBox).Text, "^[0-9]+$"))
                {
                    if (!(sender as TextBox).Text.Equals("0"))
                    {
                        string article = (sender as TextBox).Uid.ToString();
                        Values.products.FirstOrDefault(x => x.product.ProductArticleNumber == article).count = Convert.ToInt32((sender as TextBox).Text);
                        RefreshCost();
                    }
                    else
                    {
                        string article = (sender as TextBox).Uid.ToString();
                        ProductsForOrder p = Values.products.FirstOrDefault(x => x.product.ProductArticleNumber == article);
                        Values.products.Remove(p);

                        RefreshLV();
                    }
                }
                else
                {
                    (sender as TextBox).Text = "1";
                }
            }
            catch
            {

            }
        }

        private void FormAnOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            int orderId = ClassBase.BASE.Order.Max(x => x.OrderID) + 1;
            Random rnd = new Random();

            Order order = new Order();
            order.OrderID = orderId;
            order.OrderStatus = 1;
            order.OrderPickupPoint = Convert.ToInt32(PickupPointBox.SelectedValue);
            if (Values.user == null)
            {
                order.OrderCustomer = null;
            }
            else
            {
                order.OrderCustomer = Values.user.UserID;
            }
            order.ReceiptCode = rnd.Next(100, 1000);
            order.OrderDate = DateTime.Today;

            bool isProductEnough = true;
            foreach (var item in Values.products)
            {
                if (item.count + 3 < item.product.ProductQuantityInStock)
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
                order.OrderDeliveryDate = DateTime.Today.AddDays(3);
            }
            else
            {
                order.OrderDeliveryDate = DateTime.Today.AddDays(6);
            }

            ClassBase.BASE.Order.Add(order);

            foreach (var item in Values.products)
            {
                OrderProduct op = new OrderProduct()
                {
                    OrderID = order.OrderID,
                    ProductArticleNumber = item.product.ProductArticleNumber,
                    CountProduct = item.count
                };
                ClassBase.BASE.OrderProduct.Add(op);
            }
            ClassBase.BASE.SaveChanges();
            Values.products.Clear();
            Values.selectedPoint = 0;
            MessageBox.Show("Заказ успешно оформлен!\nВам доступен талон для получения заказа.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            formingPdfTicket(order.OrderID);
            this.Close();
        }

        void formingPdfTicket(int orderID)
        {
            Order order = ClassBase.BASE.Order.FirstOrDefault(x => x.OrderID == orderID);
            List<OrderProduct> op = ClassBase.BASE.OrderProduct.Where(x => x.OrderID == order.OrderID).ToList();

            SaveFileDialog sfd = new SaveFileDialog();
            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "Талон для получения заказа";
            PdfPage page = pdf.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            XFont font = new XFont("Comic sans ms", 20);
            XFont fontCode = new XFont("Comic sans ms", 20, XFontStyle.Bold, options);

            gfx.DrawString("Дата заказа: " + order.OrderDate.ToString("dd MM yyyy"), font, XBrushes.Black, new XRect(0, -355, page.Width, page.Height), XStringFormat.Center);
            gfx.DrawString("Номер заказа: " + orderID.ToString(), font, XBrushes.Black, new XRect(0, -320, page.Width, page.Height), XStringFormat.Center);
            gfx.DrawString("Состав заказа:", font, XBrushes.Black, new XRect(0, -300, page.Width, page.Height), XStringFormat.Center);
            int height = -280;
            foreach (var item in ClassBase.BASE.OrderProduct.Where(x => x.OrderID == order.OrderID))
            {
                gfx.DrawString($"{item.Product.ProductArticleNumber}: {item.Product.ProductName} ({item.CountProduct} {item.Product.Unit.UnitName})", font, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
                height += 25;
            }

            double sum = 0, sumDis = 0;
            foreach (var item2 in ClassBase.BASE.OrderProduct.Where(x => x.OrderID == order.OrderID))
            {
                sum += (double)item2.Product.ProductCost * item2.CountProduct;
                sumDis += (double)(item2.Product.ProductCost - (item2.Product.ProductCost / 100 * item2.Product.ProductDiscountAmount)) * item2.CountProduct;
            }
            height += 25;
            gfx.DrawString("Сумма заказа: " + sum.ToString(), font, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
            height += 25;
            gfx.DrawString("Сумма со скидкой: " + sumDis.ToString(), font, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
            height += 25;
            gfx.DrawString("Пункт выдачи: " + order.PickupPoint.fullNameOfPoint.ToString(), font, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
            height += 25;
            gfx.DrawString("Дата получения заказа: " + order.OrderDeliveryDate.ToString("dd MM yyyy"), font, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
            height += 25;
            if (order.User != null)
            {
                gfx.DrawString("Пользователь: " + order.User.userFullName.ToString(), font, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
                height += 25;
            }
            else
            {
                gfx.DrawString("Пользователь: Гость", font, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
                height += 25;
            }
            gfx.DrawString("Ваш код для получения заказа " + order.ReceiptCode.ToString(), fontCode, XBrushes.Black, new XRect(0, height, page.Width, page.Height), XStringFormat.Center);
            sfd.FileName = $"Талон для получения заказа №{order.OrderID}.pdf";
            pdf.Save(sfd.FileName);
            Process.Start(sfd.FileName);
        }

        private void PickupPointBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Values.selectedPoint = PickupPointBox.SelectedIndex;
        }
    }
}
