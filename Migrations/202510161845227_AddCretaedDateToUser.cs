namespace StudyGroups.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCretaedDateToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "JoinedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "JoinedDate");
        }
    }
}
