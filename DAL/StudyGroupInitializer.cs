using System;
using System.Data.Entity;
using StudyGroups.Models;

namespace StudyGroups.DAL
{
    public class StudyGroupInitializer : DropCreateDatabaseIfModelChanges<StudyGroupContext>
    {
        protected override void Seed(StudyGroupContext context)
        {
            // Create default admin account
            var admin = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@studygroups.com",
                Password = "Admin123!",
                Role = "Admin"
            };

            context.Users.Add(admin);

            // Create subjects
            var subjects = new Subject[]
            {
                new Subject { Title = "Бази на податоци и SQL", Description = "Introduction to databases and SQL" },
                new Subject { Title = "Вовед во компјутерските науки", Description = "Introduction to Computer Science" },
                new Subject { Title = "Интернет програмирање на клиентски страна", Description = "Client-side Internet Programming" },
                new Subject { Title = "Примена на алгоритми и податочни структури", Description = "Application of Algorithms and Data Structures" },
                new Subject { Title = "Структурно програмирање", Description = "Structured Programming" },
                new Subject { Title = "Електроника и мобилна телефонија", Description = "Electronics and Mobile Telephony" },
                new Subject { Title = "Маркетинг", Description = "Marketing" },
                new Subject { Title = "Објектно-ориентирано програмирање", Description = "Object-Oriented Programming" },
                new Subject { Title = "Основи на Веб дизајн", Description = "Web Design Fundamentals" },
                new Subject { Title = "Основи на сајбер безбедноста", Description = "Cybersecurity Fundamentals" },
                new Subject { Title = "Веб програмирање", Description = "Web Programming" },
                new Subject { Title = "Имплементација на софтверски системи со слободен и отворен код", Description = "Implementation of Free and Open Source Software Systems" },
                new Subject { Title = "Напреден веб дизајн", Description = "Advanced Web Design" },
                new Subject { Title = "Тестирање на софтвер", Description = "Software Testing" },
                new Subject { Title = "Шаблони за дизајн на кориснички интерфејси", Description = "User Interface Design Patterns" },
                new Subject { Title = "Компјутерска анимација", Description = "Computer Animation" },
                new Subject { Title = "Концепти на информатичко општество", Description = "Information Society Concepts" },
                new Subject { Title = "Деловна Пракса", Description = "Applied Practice" },
                new Subject { Title = "Интернет Технологии", Description = "Internet Technologies" },
            };

            foreach (var subject in subjects)
            {
                context.Subjects.Add(subject);
            }

            context.SaveChanges();

            base.Seed(context);
        }
    }
}