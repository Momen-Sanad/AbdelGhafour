using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SuperMarket.Pages
{
    public class CheckoutItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class Checkout
    {
        public List<CheckoutItem> Items { get; set; } = new List<CheckoutItem>();

        public void AddItem(CheckoutItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.Id == item.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }

        public void UpdateItemQuantity(int itemId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                if (quantity == 0)
                {
                    Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
        }

        public void RemoveItem(int itemId)
        {
            var item = Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        public decimal GetTotal()
        {
            return Items.Sum(i => i.Price * i.Quantity);
        }

        public int GetItemCount()
        {
            return Items.Sum(i => i.Quantity);
        }

    }

    public class CheckoutModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
