namespace StudyGroups.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        RatingID = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        SessionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RatingID)
                .ForeignKey("dbo.Sessions", t => t.SessionID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.SessionID);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        SessionID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Duration = c.Int(nullable: false),
                        SubjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SessionID)
                .ForeignKey("dbo.Subjects", t => t.SubjectID, cascadeDelete: true)
                .Index(t => t.SubjectID);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        SubjectID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        StudyGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubjectID)
                .ForeignKey("dbo.StudyGroups", t => t.StudyGroupID, cascadeDelete: true)
                .Index(t => t.StudyGroupID);
            
            CreateTable(
                "dbo.StudyGroups",
                c => new
                    {
                        StudyGroupID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.StudyGroupID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.UserStudyGroups",
                c => new
                    {
                        User_UserID = c.Int(nullable: false),
                        StudyGroup_StudyGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_UserID, t.StudyGroup_StudyGroupID })
                .ForeignKey("dbo.Users", t => t.User_UserID, cascadeDelete: true)
                .ForeignKey("dbo.StudyGroups", t => t.StudyGroup_StudyGroupID, cascadeDelete: true)
                .Index(t => t.User_UserID)
                .Index(t => t.StudyGroup_StudyGroupID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ratings", "UserID", "dbo.Users");
            DropForeignKey("dbo.UserStudyGroups", "StudyGroup_StudyGroupID", "dbo.StudyGroups");
            DropForeignKey("dbo.UserStudyGroups", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.Subjects", "StudyGroupID", "dbo.StudyGroups");
            DropForeignKey("dbo.Sessions", "SubjectID", "dbo.Subjects");
            DropForeignKey("dbo.Ratings", "SessionID", "dbo.Sessions");
            DropIndex("dbo.UserStudyGroups", new[] { "StudyGroup_StudyGroupID" });
            DropIndex("dbo.UserStudyGroups", new[] { "User_UserID" });
            DropIndex("dbo.Subjects", new[] { "StudyGroupID" });
            DropIndex("dbo.Sessions", new[] { "SubjectID" });
            DropIndex("dbo.Ratings", new[] { "SessionID" });
            DropIndex("dbo.Ratings", new[] { "UserID" });
            DropTable("dbo.UserStudyGroups");
            DropTable("dbo.Users");
            DropTable("dbo.StudyGroups");
            DropTable("dbo.Subjects");
            DropTable("dbo.Sessions");
            DropTable("dbo.Ratings");
        }
    }
}
