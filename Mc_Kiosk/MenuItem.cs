using System;
using System.Collections.Generic;
using System.Text;

namespace Mc_Kiosk
{
    public class Menu
    {
        // Nullable Reference Types 경고 제거를 위해 '?' 추가
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