namespace DemoWebApplication.Models;

public class ItemsAndCartViewModel
{
    public IEnumerable<Item> Items { get; set; }
    public ShoppingCart Cart { get; set; }
}
