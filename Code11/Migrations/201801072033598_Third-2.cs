namespace Code11.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Third2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reminders", "Status", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reminders", "Status");
        }
    }
}
