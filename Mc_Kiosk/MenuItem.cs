using System;

namespace Mc_Kiosk
{
    // CS101, CS0229 오류 방지를 위해 Menu 클래스는 이 파일에만 정의됩니다.
    public class Menu
    {
        // CS8618 경고 해결을 위해 string 속성에 '?' 추가
        public string? id { get; set; }
        public string? name_ko { get; set; }
        public string? name_en { get; set; }
        public string? kcal { get; set; }
        public string? url { get; set; }
        public string? imageUrl { get; set; }
        public int price { get; set; }

        // XAML 표시용 가격 포맷팅 속성
        public string price_display => $"₩ {price.ToString("N0")}";
    }
}