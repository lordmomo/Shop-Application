namespace DemoWebApplication.Models;

public class Ledger
{
    public string transactionId;
    public string username;
    public DateTime date;
    public List<Item> item;
    public double totalAmount;

    public Ledger(string transactionId, string username, DateTime date, List<Item> item, double amount)
    {
        this.transactionId = transactionId;
        this.username = username;
        this.date = date;
        this.item = item;
        this.totalAmount = amount;
    }
}
