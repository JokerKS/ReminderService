namespace Code11.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
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
            
            CreateTable(
                "dbo.Reminders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        UserId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                        DetailId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReminderDetails", t => t.DetailId, cascadeDelete: true)
                .ForeignKey("dbo.ReminderTypes", t => t.TypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.TypeId)
                .Index(t => t.DetailId);
            
            CreateTable(
                "dbo.ReminderTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 35),
                        Password = c.String(nullable: false),
                        RegisterDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reminders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Reminders", "TypeId", "dbo.ReminderTypes");
            DropForeignKey("dbo.Reminders", "DetailId", "dbo.ReminderDetails");
            DropIndex("dbo.Users", new[] { "UserName" });
            DropIndex("dbo.ReminderTypes", new[] { "Name" });
            DropIndex("dbo.Reminders", new[] { "DetailId" });
            DropIndex("dbo.Reminders", new[] { "TypeId" });
            DropIndex("dbo.Reminders", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.ReminderTypes");
            DropTable("dbo.Reminders");
            DropTable("dbo.ReminderDetails");
        }
    }
}
