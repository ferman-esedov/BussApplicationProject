using BusApplication.Models;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace BusApplication.View
{

    public partial class MainView : Window, INotifyPropertyChanged
    {
        public ApplicationIdCredentialsProvider Provider { get; set; } = new ApplicationIdCredentialsProvider(ConfigurationManager.ConnectionStrings["MapKey"].ConnectionString);
        public SolidColorBrush ColorBus { get; set; }
        public string? BusNumber { get; set; }
        private static List<Pushpin> b = new();
        static Pushpin pushpin = new Pushpin();
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern int MessageBoxTimeout(IntPtr hwnd, String text, String title, uint type, Int16 wLanguageId, Int32 milliseconds);
        private BakuBus? bakuBus;
        public BakuBus? BakuBus
        {

            get { return bakuBus; }
            set
            {
                bakuBus = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        DispatcherTimer timer = new DispatcherTimer();

        public MainView()
        {
            DataContext = this;
            InitializeComponent();
            map.CredentialsProvider = Provider;
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private async void Window_First_Loaded(object sender, RoutedEventArgs e)
        {
            var client = new HttpClient();
            var jsonString = await client.GetStringAsync("https://www.bakubus.az/az/ajax/apiNew1");
            BakuBus = JsonSerializer.Deserialize<BakuBus>(jsonString);
        }
        private SolidColorBrush RandomColor()
        {
            Random r = new Random();
            return new SolidColorBrush(Color.FromRgb((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255)));
        }


        bool flag = true;


        private void BakuBusList()
        {
            if (flag)
            {
                flag = false;
                for (int i = 0; i < bakuBus?.BUS?.Count; i++)
                {
                    cbox_search.Items.Add(bakuBus.BUS[i].Attributes?.DISPLAY_ROUTE_CODE);
                    pushpin = new();
                    string? name = bakuBus.BUS[i].Attributes?.PLATE;
                    pushpin.Name = $"N{name}";
                    double longitude = Convert.ToDouble(bakuBus.BUS[i].Attributes?.LONGITUDE);
                    double latitude = Convert.ToDouble(bakuBus.BUS[i].Attributes?.LATITUDE);
                    Style style = (Style)(Resources["styl"]);
                    MessageBoxTimeout((System.IntPtr)0, "", "", 0, 0, 1);
                    BusNumber = bakuBus.BUS[i].Attributes?.DISPLAY_ROUTE_CODE;
                    ColorBus = RandomColor();
                    pushpin.Tag = i.ToString();
                    pushpin.Style = style;
                    pushpin.MouseEnter += Pshp_MouseEnter;
                    pushpin.MouseLeave += Pshp_MouseLeave;

                    pushpin.Location = new Location(latitude, longitude);
                    b.Add(pushpin);
                    map.Children.Add(b[b.Count - 1]);
                }

            }
            else
            {
                try
                {
                    for (int i = 0; i < BakuBus?.BUS?.Count; i++)
                    {
                        if ($"N{bakuBus?.BUS?[i].Attributes?.PLATE}" == b[i].Name)
                        {
                            double num1 = Convert.ToDouble(bakuBus?.BUS?[i].Attributes?.LONGITUDE);
                            double num2 = Convert.ToDouble(bakuBus?.BUS?[i].Attributes?.LATITUDE);
                            b[i].Location = new Location(num2, num1);
                        }
                    };
                }
                catch { }
            }
        }

        private void Pshp_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Pushpin)
            {
                Values.Background = new SolidColorBrush(Colors.Transparent);
                Values.IsHitTestVisible = false;
                Busimage.Visibility = Visibility.Hidden;
                Text1.Content = "";
                Sep.Visibility = Visibility.Hidden;
                Text2.Content = "";
                Text3.Content = "";
                txt_Current.Content = "";
                txt_Prev.Content = "";
                txt_Route.Content = "";
            }
        }

        private void Pshp_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (sender is Pushpin psp)
            {

                Values.IsHitTestVisible = true;
                Values.Background = new SolidColorBrush(Colors.White);
                Busimage.Visibility = Visibility.Visible;
                Text1.Content = bakuBus.BUS[Convert.ToInt32(psp.Tag)].Attributes.ROUTE_NAME;
                Sep.Visibility = Visibility.Visible;
                Text2.Content = bakuBus.BUS[Convert.ToInt32(psp.Tag)].Attributes.CURRENT_STOP;
                Text3.Content = bakuBus.BUS[Convert.ToInt32(psp.Tag)].Attributes.PREV_STOP;
                txt_Current.Content = "Current_Stop";
                txt_Prev.Content = "Prev_Stop";
                txt_Route.Content = "Route_Name";


            }
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            using HttpClient client = new HttpClient();
            var jsonString = await client.GetStringAsync("https://www.bakubus.az/az/ajax/apiNew1");
            BakuBus = JsonSerializer.Deserialize<BakuBus>(jsonString);
            BakuBusList();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AboutUs a = new AboutUs();
            a.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TicketAndPrices a = new TicketAndPrices();
            a.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            TermsOfUse a = new TermsOfUse();
            a.ShowDialog();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Career a = new Career();
            a.ShowDialog();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ContactUs a = new ContactUs();
            a.ShowDialog();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            News a = new News();

            a.ShowDialog();
        }

        private void Button_Click_Search(object sender, RoutedEventArgs e)
        {
            flag = true;
            ClearMap();
            if (flag)
            {
                flag = false;
                for (int i = 0; i < bakuBus?.BUS?.Count; i++)
                {
                    if (cbox_search.Text == bakuBus.BUS[i].Attributes?.DISPLAY_ROUTE_CODE)
                    {

                        pushpin = new();
                        string? name = bakuBus.BUS[i].Attributes?.PLATE;
                        pushpin.Name = $"N{name}";
                        double longitude = Convert.ToDouble(bakuBus.BUS[i].Attributes?.LONGITUDE);
                        double latitude = Convert.ToDouble(bakuBus.BUS[i].Attributes?.LATITUDE);
                        Style style = (Style)(Resources["styl"]);
                        MessageBoxTimeout((System.IntPtr)0, "", "", 0, 0, 1);
                        BusNumber = bakuBus.BUS[i].Attributes?.DISPLAY_ROUTE_CODE;

                        ColorBus = RandomColor();
                        pushpin.Tag = i.ToString();
                        pushpin.Style = style;

                        pushpin.MouseEnter += Pshp_MouseEnter;
                        pushpin.MouseLeave += Pshp_MouseLeave;

                        pushpin.Location = new Location(latitude, longitude);
                        b.Add(pushpin);
                        map.Children.Add(b[b.Count - 1]);
                    }
                }
            }
            else
            {
                try
                {
                    for (int i = 0; i < BakuBus?.BUS?.Count; i++)
                    {
                        if (cbox_search.Text == bakuBus.BUS[i].Attributes?.DISPLAY_ROUTE_CODE)
                        {

                            if ($"N{bakuBus?.BUS?[i].Attributes?.PLATE}" == b[i].Name)
                            {
                                double num1 = Convert.ToDouble(bakuBus?.BUS?[i].Attributes?.LONGITUDE);
                                double num2 = Convert.ToDouble(bakuBus?.BUS?[i].Attributes?.LATITUDE);
                                b[i].Location = new Location(num2, num1);
                            }
                        }
                    };
                }
                catch { }
            }
        }
        private void ClearMap()
        {
            map.Children.Clear();
            
        }

    }
}
