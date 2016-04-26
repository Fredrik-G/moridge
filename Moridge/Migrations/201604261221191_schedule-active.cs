namespace Moridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scheduleactive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DaySchedules", "MorningActive", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.DaySchedules", "AfternoonActive", c => c.Boolean(nullable: false,  defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DaySchedules", "AfternoonActive");
            DropColumn("dbo.DaySchedules", "MorningActive");
        }
    }
}
