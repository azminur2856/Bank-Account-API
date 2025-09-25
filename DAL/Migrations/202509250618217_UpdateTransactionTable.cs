namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTransactionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "Fees", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Transactions", "SourceType", c => c.Int());
            AddColumn("dbo.Transactions", "DestinationType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "DestinationType");
            DropColumn("dbo.Transactions", "SourceType");
            DropColumn("dbo.Transactions", "Fees");
        }
    }
}
