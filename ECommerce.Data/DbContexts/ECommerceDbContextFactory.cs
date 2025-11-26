using ECommerce.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class ECommerceDbContextFactory : IDesignTimeDbContextFactory<ECommerceDbContext>
{
    public ECommerceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ECommerceDbContext>();
        // SQL Server connection string örneği
        optionsBuilder.UseSqlServer("Server=.;Database=ECommerceDb;Trusted_Connection=True;");

        return new ECommerceDbContext(optionsBuilder.Options);
    }
}