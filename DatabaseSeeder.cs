using Bogus;
using StudyGroups.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

public class DatabaseSeeder
{
    private readonly string _connectionString;

    public DatabaseSeeder(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void SeedDatabase()
    {
        // Load existing data from database
        var users = LoadExistingUsers();
        var subjects = LoadExistingSubjects();

        Console.WriteLine($"Found {users.Count} existing users and {subjects.Count} existing subjects");

        // Generate and insert study groups
        var studyGroups = GenerateStudyGroups(15, users, subjects);
        InsertStudyGroups(studyGroups);

        // Reload study groups from database to get the actual IDs
        var insertedStudyGroups = LoadExistingStudyGroups();
        Console.WriteLine($"Loaded {insertedStudyGroups.Count} study groups from database");

        // Now generate sessions using the correct study group IDs
        var sessions = GenerateSessions(50, insertedStudyGroups, users);
        InsertSessions(sessions);

        // Reload sessions to get actual IDs for ratings
        var insertedSessions = LoadExistingSessions();
        Console.WriteLine($"Loaded {insertedSessions.Count} sessions from database");

        // Generate ratings using the correct session IDs
        var ratings = GenerateRatings(100, users, insertedSessions);
        InsertRatings(ratings);

        Console.WriteLine("Database seeding completed!");
    }

    private List<User> LoadExistingUsers()
    {
        var users = new List<User>();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT UserID, FirstName, LastName, Email, Password, Role FROM Users", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        UserID = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Email = reader.GetString(3),
                        Password = reader.GetString(4),
                        Role = reader.GetString(5)
                    });
                }
            }
        }
        return users;
    }

    private List<Subject> LoadExistingSubjects()
    {
        var subjects = new List<Subject>();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT SubjectID, Title, Description FROM Subjects", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    subjects.Add(new Subject
                    {
                        SubjectID = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                    });
                }
            }
        }
        return subjects;
    }

    private List<StudyGroup> LoadExistingStudyGroups()
    {
        var studyGroups = new List<StudyGroup>();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT StudyGroupID, Name, Description, SubjectID, CreatorUserID FROM StudyGroups", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    studyGroups.Add(new StudyGroup
                    {
                        StudyGroupID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        SubjectID = reader.GetInt32(3),
                        CreatorUserID = reader.GetInt32(4)
                    });
                }
            }
        }
        return studyGroups;
    }

    private List<Session> LoadExistingSessions()
    {
        var sessions = new List<Session>();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT SessionID, Date, Duration, StudyGroupID, CreatorUserID FROM Sessions", connection);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    sessions.Add(new Session
                    {
                        SessionID = reader.GetInt32(0),
                        Date = reader.GetDateTime(1),
                        Duration = reader.GetInt32(2),
                        StudyGroupID = reader.GetInt32(3),
                        CreatorUserID = reader.GetInt32(4)
                    });
                }
            }
        }
        return sessions;
    }

    private List<StudyGroup> GenerateStudyGroups(int count, List<User> users, List<Subject> subjects)
    {
        var groupFaker = new Faker<StudyGroup>()
            .RuleFor(sg => sg.Name, f => f.Company.CompanyName() + " Study Group")
            .RuleFor(sg => sg.Description, f => f.Lorem.Paragraph())
            .RuleFor(sg => sg.SubjectID, f => f.PickRandom(subjects).SubjectID)
            .RuleFor(sg => sg.CreatorUserID, f => f.PickRandom(users).UserID);

        return groupFaker.Generate(count);
    }

    private List<Session> GenerateSessions(int count, List<StudyGroup> studyGroups, List<User> users)
    {
        var sessionFaker = new Faker<Session>()
            .RuleFor(s => s.Date, f => f.Date.Between(DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(1)))
            .RuleFor(s => s.Duration, f => f.Random.Int(30, 180))
            .RuleFor(s => s.StudyGroupID, f => f.PickRandom(studyGroups).StudyGroupID)
            .RuleFor(s => s.CreatorUserID, f => f.PickRandom(users).UserID);

        return sessionFaker.Generate(count);
    }

    private List<Rating> GenerateRatings(int count, List<User> users, List<Session> sessions)
    {
        var ratingFaker = new Faker<Rating>()
            .RuleFor(r => r.Score, f => f.Random.Int(1, 5))
            .RuleFor(r => r.UserID, f => f.PickRandom(users).UserID)
            .RuleFor(r => r.SessionID, f => f.PickRandom(sessions).SessionID);

        return ratingFaker.Generate(count);
    }

    private void InsertStudyGroups(List<StudyGroup> studyGroups)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            foreach (var sg in studyGroups)
            {
                var command = new SqlCommand(
                    "INSERT INTO StudyGroups (Name, Description, SubjectID, CreatorUserID) " +
                    "VALUES (@Name, @Description, @SubjectID, @CreatorUserID); SELECT SCOPE_IDENTITY();",
                    connection);

                command.Parameters.AddWithValue("@Name", sg.Name);
                command.Parameters.AddWithValue("@Description", sg.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SubjectID", sg.SubjectID);
                command.Parameters.AddWithValue("@CreatorUserID", sg.CreatorUserID);

                sg.StudyGroupID = Convert.ToInt32(command.ExecuteScalar());
            }
        }
        Console.WriteLine($"Inserted {studyGroups.Count} study groups");
    }

    private void InsertSessions(List<Session> sessions)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            foreach (var session in sessions)
            {
                var command = new SqlCommand(
                    "INSERT INTO Sessions (Date, Duration, StudyGroupID, CreatorUserID) " +
                    "VALUES (@Date, @Duration, @StudyGroupID, @CreatorUserID); SELECT SCOPE_IDENTITY();",
                    connection);

                command.Parameters.AddWithValue("@Date", session.Date);
                command.Parameters.AddWithValue("@Duration", session.Duration);
                command.Parameters.AddWithValue("@StudyGroupID", session.StudyGroupID);
                command.Parameters.AddWithValue("@CreatorUserID", session.CreatorUserID);

                session.SessionID = Convert.ToInt32(command.ExecuteScalar());
            }
        }
        Console.WriteLine($"Inserted {sessions.Count} sessions");
    }

    private void InsertRatings(List<Rating> ratings)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            foreach (var rating in ratings)
            {
                var command = new SqlCommand(
                    "INSERT INTO Ratings (Score, UserID, SessionID) " +
                    "VALUES (@Score, @UserID, @SessionID)",
                    connection);

                command.Parameters.AddWithValue("@Score", rating.Score);
                command.Parameters.AddWithValue("@UserID", rating.UserID);
                command.Parameters.AddWithValue("@SessionID", rating.SessionID);

                command.ExecuteNonQuery();
            }
        }
        Console.WriteLine($"Inserted {ratings.Count} ratings");
    }
}