namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixAccountRelationships : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Accounts", "User_UserId", "dbo.Users");
            DropIndex("dbo.Accounts", new[] { "User_UserId" });
            DropColumn("dbo.Accounts", "User_UserId");
        }

        public override void Down()
        {
            AddColumn("dbo.Accounts", "User_UserId", c => c.Int());
            CreateIndex("dbo.Accounts", "User_UserId");
            AddForeignKey("dbo.Accounts", "User_UserId", "dbo.Users", "UserId");
        }
    }
}
