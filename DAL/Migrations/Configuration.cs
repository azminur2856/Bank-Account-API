namespace DAL.Migrations
{
    using DAL.EF.Tables;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DAL.EF.BANKContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DAL.EF.BANKContext context)
        {
            context.Set<AccountPolicy>().AddOrUpdate(
              ap => ap.AccountType,
              new AccountPolicy { AccountType = Enums.AccountType.Master, MinimumBalance = 10000m, CreatedAt = DateTime.Now },
              new AccountPolicy { AccountType = Enums.AccountType.Savings, MinimumBalance = 500m, CreatedAt = DateTime.Now },
              new AccountPolicy { AccountType = Enums.AccountType.Current, MinimumBalance = 1000m, CreatedAt = DateTime.Now }
            );

            context.SaveChanges();
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
