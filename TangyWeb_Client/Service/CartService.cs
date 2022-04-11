using Blazored.LocalStorage;

using Tangy_Common;

using TangyWeb_Client.Service.IService;
using TangyWeb_Client.ViewModel;

namespace TangyWeb_Client.Service;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorage;
    public event Action OnChange;
    public CartService(ILocalStorageService localStorageService)
    {
        _localStorage = localStorageService;
    }

    async Task ICartService.IncrementCart(ShoppingCart cartToIncrement)
    {
        var cart = await _localStorage.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);
        bool itemInCart = false;

        if (cart == null)
        {
            cart = new List<ShoppingCart>();
        }

        foreach (var item in cart)
        {
            if (item.ProductId == cartToIncrement.ProductId && item.ProductPriceId == cartToIncrement.ProductPriceId)
            {
                itemInCart = true;
                item.Count += cartToIncrement.Count;
            }
        }

        if (!itemInCart)
        {
            cart.Add(new ShoppingCart()
            {
                ProductId = cartToIncrement.ProductId,
                ProductPriceId = cartToIncrement.ProductPriceId,
                Count = cartToIncrement.Count
            });
        }

        await _localStorage.SetItemAsync(SD.ShoppingCart, cart);
        OnChange.Invoke();
    }

    async Task ICartService.DecrementCart(ShoppingCart cartToDecrement)
    {
        var cart = await _localStorage.GetItemAsync<List<ShoppingCart>>(SD.ShoppingCart);

        for (int i = 0; i < cart.Count; i++)
        {
            if (cart[i].ProductId == cartToDecrement.ProductId && cart[i].ProductPriceId == cartToDecrement.ProductPriceId)
            {
                if(cart[i].Count == 1 || cartToDecrement.Count == 0)
                {
                    cart.Remove(cart[i]);
                }
                else
                {
                    cart[i].Count -= cartToDecrement.Count;
                }
            }
        }

        await _localStorage.SetItemAsync(SD.ShoppingCart, cart);
        OnChange.Invoke();
    }
}
