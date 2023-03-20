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
using System.Windows.Threading;
using WriteErase.Classes;
using WriteErase.Windows;

namespace WriteErase
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int time = 10;
        private DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            ClassBase.BASE = new Entities();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += Timer_Tick;
            capchaRow.Height = new GridLength(0);
            this.Height = 300;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (time != 0)
            {
                time--;
                timer.Text = $"Осталось {time} секунд";
            }
            else
            {
                dispatcherTimer.Stop();
                timer.Text = "";
                buttonsGrid.IsEnabled = true;
            }
        }

        int attemptCount = 0;
        string str = "";

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(LoginBox.Text) && !String.IsNullOrEmpty(PasswordB.Password))
            {
                List<User> users = ClassBase.BASE.User.ToList();
                if (users.Where(x => x.UserLogin.Equals(LoginBox.Text.Trim()) && x.UserPassword.Equals(PasswordB.Password.Trim())).Count() == 1)
                {
                    if (attemptCount > 0)
                    {
                        if (CapchaBox.Text.Equals(str))
                        {
                            User user = ClassBase.BASE.User.FirstOrDefault(x => x.UserLogin.Equals(LoginBox.Text) && x.UserPassword.Equals(PasswordB.Password));
                            Values.user = user;
                            HomeWindow window = new HomeWindow(user);
                            window.Show();
                            this.Hide();

                            window.Closing += (obj, args) =>
                            {
                                this.Show();
                                LoginBox.Text = "";
                                PasswordB.Password = "";
                                CapchaBox.Text = "";
                                capchaEnteringG.Visibility = Visibility.Collapsed;
                                attemptCount = 0;
                                capchaRow.Height = new GridLength(0);
                                this.Height = 300;
                            };
                        }
                        else
                        {
                            MessageBox.Show("Ваш текст не совпадает с текстом из картинки\nПовторите попытку!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                            if (attemptCount >= 2)
                            {
                                timer.Text = $"Осталось 10 секунд!";
                                time = 10;
                                dispatcherTimer.Start();

                                buttonsGrid.IsEnabled = false;
                            }
                            updateCapcha();
                        }
                    }
                    else
                    {
                        User user = ClassBase.BASE.User.FirstOrDefault(x => x.UserLogin.Equals(LoginBox.Text) && x.UserPassword.Equals(PasswordB.Password));
                        Values.user = user;
                        HomeWindow window = new HomeWindow(user);
                        window.Show();
                        this.Hide();

                        window.Closing += (obj, args) =>
                        {
                            this.Show();
                            LoginBox.Text = "";
                            PasswordB.Password = "";
                            CapchaBox.Text = "";
                            capchaEnteringG.Visibility = Visibility.Collapsed;
                            attemptCount = 0;
                            capchaRow.Height = new GridLength(0);
                            this.Height = 300;
                        };
                    }
                }
                else
                {
                    MessageBox.Show("Проверьте правильность введенных данных", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    attemptCount++;
                    if (attemptCount == 1)
                    {
                        int procent = (int)(this.Height / 100 * 41) + 60;
                        capchaRow.Height = new GridLength(procent);
                        this.Height = 450;
                        capchaEnteringG.Visibility = Visibility.Visible;
                        updateCapcha();
                        CapchaBox.Text = "";
                    }
                    if (attemptCount >= 2)
                    {
                        timer.Text = $"Осталось 10 секунд";
                        time = 10;
                        updateCapcha();
                        dispatcherTimer.Start();

                        buttonsGrid.IsEnabled = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("Поля логина и/или пароля пустые", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void updateCapcha()
        {
            canvas.Children.Clear();
            str = "";
            Random rnd = new Random();
            int linesCount = rnd.Next(40, 70);
            int textLength = 4;

            for (int i = 0; i < linesCount; i++)
            {
                Brush color = new SolidColorBrush(Color.FromRgb(Convert.ToByte(rnd.Next(1, 255)), Convert.ToByte(rnd.Next(1, 255)), Convert.ToByte(rnd.Next(1, 233))));
                Line line = new Line()
                {
                    X1 = rnd.Next(Convert.ToInt32(canvas.Width)),
                    Y1 = rnd.Next(Convert.ToInt32(canvas.Height)),
                    X2 = rnd.Next(Convert.ToInt32(canvas.Width)),
                    Y2 = rnd.Next(Convert.ToInt32(canvas.Height)),
                    Stroke = color,
                    Fill = color
                };
                canvas.Children.Add(line);
            }

            for (int i = 0; i < textLength; i++)
            {
                int symbol = rnd.Next(2);
                switch (symbol)
                {
                    case 0:
                        int size = rnd.Next(2);
                        switch (size)
                        {
                            case 0:
                                str += Convert.ToChar(rnd.Next('a', 'z' + 1));
                                break;
                            case 1:
                                str += Convert.ToChar(rnd.Next('A', 'Z' + 1));
                                break;
                        }
                        break;
                    case 1:
                        str += rnd.Next(0, 10).ToString();
                        break;
                }
            }

            int startSegment = 0, endSegment = 0;
            int step = Convert.ToInt32(canvas.Width / str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                if (i == 0)
                {
                    endSegment += step;
                }
                else
                {
                    startSegment = endSegment;
                    endSegment += step;
                }
                int widthSegment = rnd.Next(startSegment, endSegment), heightSegment = rnd.Next(0, Convert.ToInt32(canvas.Height));
                if (widthSegment >= canvas.Width - 30) widthSegment -= 30;
                if (heightSegment >= canvas.Height - 30) heightSegment -= 30;
                int font = rnd.Next(3);
                TextBlock tb = null;
                switch (font)
                {
                    case 0:
                        tb = new TextBlock()
                        {
                            Text = str[i].ToString(),
                            FontSize = 16,
                            FontWeight = FontWeights.Bold,
                            Padding = new Thickness(widthSegment, heightSegment, 0, 0),
                        };
                        break;
                    case 1:
                        tb = new TextBlock()
                        {
                            Text = str[i].ToString(),
                            FontSize = 16,
                            FontStyle = FontStyles.Italic,
                            Padding = new Thickness(widthSegment, heightSegment, 0, 0),
                        };
                        break;
                    case 2:
                        tb = new TextBlock()
                        {
                            Text = str[i].ToString(),
                            FontSize = 16,
                            FontStyle = FontStyles.Italic,
                            FontWeight = FontWeights.Bold,
                            Padding = new Thickness(widthSegment, heightSegment, 0, 0),
                        };
                        break;
                }
                canvas.Children.Add(tb);
            }
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            HomeWindow window = new HomeWindow();
            window.Show();
            this.Hide();

            window.Closing += (obj, args) =>
            {
                this.Show();
            };
        }
    }
}
