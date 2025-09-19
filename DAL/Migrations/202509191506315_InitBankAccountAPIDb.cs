namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitBankAccountAPIDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        AccountId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        AccountNumber = c.String(nullable: false, maxLength: 20, unicode: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.AccountId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.CreatedBy)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100, unicode: false),
                        Email = c.String(nullable: false, maxLength: 100, unicode: false),
                        PhoneNumber = c.String(nullable: false, maxLength: 15, unicode: false),
                        PasswordHash = c.String(nullable: false, maxLength: 8000, unicode: false),
                        Role = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Details = c.String(unicode: false, storeType: "text"),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Tokens",
                c => new
                    {
                        TokenId = c.Int(nullable: false, identity: true),
                        TokenKey = c.String(nullable: false, maxLength: 8000, unicode: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ExpireAt = c.DateTime(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TokenId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        SourceAccountId = c.Int(),
                        DestinationAccountId = c.Int(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                        PerformedBy = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Accounts", t => t.DestinationAccountId)
                .ForeignKey("dbo.Users", t => t.PerformedBy, cascadeDelete: true)
                .ForeignKey("dbo.Accounts", t => t.SourceAccountId)
                .Index(t => t.SourceAccountId)
                .Index(t => t.DestinationAccountId)
                .Index(t => t.PerformedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accounts", "UserId", "dbo.Users");
            DropForeignKey("dbo.Accounts", "CreatedBy", "dbo.Users");
            DropForeignKey("dbo.Transactions", "SourceAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Transactions", "PerformedBy", "dbo.Users");
            DropForeignKey("dbo.Transactions", "DestinationAccountId", "dbo.Accounts");
            DropForeignKey("dbo.Tokens", "UserId", "dbo.Users");
            DropForeignKey("dbo.AuditLogs", "UserId", "dbo.Users");
            DropForeignKey("dbo.Accounts", "User_UserId", "dbo.Users");
            DropIndex("dbo.Transactions", new[] { "PerformedBy" });
            DropIndex("dbo.Transactions", new[] { "DestinationAccountId" });
            DropIndex("dbo.Transactions", new[] { "SourceAccountId" });
            DropIndex("dbo.Tokens", new[] { "UserId" });
            DropIndex("dbo.AuditLogs", new[] { "UserId" });
            DropIndex("dbo.Accounts", new[] { "User_UserId" });
            DropIndex("dbo.Accounts", new[] { "CreatedBy" });
            DropIndex("dbo.Accounts", new[] { "UserId" });
            DropTable("dbo.Transactions");
            DropTable("dbo.Tokens");
            DropTable("dbo.AuditLogs");
            DropTable("dbo.Users");
            DropTable("dbo.Accounts");
        }
    }
}
