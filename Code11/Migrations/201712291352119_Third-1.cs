namespace Code11.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Third1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reminders", "DetailId", "dbo.ReminderDetails");
            DropIndex("dbo.Reminders", new[] { "DetailId" });
            AddColumn("dbo.Reminders", "DateAndTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Reminders", "Days", c => c.String());
            DropColumn("dbo.Reminders", "DetailId");
            DropTable("dbo.ReminderDetails");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ReminderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateAndTime = c.DateTime(nullable: false),
                        Days = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Reminders", "DetailId", c => c.Int(nullable: false));
            DropColumn("dbo.Reminders", "Days");
            DropColumn("dbo.Reminders", "DateAndTime");
            CreateIndex("dbo.Reminders", "DetailId");
            AddForeignKey("dbo.Reminders", "DetailId", "dbo.ReminderDetails", "Id", cascadeDelete: true);
        }
    }
}
