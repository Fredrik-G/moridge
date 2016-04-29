namespace Moridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scheduleDeviationactive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduleDeviations", "MorningActive", c => c.Boolean(nullable: true, defaultValue: true));
            AddColumn("dbo.ScheduleDeviations", "AfternoonActive", c => c.Boolean(nullable: true, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduleDeviations", "AfternoonActive");
            DropColumn("dbo.ScheduleDeviations", "MorningActive");
        }
    }
}
