namespace Code11.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Third4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LastLoginDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LastLoginDate");
        }
    }
}
