using DemoWebApplication.Constants;
using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceInterface;
using DemoWebApplication.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace DemoWebApplication.Controllers;

public class ShopController : Controller
{
    private readonly IMemoryCache _memoryCache;

    private readonly string _cacheKey = "ProductData";

    private readonly IHttpContextAccessor _httpContextAccessor;

    private IShopInterface _shopInterface;

    private IUserInterface _userInterface;
    public ShopController(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor, IShopInterface shopInterface, IUserInterface userInterface)
    {
        _memoryCache = memoryCache;
        _httpContextAccessor = httpContextAccessor;
        _shopInterface = shopInterface;
        _userInterface = userInterface;
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin,Moderator")]
    [HttpGet]
    [Route(Urls.showAllItemsApi)]
    public ActionResult<IEnumerable<Item>> ShowAllItemsInShop()
    {
        if (!_memoryCache.TryGetValue(_cacheKey, out List<Item> items))
        {
            items = _shopInterface.GetAllItems();
            Log.Debug(Message.CacheCreatedForListofItems);
            _memoryCache.Set(_cacheKey, items, TimeSpan.FromMinutes(10));
        }
        return Json(items);

    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User,Moderator")]
    [HttpPost]
    [Route(Urls.deductUserBalanceApi)]
    public IActionResult DeductBalance([FromBody] DeduceBalanceRequestModel model)
    {

        var user = _userInterface.GetUserByUsername(model.Username);
        if (user == null)
        {
            return Json(new { success = false, message = Message.UserNotFound });

        }
        var result = _shopInterface.DeductUserBalance(user, model.CartItems, model.TotalAmount);
        if (!result)
        {
            return Json(new { success = false, message = Message.FailedToDeduceBalanceMessage });

        }
        else
        {
            return Json(new { success = true, message = Message.SuccessfulDeduceBalanceMessage });

        }

    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost]
    [Route(Urls.addItemsInShopApi)]
    public IActionResult AddItemsInShop([FromBody] Item item)
    {
            try
            {
                _shopInterface.AddItemsToDb(item);
                UpdateItemListCache();
                Log.Debug(Message.CacheUpdatedAfterDataAdded);
            return Json(new { success = true, message = Message.ItemAddedToShop });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            return Json(new { success = false, message = Message.ErrorAddingItemToShop });

            }

    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost]
    [Route(Urls.removeItemsInShopApi)]

    public IActionResult RemoveItemsInShop([FromBody]int productId)
    {
        try
        {
            var removeCheckFlg = _shopInterface.RemoveItemsDb(productId);
            if (removeCheckFlg)
            {
                UpdateItemListCache();
                Log.Debug(Message.CacheUpdatedAfterDataRemoval);
                return Json(new { success = true, message = Message.ItemDeletedFromShop });

            }
            else
            {
                return Json(new { success = false, message = Message.ItemNotFound });

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return Json(new { success = false, message = Message.ErrorDeletingItemFromShop });

        }
    }



    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost]
    [Route(Urls.updateItemsInShopApi)]
    public IActionResult UpdateItemsInShop([FromBody] Item item)
    {
        try
        {
            var updateCheckFlg = _shopInterface.UpdateItemInDb(item);
            if (updateCheckFlg)
            {
                UpdateItemListCache();
                Log.Debug(Message.CacheUpdatedAfterDataUpdate);
                return Json(new { success = true, message = Message.ItemEditedOfShop });
            }
            else
            {
                return Json(new { success = false, message = Message.ItemNotFound });

            }

        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.ToString());
            return Json(new { success = false, message = Message.ErrorEditingItemOfShop });
        }
    }
    public void UpdateItemListCache()
    {
        if (_memoryCache.TryGetValue(_cacheKey, out List<Item> items))
        {
            items = _shopInterface.GetAllItems();
            _memoryCache.Set(_cacheKey, items, TimeSpan.FromMinutes(10));
        }
    }

    [HttpGet]
    [Route(Urls.searchItemInShop)]
    public ActionResult<IEnumerable<Item>> SearchedItem(string shopItem)
    {
        return Json(_shopInterface.getSerachedItem(shopItem));
    }

    [HttpGet]
    [Route(Urls.getUserFavouriteItems)]
    public IActionResult GetUserFavouriteItem(string username)
    {
        var user = _userInterface.GetUserByUsername(username);
        if (user == null)
        {
            return Json(Message.UserNotFound);
        }

        List<Item> favItems = _userInterface.getUserFavouriteItems(user);
        return Json(favItems);
    }

    [HttpPost]
    [Route(Urls.saveFavouriteItem)]
    public  IActionResult SaveFavouriteItem(string username, [FromBody] SelectedItem favouriteItem)
    {
        var user =  _userInterface.GetUserByUsername(username);
        if(user == null)
        {
            return Json(Message.UserNotFound);
        }

        string val = _userInterface.saveFavouriteItem(user, favouriteItem);
        return Json(val);

    }
}
