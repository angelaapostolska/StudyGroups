namespace StudyGroups.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMaxAttendeesToSession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "MaxAttendees", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "MaxAttendees");
        }
    }
}
