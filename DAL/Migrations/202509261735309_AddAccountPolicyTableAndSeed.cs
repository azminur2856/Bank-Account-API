namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountPolicyTableAndSeed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountPolicies",
                c => new
                    {
                        PolicyId = c.Int(nullable: false, identity: true),
                        AccountType = c.Int(nullable: false),
                        MinimumBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PolicyId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AccountPolicies");
        }
    }
}
