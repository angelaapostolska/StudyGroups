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
        }
    }
}