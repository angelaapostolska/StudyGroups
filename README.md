Study Groups MVC Application
	Course: Internet Technologies
	Student: Angela Apostolska
	Framework: ASP.NET MVC 5 (.NET Framework 4.7.2)

Project Overview
	A web-based study group management system that allows users to organize study sessions, manage subjects, and rate session effectiveness.
	The application provides full CRUD (Create, Read, Update, Delete) functionality for managing study groups, users, subjects, study sessions, and ratings.

Features

	-User Management – Register and manage user profiles with authentication
	-Study Group Organization – Create and manage study groups with descriptions
	-Subject Tracking – Add subjects to study groups for organized learning
	-Session Scheduling – Schedule study sessions with date and duration
	-Rating System – Rate study sessions to track effectiveness (1–5 scale)	
	-Technology Stack

Framework: ASP.NET MVC 5
.NET Version: 4.7.2
ORM: Entity Framework 6.4.4
Database: SQL Server LocalDB
Frontend: Razor Views, Bootstrap, jQuery
Language: C#

Database Schema

User:

	UserID (Primary Key)

	FirstName

	LastName

	Email

	Password	

	StudyGroups (Navigation)

StudyGroup:

	StudyGroupID (Primary Key)

	Name

	Description

	Users (Navigation)

	Subjects (Navigation)

Subject:

	SubjectID (Primary Key)

	Title

	StudyGroupID (Foreign Key)

	Sessions (Navigation)

Session:

	SessionID (Primary Key)

	Date

	Duration (in minutes)

	SubjectID (Foreign Key)

	Ratings (Navigation)

Rating:

	RatingID (Primary Key)

	Score (1–5)

	UserID (Foreign Key)

	SessionID (Foreign Key)

Relationships""

	User ↔ StudyGroup: Many-to-Many

	StudyGroup → Subject: One-to-Many

	Subject → Session: One-to-Many

	Session → Rating: One-to-Many

	User → Rating: One-to-Many

Installation & Setup

Prerequisites

Visual Studio 2017 or later

.NET Framework 4.7.2

SQL Server LocalDB (included with Visual Studio)

Setup Instructions

	Clone or download the project

	Open the solution file (.sln) in Visual Studio

	Restore NuGet Packages

	Right-click on the solution → Restore NuGet Packages

	Ensure Entity Framework 6.4.4 is installed

	Configure Database Connection

	Connection string is already configured in Web.config

	Database name: StudyGroupDb

	Apply Database Migrations

	Open Package Manager Console (Tools → NuGet Package Manager → Package Manager Console)

	Run the following commands:
	Enable-Migrations
	Add-Migration InitialCreate
	Update-Database

	Build and Run

	Press F5 or click the Start button

	The application will open in your default browser

Project Structure:

StudyGroups/
│
├── Controllers/ – MVC Controllers (UsersController, StudyGroupsController, etc.)
├── Models/ – Entity models (User, StudyGroup, Subject, Session, Rating)
├── Views/ – Razor views for each controller
│ ├── Users/
│ ├── StudyGroups/
│ ├── Subjects/
│ ├── Sessions/
│ └── Ratings/
├── DAL/ – Data Access Layer (StudyGroupContext.cs)
├── App_Start/ – Configuration files (RouteConfig, BundleConfig)
└── Web.config – Application configuration

Usage

Accessing the Application
After running the application, navigate to:

	Study Groups: https://localhost:44384/StudyGroups

	Users: https://localhost:44384/Users

	Subjects: https://localhost:44384/Subjects

	Sessions: https://localhost:44384/Sessions

	Ratings: https://localhost:44384/Ratings

Basic Operations

Create a User – Navigate to Users → Create New

Create a Study Group – Navigate to Study Groups → Create New

Add Subjects – Navigate to Subjects → Create New (select study group)

Schedule Sessions – Navigate to Sessions → Create New (select subject)

Rate Sessions – Navigate to Sessions Details  → Create New if you were a member of the session

Key Implementation Details

DbContext Configuration

Located in DAL/StudyGroupContext.cs

Inherits from DbContext

Contains DbSet properties for each entity

Data Annotations
	Used for:

	Validation ([Required], [Range], [EmailAddress])

	Display formatting ([StringLength], [DataType])

	Relationships ([ForeignKey])

Entity Framework Code First

Models define the database schema

Migrations track schema changes

Database creation and updates are automatic

Future Enhancements

	Implement ASP.NET Identity for secure authentication

	Add real-time notifications for upcoming sessions

	Integrate a calendar view for session scheduling

	Add file upload for study materials

	Implement group chat functionality

	Create a dashboard with statistics and analytics

Troubleshooting

	Build Errors

	Ensure all NuGet packages are restored

	Verify Entity Framework 6.4.4 is installed

	Confirm .NET Framework 4.7.2 is installed

	Database Errors

	Check connection string in Web.config

	Ensure SQL Server LocalDB is running

	Re-run migrations if schema changes aren’t applied

	404 Errors

	Confirm controller names match route patterns

	Ensure views exist in correct folders

	Verify route mappings in RouteConfig.cs

License

	This project was created for educational purposes as part of the Internet Technologies course.

Contact

	Angela Apostolska
	Internet Technologies Course Project

	Last Updated: October 2025