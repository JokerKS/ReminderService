namespace Code11.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Third : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reminders", "TypeId", "dbo.ReminderTypes");
            DropIndex("dbo.Reminders", new[] { "TypeId" });
            DropIndex("dbo.ReminderTypes", new[] { "Name" });
            AddColumn("dbo.Reminders", "TypeNumber", c => c.Int(nullable: false));
            DropColumn("dbo.Reminders", "TypeId");
            DropTable("dbo.ReminderTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ReminderTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Reminders", "TypeId", c => c.Int(nullable: false));
            DropColumn("dbo.Reminders", "TypeNumber");
            CreateIndex("dbo.ReminderTypes", "Name", unique: true);
            CreateIndex("dbo.Reminders", "TypeId");
            AddForeignKey("dbo.Reminders", "TypeId", "dbo.ReminderTypes", "Id", cascadeDelete: true);
        }
    }
}
