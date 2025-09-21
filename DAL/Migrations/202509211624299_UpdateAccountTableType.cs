namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAccountTableType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Type");
        }
    }
}
