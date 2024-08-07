using AutoMapper;
using DemoWebApplication.Constants;
using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceInterface;
using DemoWebApplication.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DemoWebApplication.Service.ServiceImplementation;

public class UserIntefaceImpl : IUserInterface
{
    public const string userDataFilePath = Constants.Constants.userDataFilePath;
    public ApplicationDbContext db;
    public AuthServiceImpl authService;
    public IMapper mapper;
    public UserIntefaceImpl(ApplicationDbContext applicationDbContext, AuthServiceImpl authService, IMapper mapper)
    {
        this.db = applicationDbContext;
        this.authService = authService;
        this.mapper = mapper;
    }
    //public void SaveUserData(Person user)
    //{
    //    var prevUserList = JsonHandler.GetDataFromJson<Person>(userDataFilePath);
    //    prevUserList.Add(user);
    //    JsonHandler.StoreDataInJson(userDataFilePath, prevUserList);
    //}

    public List<PersonDto> GetAlUsers()
    {
        List<Person> userList =  db.Users.ToList();
        List<PersonDto> userDtoList = new List<PersonDto>();
        foreach(Person person in userList)
        {
            //PersonDto personDto = new PersonDto
            //{
            //    CustomerName = person.CustomerName,
            //    Address = person.Address,
            //    Username = person.UserName,
            //    Password = person.Password, 
            //    Balance = person.Balance,
            //    FilePath = person.FilePath
            //};
            userDtoList.Add(mapper.Map<PersonDto>(person));
        }
        return userDtoList;
    }

    public async Task<string> getRoleofUser(string id)
    {
        string roleId = db.UserRoles.FirstOrDefault(u => u.UserId == id).RoleId;
        return db.Roles.FirstOrDefault(r => r.Id == roleId).Name;
    }

    public string GetUniqueFilename(string fileName)
    {
        fileName = Path.GetFileName(fileName);
        return Path.GetFileNameWithoutExtension(fileName)
            +"_"
            +Guid.NewGuid().ToString().Substring(0,4)
            +Path.GetExtension(fileName);
    }

    public PersonDto? GetUserByUsername(string username)
    {
        Person person = db.Users.FirstOrDefault(u => u.UserName == username);
        //Person person = db.Users.AsNoTracking().FirstOrDefault(u => u.UserName == username);
        

        return mapper.Map<PersonDto>(person);
        //return personDto;
    }

   

    public string? GetUserFromToken(string token)
    {
        var principal = authService.ValidateToken(token);
        if(principal != null) 
        {
            return principal.FindFirst("username").Value;
        }
        return null;
    }

    public List<SalesHistoryResponse> GetUserTransactionHistory(string username)
    {
        Person person = db.Users.FirstOrDefault(u => u.UserName == username);
        if(person == null)
        {
            return null;
        }
        List<Sales> transactionHistory = db.Sales.Where(u => u.UserId == person.Id).ToList();
        List<SalesHistoryResponse> salesHistroy = new List<SalesHistoryResponse>();
        foreach(Sales transaction in transactionHistory)
        {
            SalesHistoryResponse saleResponse = new SalesHistoryResponse(transaction.TransactionId, transaction.ProductId, transaction.date_of_sale);
            salesHistroy.Add(saleResponse);
        }
        return salesHistroy;
    }

    public string saveFavouriteItem(PersonDto user, SelectedItem selectedItem)
    {
        if(selectedItem == null) { 
            return Message.ItemNotFound; 
        }

        if(user == null)
        {
            return Message.UserNotFound;
        }

        bool productExists = db.FavouriteItems.Any(p => p.ProductId == selectedItem.ProductId);

        if (productExists)
        {

            bool userAlreadyFavorited = db.FavouriteItems.Any(p => p.ProductId == selectedItem.ProductId && p.UserId == user.Id);

            if (userAlreadyFavorited)
            {
                return Message.ItemAlreadyInFavourites;
            }
            else
            {

                if (AddFavoriteItem(user.Id, selectedItem.ProductId))
                {
                    return Message.ItemAddedInFavourites;
                }
                else
                {
                    // Handle failure to add item if needed
                    return Message.InternalServerError;
                }

                //FavouriteItemDto favouriteItemDto = new FavouriteItemDto
                //{
                //    UserId = user.Id,
                //    ProductId = selectedItem.ProductId
                //};
                //FavouriteItem favouriteItem = mapper.Map<FavouriteItem>(favouriteItemDto);
                //db.FavouriteItems.Add(favouriteItem);
                //db.SaveChanges();
                //return Message.ItemAddedInFavourites;
            }

        }
        else
        {

            if (AddFavoriteItem(user.Id, selectedItem.ProductId))
            {
                return Message.ItemAddedInFavourites;
            }
            else
            {
                // Handle failure to add item if needed
                return Message.InternalServerError;
            }
            //FavouriteItemDto favouriteItemDto = new FavouriteItemDto
            //{
            //    UserId = user.Id,
            //    ProductId = selectedItem.ProductId
            //};
            //FavouriteItem favouriteItem = mapper.Map<FavouriteItem>(favouriteItemDto);
            //db.FavouriteItems.Add(favouriteItem);
            //db.SaveChanges();
            //return Message.ItemAddedInFavourites;
        }
      
    }

    private bool AddFavoriteItem(string userId, int productId)
    {
        try
        {
            FavouriteItemDto favouriteItemDto = new FavouriteItemDto
            {
                UserId = userId,
                ProductId = productId
            };
            FavouriteItem favouriteItem = mapper.Map<FavouriteItem>(favouriteItemDto);
            db.FavouriteItems.Add(favouriteItem);
            db.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Handle exception or log error
            Console.WriteLine($"Error adding favorite item: {ex.Message}");
            return false;
        }
    }

    public List<Item> getUserFavouriteItems(PersonDto user)
    {
        List<Item> responseItem = new List<Item>();
        var favouriteItems = db.FavouriteItems.Where( u =>  u.UserId == user.Id ).ToList();
        foreach(var item in favouriteItems)
        {
            var favItem = db.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if(favItem != null)
            {
                responseItem.Add(favItem);

            }
        }
        return responseItem;
    }

    public bool ValidateToken(string token)
    {
        var principal = authService.ValidateToken(token);
        if(principal != null)
        {
            return true;
        }
        return false;
    }
}
