namespace Mc_Kiosk
{
    public class CartItem
    {
        public Menu Menu { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice => Menu.price * Quantity;

        public string DisplayPrice => $"₩ {TotalPrice:N0}";
    }
}