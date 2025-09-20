namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVerificationAndUpdateUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Verifications",
                c => new
                    {
                        VerificationId = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 8000, unicode: false),
                        Type = c.Int(nullable: false),
                        IsUsed = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ExpireAt = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.VerificationId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Users", "IsEmailVerified", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "IsPhoneNumberVerified", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Verifications", "UserId", "dbo.Users");
            DropIndex("dbo.Verifications", new[] { "UserId" });
            DropColumn("dbo.Users", "IsActive");
            DropColumn("dbo.Users", "IsPhoneNumberVerified");
            DropColumn("dbo.Users", "IsEmailVerified");
            DropTable("dbo.Verifications");
        }
    }
}
