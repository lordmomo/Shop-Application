﻿using DemoWebApplication.Models;

namespace DemoWebApplication.Service.ServiceInterface;

public interface IShopInterface
{
    public ShoppingCart GetOrCreateCart(ISession session);
    public Person? GetUserByUsername(string username);
    public List<Item> GetAllItems();

    public Item? GetItemById(int id);

    public bool DeductUserBalance(PersonDto user, List<SelectedItem> cartItems, double totalAmount);
    public void AddItemsToDb(Item item);
    public bool RemoveItemsDb(int productId);
    public bool UpdateItemInDb(Item item);
    public List<Item> getSerachedItem(string shopItem);
}
