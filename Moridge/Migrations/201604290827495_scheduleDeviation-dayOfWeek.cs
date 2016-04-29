namespace Moridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class scheduleDeviationdayOfWeek : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduleDeviations", "DayOfWeek", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduleDeviations", "DayOfWeek");
        }
    }
}
