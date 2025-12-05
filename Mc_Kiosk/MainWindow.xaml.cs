using System;
using System.Collections.Generic;
// using System.Collections.ObjectModel; // 👈 제거
// using System.Linq;                    // 👈 제거
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace Mc_Kiosk
{
    public partial class MainWindow : Window
    {
        // List<Menu> 사용으로 롤백, Nullable 경고 제거를 위해 '?' 추가
        private List<Menu>? BurgerMenus { get; set; }
        private List<Menu>? SideMenus { get; set; }

        // 장바구니 리스트를 List<Menu>로 롤백
        private List<Menu> CurrentOrder = new List<Menu>();
        private int TotalAmount = 0;

        private static readonly Brush ActiveColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC72C"));
        private static readonly Brush InactiveColor = Brushes.LightGray;

        public MainWindow()
        {
            InitializeComponent();
            LoadMenuData();
            // CartListView.ItemsSource = CurrentOrder; 👈 장바구니 목록 연결 로직 제거
        }

        private void LoadMenuData()
        {
            // LoadJsonFile 호출 시 제네릭 타입을 List<Menu>로 사용
            BurgerMenus = LoadJsonFile<List<Menu>>("data/mc-burger.json");
            SideMenus = LoadJsonFile<List<Menu>>("data/mc-side.json");

            if (BurgerMenus == null) BurgerMenus = new List<Menu>();
            if (SideMenus == null) SideMenus = new List<Menu>();
        }

        private T LoadJsonFile<T>(string path) where T : new()
        {
            try
            {
                // Uri 경로 설정 (프로젝트 이름: Mc_Kiosk)
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
                MessageBox.Show($"'{path}' 파일 로드 및 파싱 오류: {ex.Message}", "데이터 오류 경고");
                return new T();
            }
        }

        // --- 이벤트 핸들러 시작 ---

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Screen1Grid != null && Screen2Grid != null)
            {
                Screen1Grid.Visibility = Visibility.Collapsed;
                Screen2Grid.Visibility = Visibility.Visible;

                if (MenuListView != null)
                {
                    MenuListView.ItemsSource = BurgerMenus;
                }

                BurgerButton.Background = ActiveColor;
                SideButton.Background = InactiveColor;

                TotalAmount = 0;
                CurrentOrder.Clear();
                UpdateTotalAmountDisplay();
            }
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            BurgerButton.Background = InactiveColor;
            SideButton.Background = InactiveColor;

            if (clickedButton != null && MenuListView != null)
            {
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
            }
            MenuListView?.Items.Refresh();
        }

        /// <summary>
        /// 메뉴 항목 클릭 시 장바구니에 추가하고 총액을 업데이트합니다. (수량/목록 처리 없음)
        /// </summary>
        private void MenuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Menu selectedItem)
            {
                // 1. 장바구니에 추가 (수량 체크 없이 무조건 추가)
                CurrentOrder.Add(selectedItem);

                // 2. 총액 업데이트
                TotalAmount += selectedItem.price;

                // 3. 화면에 총액 표시 업데이트
                UpdateTotalAmountDisplay();
            }
        }

        private void UpdateTotalAmountDisplay()
        {
            TotalAmountTextBlock.Text = $"₩ {TotalAmount.ToString("N0")}";
        }

        /// <summary>
        /// '결제하기' 버튼 클릭 시 결제 완료 메시지를 표시하고 초기화합니다.
        /// </summary>
        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (TotalAmount > 0)
            {
                MessageBox.Show($"총 계산 금액: {TotalAmount.ToString("N0")}원\n\n결제가 완료되었습니다.\n감사합니다!", "결제 완료");

                TotalAmount = 0;
                CurrentOrder.Clear();
                UpdateTotalAmountDisplay();

                Screen2Grid.Visibility = Visibility.Collapsed;
                Screen1Grid.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("장바구니가 비어있습니다. 메뉴를 선택해주세요.", "결제 오류");
            }
        }
    }
}