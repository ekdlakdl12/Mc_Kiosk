using System;

namespace Mc_Kiosk
{
    // CS101, CS0229 오류 방지를 위해 CartItem 클래스는 이 파일에만 정의됩니다.
    public class CartItem
    {
        // CS8618 경고 해결: Menu가 Nullable일 수 있으므로 '?' 추가
        public Menu? Menu { get; set; }
        public int Quantity { get; set; }

        // Null 안전성을 위해 Menu가 null일 경우 0을 반환
        public int TotalPrice => Menu != null ? Menu.price * Quantity : 0;

        // XAML 표시용 가격 포맷팅 속성
        public string DisplayPrice => $"₩ {TotalPrice:N0}";
    }
}