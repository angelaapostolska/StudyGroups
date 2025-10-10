namespace StudyGroups.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModelsWithOwnershipAndMembership : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sessions", "SubjectID", "dbo.Subjects");
            DropForeignKey("dbo.Subjects", "StudyGroupID", "dbo.StudyGroups");
            DropForeignKey("dbo.UserStudyGroups", "User_UserID", "dbo.Users");
            DropForeignKey("dbo.UserStudyGroups", "StudyGroup_StudyGroupID", "dbo.StudyGroups");
            DropIndex("dbo.Sessions", new[] { "SubjectID" });
            DropIndex("dbo.Subjects", new[] { "StudyGroupID" });
            DropIndex("dbo.UserStudyGroups", new[] { "User_UserID" });
            DropIndex("dbo.UserStudyGroups", new[] { "StudyGroup_StudyGroupID" });
            CreateTable(
                "dbo.StudyGroupMembers",
                c => new
                    {
                        UserID = c.Int(nullable: false),
                        StudyGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserID, t.StudyGroupID })
                .ForeignKey("dbo.StudyGroups", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.StudyGroupID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.StudyGroupID);
            
            CreateTable(
                "dbo.SessionAttendees",
                c => new
                    {
                        SessionID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SessionID, t.UserID })
                .ForeignKey("dbo.Sessions", t => t.SessionID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.SessionID)
                .Index(t => t.UserID);
            
            AddColumn("dbo.Sessions", "StudyGroupID", c => c.Int(nullable: false));
            AddColumn("dbo.Sessions", "CreatorUserID", c => c.Int(nullable: false));
            AddColumn("dbo.Subjects", "Description", c => c.String(maxLength: 500));
            AddColumn("dbo.StudyGroups", "SubjectID", c => c.Int(nullable: false));
            AddColumn("dbo.StudyGroups", "CreatorUserID", c => c.Int(nullable: false));
            CreateIndex("dbo.Sessions", "StudyGroupID");
            CreateIndex("dbo.Sessions", "CreatorUserID");
            CreateIndex("dbo.StudyGroups", "SubjectID");
            CreateIndex("dbo.StudyGroups", "CreatorUserID");
            AddForeignKey("dbo.StudyGroups", "CreatorUserID", "dbo.Users", "UserID");
            AddForeignKey("dbo.Sessions", "StudyGroupID", "dbo.StudyGroups", "StudyGroupID", cascadeDelete: true);
            AddForeignKey("dbo.StudyGroups", "SubjectID", "dbo.Subjects", "SubjectID");
            AddForeignKey("dbo.Sessions", "CreatorUserID", "dbo.Users", "UserID");
            DropColumn("dbo.Sessions", "SubjectID");
            DropColumn("dbo.Subjects", "StudyGroupID");
            DropTable("dbo.UserStudyGroups");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserStudyGroups",
                c => new
                    {
                        User_UserID = c.Int(nullable: false),
                        StudyGroup_StudyGroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_UserID, t.StudyGroup_StudyGroupID });
            
            AddColumn("dbo.Subjects", "StudyGroupID", c => c.Int(nullable: false));
            AddColumn("dbo.Sessions", "SubjectID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Sessions", "CreatorUserID", "dbo.Users");
            DropForeignKey("dbo.SessionAttendees", "UserID", "dbo.Users");
            DropForeignKey("dbo.SessionAttendees", "SessionID", "dbo.Sessions");
            DropForeignKey("dbo.StudyGroups", "SubjectID", "dbo.Subjects");
            DropForeignKey("dbo.Sessions", "StudyGroupID", "dbo.StudyGroups");
            DropForeignKey("dbo.StudyGroupMembers", "StudyGroupID", "dbo.Users");
            DropForeignKey("dbo.StudyGroupMembers", "UserID", "dbo.StudyGroups");
            DropForeignKey("dbo.StudyGroups", "CreatorUserID", "dbo.Users");
            DropIndex("dbo.SessionAttendees", new[] { "UserID" });
            DropIndex("dbo.SessionAttendees", new[] { "SessionID" });
            DropIndex("dbo.StudyGroupMembers", new[] { "StudyGroupID" });
            DropIndex("dbo.StudyGroupMembers", new[] { "UserID" });
            DropIndex("dbo.StudyGroups", new[] { "CreatorUserID" });
            DropIndex("dbo.StudyGroups", new[] { "SubjectID" });
            DropIndex("dbo.Sessions", new[] { "CreatorUserID" });
            DropIndex("dbo.Sessions", new[] { "StudyGroupID" });
            DropColumn("dbo.StudyGroups", "CreatorUserID");
            DropColumn("dbo.StudyGroups", "SubjectID");
            DropColumn("dbo.Subjects", "Description");
            DropColumn("dbo.Sessions", "CreatorUserID");
            DropColumn("dbo.Sessions", "StudyGroupID");
            DropTable("dbo.SessionAttendees");
            DropTable("dbo.StudyGroupMembers");
            CreateIndex("dbo.UserStudyGroups", "StudyGroup_StudyGroupID");
            CreateIndex("dbo.UserStudyGroups", "User_UserID");
            CreateIndex("dbo.Subjects", "StudyGroupID");
            CreateIndex("dbo.Sessions", "SubjectID");
            AddForeignKey("dbo.UserStudyGroups", "StudyGroup_StudyGroupID", "dbo.StudyGroups", "StudyGroupID", cascadeDelete: true);
            AddForeignKey("dbo.UserStudyGroups", "User_UserID", "dbo.Users", "UserID", cascadeDelete: true);
            AddForeignKey("dbo.Subjects", "StudyGroupID", "dbo.StudyGroups", "StudyGroupID", cascadeDelete: true);
            AddForeignKey("dbo.Sessions", "SubjectID", "dbo.Subjects", "SubjectID", cascadeDelete: true);
        }
    }
}
