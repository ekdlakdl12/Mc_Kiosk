using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.Json;
using System.IO;
using System.Speech.Synthesis; // 음성 안내 (TTS) 기능을 위한 네임스페이스

namespace Mc_Kiosk
{
    // ⚠️ 경고: Menu와 CartItem 클래스는 중복 정의 오류를 피하기 위해 반드시 별도의 파일에 정의되어 있어야 합니다.
    public partial class MainWindow : Window
    {
        // =======================================================
        // 1. 멤버 변수 및 필드 선언
        // =======================================================

        // 음성 합성기 인스턴스: TTS 기능을 담당합니다.
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        // JSON 파일에서 로드된 버거 및 사이드 메뉴 목록 (Nullable로 선언하여 Null 경고 방지)
        private List<Menu>? BurgerMenus { get; set; }
        private List<Menu>? SideMenus { get; set; }

        // 현재 장바구니에 담긴 주문 항목 리스트
        private List<CartItem> CurrentOrder = new List<CartItem>();

        // 현재 주문의 총 금액
        private int TotalAmount = 0;

        // 카테고리 버튼의 활성화 상태(노란색) 및 비활성화 상태(회색) 색상 정의
        private static readonly Brush ActiveColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC72C"));
        private static readonly Brush InactiveColor = Brushes.LightGray;

        // =======================================================
        // 2. 생성자 및 초기화
        // =======================================================

        public MainWindow()
        {
            InitializeComponent(); // XAML UI 구성 요소 초기화

            // 음성 합성기 설정: 속도(rate)와 볼륨(volume)을 조절합니다.
            synthesizer.Rate = 1;
            synthesizer.Volume = 80;

            LoadMenuData();         // JSON 파일에서 메뉴 데이터를 로드합니다.
            CartListView.ItemsSource = CurrentOrder; // 장바구니 리스트뷰에 주문 목록을 바인딩합니다.

            // 앱 시작 시 안내 음성 출력
            SpeakAsync("맥도날드 키오스크입니다. 주문을 시작해 주세요.");
        }

        // =======================================================
        // 3. TTS 및 파일 로딩 메서드
        // =======================================================

        // 비동기 음성 재생 메서드
        // UI 스레드를 차단하지 않고 백그라운드에서 음성을 출력합니다.
        private void SpeakAsync(string text)
        {
            synthesizer.SpeakAsync(text);
        }

        // JSON 파일 로딩 일반화 함수 (제네릭 메서드)
        // 지정된 경로의 JSON 리소스 파일을 읽어와 지정된 형식(T)의 객체로 역직렬화합니다.
        private T? LoadJsonFile<T>(string path) where T : new()
        {
            try
            {
                // WPF 리소스 파일에 접근하기 위한 URI 생성
                Uri resourceUri = new Uri($"pack://application:,,,/Mc_Kiosk;component/{path}", UriKind.Absolute);
                var resourceInfo = Application.GetResourceStream(resourceUri);

                if (resourceInfo != null)
                {
                    using (var stream = resourceInfo.Stream)
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        string jsonString = reader.ReadToEnd();
                        // JSON 문자열을 T 타입 객체로 역직렬화 (Null 가능)
                        return JsonSerializer.Deserialize<T>(jsonString);
                    }
                }
                // 리소스를 찾지 못하면 기본값(null) 반환
                return default(T);
            }
            catch (Exception ex)
            {
                // 파일 로드 중 오류 발생 시 메시지 박스 출력 및 null 반환
                MessageBox.Show($"'{path}' 파일 로드 오류: {ex.Message}");
                return default(T);
            }
        }

        // JSON 파일을 읽어 List<Menu> 형태로 변환하는 함수
        private void LoadMenuData()
        {
            // 각 메뉴 데이터를 로드
            BurgerMenus = LoadJsonFile<List<Menu>>("data/mc-burger.json");
            SideMenus = LoadJsonFile<List<Menu>>("data/mc-side.json");

            // 로드 실패(null) 시 빈 리스트로 초기화하여 NullReferenceException 방지
            if (BurgerMenus == null) BurgerMenus = new List<Menu>();
            if (SideMenus == null) SideMenus = new List<Menu>();
        }

        // =======================================================
        // 4. 이벤트 핸들러: 화면 및 카테고리 전환
        // =======================================================

        // === 주문하기 버튼 클릭 시 ===
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            // 화면 전환: 시작 화면 숨기기, 주문 화면 보이기
            Screen1Grid.Visibility = Visibility.Collapsed;
            Screen2Grid.Visibility = Visibility.Visible;

            // 메뉴 리스트뷰에 기본값(버거) 할당
            MenuListView.ItemsSource = BurgerMenus;

            // 카테고리 버튼 색상 초기 설정
            BurgerButton.Background = ActiveColor;
            SideButton.Background = InactiveColor;

            // 장바구니 관련 값 초기화
            TotalAmount = 0;
            CurrentOrder.Clear();
            CartListView.Items.Refresh();
            UpdateTotalAmountDisplay();

            // TTS 호출: 주문 화면 진입 안내
            SpeakAsync("버거 메뉴 목록이 표시되었습니다.");
        }

        // === 상단 카테고리 버튼 클릭 시 ===
        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button? clickedButton = sender as Button;

            // 모든 버튼 비활성화 색으로 초기화
            BurgerButton.Background = InactiveColor;
            SideButton.Background = InactiveColor;

            string categoryName = "";

            // 클릭된 버튼을 식별하여 활성화 색상 설정 및 메뉴 데이터 변경
            if (clickedButton?.Name == "BurgerButton")
            {
                clickedButton.Background = ActiveColor;
                MenuListView.ItemsSource = BurgerMenus;
                categoryName = "버거";
            }
            else if (clickedButton?.Name == "SideButton")
            {
                clickedButton.Background = ActiveColor;
                MenuListView.ItemsSource = SideMenus;
                categoryName = "사이드";
            }

            MenuListView?.Items.Refresh(); // 메뉴 리스트뷰 갱신

            // TTS 호출: 카테고리 변경 안내
            if (!string.IsNullOrEmpty(categoryName))
            {
                SpeakAsync($"{categoryName} 메뉴 목록이 표시되었습니다.");
            }
        }

        // =======================================================
        // 5. 이벤트 핸들러: 장바구니 관리
        // =======================================================

        // === 메뉴 아이템 클릭 시 장바구니에 담기 ===
        private void MenuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 이벤트 발생 요소에서 Menu 객체를 가져옴
            if (sender is Border border && border.DataContext is Menu selectedItem)
            {
                // 장바구니에 이미 해당 항목이 있는지 찾음 (Null 검사 포함)
                var existingItem = CurrentOrder.Find(x => x.Menu != null && x.Menu.name_ko == selectedItem.name_ko);

                if (existingItem != null)
                {
                    // 항목이 있으면 수량만 증가
                    existingItem.Quantity++;
                    // ❌ TTS 제거됨: 메뉴 클릭 시 반복적인 음성 안내는 출력하지 않음
                }
                else
                {
                    // 항목이 없으면 새로 CartItem 생성하여 추가
                    CurrentOrder.Add(new CartItem { Menu = selectedItem, Quantity = 1 });
                    // ❌ TTS 제거됨: 메뉴 클릭 시 반복적인 음성 안내는 출력하지 않음
                }

                TotalAmount += selectedItem.price; // 총 금액 업데이트
                CartListView.Items.Refresh();      // 장바구니 UI 갱신
                UpdateTotalAmountDisplay();        // 총 금액 표시 업데이트
            }
        }

        // 총 금액 표시 업데이트 전용 메서드
        private void UpdateTotalAmountDisplay()
        {
            TotalAmountTextBlock.Text = $"₩ {TotalAmount:N0}";
        }

        // 장바구니 아이템의 수량을 줄이거나 제거하는 핵심 로직
        private void RemoveItemFromCart(CartItem itemToRemove)
        {
            // Null 안정성 검사
            if (itemToRemove == null || itemToRemove.Menu == null) return;

            // 총 금액에서 감소
            TotalAmount -= itemToRemove.Menu.price;

            if (itemToRemove.Quantity > 1)
            {
                // 수량이 1보다 크면 수량만 감소
                itemToRemove.Quantity--;
            }
            else
            {
                // 수량이 1이면 리스트에서 항목 자체를 제거
                CurrentOrder.Remove(itemToRemove);
            }

            // UI 및 총 금액 표시 업데이트
            CartListView.Items.Refresh();
            UpdateTotalAmountDisplay();
        }

        // 장바구니의 개별 항목 취소 버튼 클릭 이벤트 핸들러
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            // 버튼의 DataContext (즉, CartItem 객체)를 가져옴
            if (sender is Button button && button.DataContext is CartItem item)
            {
                RemoveItemFromCart(item);
            }
        }

        // =======================================================
        // 6. 이벤트 핸들러: 결제 및 초기화
        // =======================================================

        // === 결제 버튼 클릭 ===
        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            if (TotalAmount > 0)
            {
                // TTS 호출: 결제 완료 안내 (유지)
                SpeakAsync($"총 {TotalAmount:N0}원 결제가 완료되었습니다. 주문하신 상품을 준비해 드리겠습니다.");

                MessageBox.Show(
                    $"총 금액: {TotalAmount:N0}원\n결제가 완료되었습니다.",
                    "결제 완료"
                );

                // 결제 후 초기화
                TotalAmount = 0;
                CurrentOrder.Clear();
                CartListView.Items.Refresh();
                UpdateTotalAmountDisplay();

                // 첫 화면으로 돌아가기
                Screen2Grid.Visibility = Visibility.Collapsed;
                Screen1Grid.Visibility = Visibility.Visible;
            }
            else
            {
                // TTS 호출: 장바구니 비어있음 안내 (유지)
                SpeakAsync("장바구니가 비어 있습니다. 메뉴를 선택해 주세요.");

                MessageBox.Show("장바구니가 비어 있습니다.", "결제 불가");
            }
        }
    }
}