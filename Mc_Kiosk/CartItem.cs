using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mc_Kiosk
{   
    //카트 클래스
    public class CartItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // 원본 메뉴 데이터 객체를 참조합니다.
        public Menu? MenuItem { get; set; }

        // --- 수량 (Quantity) 속성 ---
        private int _quantity = 1;

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPriceDisplay)); // 수량 변경 시 항목별 총 금액도 업데이트 알림
                }
            }
        }

        // --- XAML 표시용 속성 (바인딩 편의를 위해 Menu 객체에서 가져옴) ---
        public string NameKo => MenuItem.name_ko;
        public string PriceDisplay => MenuItem.price_display; // 단가

        // --- 항목별 총 가격 계산 및 포맷팅 ---
        public string TotalPriceDisplay => $"₩ {(MenuItem.price * Quantity).ToString("N0")}";

        // 생성자: 장바구니에 추가될 Menu 객체를 받습니다.
        public CartItem(Menu menu)
        {
            this.MenuItem = menu;
        }

        // PropertyChanged 이벤트 호출 헬퍼 함수
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
