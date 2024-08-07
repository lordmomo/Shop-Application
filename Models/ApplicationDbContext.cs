using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Microsoft.Extensions.Options;

namespace DemoWebApplication.Models;

public class ApplicationDbContext : IdentityDbContext<Person>
{
    protected readonly IConfiguration _configuration;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));

    }

    public DbSet<Person> Users { get; set; }

    public DbSet<Item> Items { get; set; }

    public DbSet<Sales> Sales { get; set; }

    public DbSet<FavouriteItem> FavouriteItems { get; set; }

    //protected override void OnModelCreating(ModelBuilder builder)
    //{
    //    base.OnModelCreating(builder);
    //}
}
