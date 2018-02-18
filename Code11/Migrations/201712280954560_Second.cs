namespace Code11.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Second : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Reminders", "StartDate");
            DropColumn("dbo.Reminders", "EndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reminders", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Reminders", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
