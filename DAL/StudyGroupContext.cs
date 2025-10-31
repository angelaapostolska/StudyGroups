using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StudyGroups.Models;
using System.Data.Entity;

namespace StudyGroups.DAL
{
    public class StudyGroupContext : DbContext
    {
        public StudyGroupContext() : base("DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<StudyGroup> StudyGroups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User -> StudyGroup (Creator) relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedStudyGroups)
                .WithRequired(sg => sg.Creator)
                .HasForeignKey(sg => sg.CreatorUserID)
                .WillCascadeOnDelete(false);

            // User <-> StudyGroup (Members) Many-to-Many
            modelBuilder.Entity<User>()
                .HasMany(u => u.JoinedStudyGroups)
                .WithMany(sg => sg.Members)
                .Map(m =>
                {
                    m.ToTable("StudyGroupMembers");
                    m.MapLeftKey("UserID");
                    m.MapRightKey("StudyGroupID");
                });

            // User -> Session (Creator) relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedSessions)
                .WithRequired(s => s.Creator)
                .HasForeignKey(s => s.CreatorUserID)
                .WillCascadeOnDelete(false);

            // User <-> Session (Attendees) Many-to-Many
            modelBuilder.Entity<User>()
                .HasMany(u => u.AttendingSessions)
                .WithMany(s => s.Attendees)
                .Map(m =>
                {
                    m.ToTable("SessionAttendees");
                    m.MapLeftKey("UserID");
                    m.MapRightKey("SessionID");
                });

            // StudyGroup -> Subject (One-to-Many)
            modelBuilder.Entity<StudyGroup>()
                .HasRequired(sg => sg.Subject)
                .WithMany(s => s.StudyGroups)
                .HasForeignKey(sg => sg.SubjectID)
                .WillCascadeOnDelete(false);
        }
    }
}