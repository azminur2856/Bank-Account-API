namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAddressTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        StreetAddress = c.String(nullable: false, maxLength: 255, unicode: false),
                        City = c.String(nullable: false, maxLength: 100, unicode: false),
                        State = c.String(nullable: false, maxLength: 100, unicode: false),
                        PostalCode = c.String(nullable: false, maxLength: 20, unicode: false),
                        Country = c.String(nullable: false, maxLength: 100, unicode: false),
                        Type = c.Int(nullable: false),
                        IsVerified = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "UserId", "dbo.Users");
            DropIndex("dbo.Addresses", new[] { "UserId" });
            DropTable("dbo.Addresses");
        }
    }
}
