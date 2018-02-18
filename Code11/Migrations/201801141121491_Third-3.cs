namespace Code11.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Third3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Email", c => c.String(maxLength: 254));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Email");
        }
    }
}
