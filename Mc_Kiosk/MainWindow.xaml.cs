using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.Json;

namespace Mc_Kiosk
{
    public partial class MainWindow : Window
    {
        private List<Menu>? BurgerMenus { get; set; }
        private List<Menu>? SideMenus { get; set; }

        // === 장바구니 리스트 ===
        private List<CartItem> CurrentOrder = new List<CartItem>();
        private int TotalAmount = 0;

        private static readonly Brush ActiveColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC72C"));
        private static readonly Brush InactiveColor = Brushes.LightGray;

        public MainWindow()
        {
            InitializeComponent();
            LoadMenuData();

            // 장바구니 UI 바인딩
            CartListView.ItemsSource = CurrentOrder;
        }

        private void LoadMenuData()
        {
            BurgerMenus = LoadJsonFile<List<Menu>>("data/mc-burger.json");
            SideMenus = LoadJsonFile<List<Menu>>("data/mc-side.json");

            if (BurgerMenus == null) BurgerMenus = new List<Menu>();
            if (SideMenus == null) SideMenus = new List<Menu>();
        }

        private T LoadJsonFile<T>(string path) where T : new()
        {
            try
            {
                Uri resourceUri = new Uri($"pack://application:,,,/Mc_Kiosk;component/{path}", UriKind.Absolute);
                var resourceInfo = Application.GetResourceStream(resourceUri);

                if (resourceInfo != null)
                {
                    using (var stream = resourceInfo.Stream)
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        string jsonString = reader.ReadToEnd();
                        return JsonSerializer.Deserialize<T>(jsonString);
                    }
                }
                return new T();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"'{path}' 파일 로드 오류: {ex.Message}");
                return new T();
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            Screen1Grid.Visibility = Visibility.Collapsed;
            Screen2Grid.Visibility = Visibility.Visible;

            MenuListView.ItemsSource = BurgerMenus;
            BurgerButton.Background = ActiveColor;
            SideButton.Background = InactiveColor;

            TotalAmount = 0;
            CurrentOrder.Clear();
            CartListView.Items.Refresh();
            UpdateTotalAmountDisplay();
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            BurgerButton.Background = InactiveColor;
            SideButton.Background = InactiveColor;

            if (clickedButton.Name == "BurgerButton")
            {
                clickedButton.Background = ActiveColor;
                MenuListView.ItemsSource = BurgerMenus;
            }
            else if (clickedButton.Name == "SideButton")
            {
                clickedButton.Background = ActiveColor;
                MenuListView.ItemsSource = SideMenus;
            }

            MenuListView?.Items.Refresh();
        }

        // === 메뉴 클릭 시 장바구니 담기 처리 ===
        private void MenuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Menu selectedItem)
            {
                var existingItem = CurrentOrder.Find(x => x.Menu.name_ko == selectedItem.name_ko);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    CurrentOrder.Add(new CartItem { Menu = selectedItem, Quantity = 1 });
                }

                TotalAmount += selectedItem.price;
                CartListView.Items.Refresh();
                UpdateTotalAmountDisplay();
            }
        }

        private void UpdateTotalAmountDisplay()
        {
            TotalAmountTextBlock.Text = $"₩ {TotalAmount:N0}";
        }

        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (TotalAmount > 0)
            {
                MessageBox.Show(
                    $"총 금액: {TotalAmount:N0}원\n결제가 완료되었습니다.",
                    "결제 완료"
                );

                TotalAmount = 0;
                CurrentOrder.Clear();
                CartListView.Items.Refresh();
                UpdateTotalAmountDisplay();

                Screen2Grid.Visibility = Visibility.Collapsed;
                Screen1Grid.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("장바구니가 비어 있습니다.", "결제 불가");
            }
        }
    }
}
