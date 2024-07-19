using Domain;
using Domain.Entity;

using Persistence.Context;

namespace Persistence.Seeding
{
    public static class ApplicationDbContextSeed
    {
        public static void Seed(ApplicationDbContext dbContext)
        {
            if (!dbContext.Todos.Any())
            {
                dbContext.Todos.AddRange(Todos);
                dbContext.SaveChanges();
            }
        }
        public static readonly List<Todo> Todos = new List<Todo>
{
    new Todo
    {
        Id = 1,
        Title = "Implement authentication",
        Description = "Implement user authentication using JWT tokens.",
        Status = (short)TodoStatusEnum.Progressing,
        CreatedBy = "Alice",
        Created = new DateTime(2024, 07, 18),
        LastModifiedBy = "Bob",
        LastModified = new DateTime(2024, 07, 19)
    },
    new Todo
    {
        Id = 2,
        Title = "Refactor backend API",
        Description = "Refactor backend API endpoints for better performance.",
        Status = (short)TodoStatusEnum.Opening,
        CreatedBy = "Charlie",
        Created = new DateTime(2024, 07, 17)
    },
    new Todo
    {
        Id = 3,
        Title = "Write unit tests",
        Description = "Write comprehensive unit tests for frontend components.",
        Status = (short)TodoStatusEnum.Testing,
        CreatedBy = "Eve",
        Created = new DateTime(2024, 07, 16)
    },
    new Todo
    {
        Id = 4,
        Title = "Update UI/UX design",
        Description = "Update user interface and experience based on feedback.",
        Status = (short)TodoStatusEnum.Done,
        CreatedBy = "Grace",
        Created = new DateTime(2024, 07, 15),
        LastModifiedBy = "David",
        LastModified = new DateTime(2024, 07, 18)
    },
    new Todo
    {
        Id = 5,
        Title = "Fix security vulnerabilities",
        Description = "Address security vulnerabilities identified in recent audit.",
        Status = (short)TodoStatusEnum.Rejected,
        CreatedBy = "Mallory",
        Created = new DateTime(2024, 07, 14)
    },
    // Additional entries
    new Todo
    {
        Id = 6,
        Title = "Optimize database queries",
        Description = "Optimize database queries to improve application performance.",
        Status = (short)TodoStatusEnum.Opening,
        CreatedBy = "Olivia",
        Created = new DateTime(2024, 07, 13)
    },
    new Todo
    {
        Id = 7,
        Title = "Integrate third-party API",
        Description = "Integrate a third-party API for geolocation services.",
        Status = (short)TodoStatusEnum.Progressing,
        CreatedBy = "Frank",
        Created = new DateTime(2024, 07, 12)
    },
    new Todo
    {
        Id = 8,
        Title = "Implement caching mechanism",
        Description = "Implement caching to reduce server response time.",
        Status = (short)TodoStatusEnum.Opening,
        CreatedBy = "Hannah",
        Created = new DateTime(2024, 07, 11)
    },
    new Todo
    {
        Id = 9,
        Title = "Design user onboarding flow",
        Description = "Design a streamlined user onboarding flow.",
        Status = (short)TodoStatusEnum.Testing,
        CreatedBy = "Isaac",
        Created = new DateTime(2024, 07, 10)
    },
    new Todo
    {
        Id = 10,
        Title = "Improve error handling",
        Description = "Improve error handling and reporting mechanisms.",
        Status = (short)TodoStatusEnum.Progressing,
        CreatedBy = "Julia",
        Created = new DateTime(2024, 07, 9)
    },
    new Todo
    {
        Id = 11,
        Title = "Update documentation",
        Description = "Update technical documentation with latest changes.",
        Status = (short)TodoStatusEnum.Opening,
        CreatedBy = "Kevin",
        Created = new DateTime(2024, 07, 8)
    },
    new Todo
    {
        Id = 12,
        Title = "Conduct performance testing",
        Description = "Conduct performance testing to identify bottlenecks.",
        Status = (short)TodoStatusEnum.Testing,
        CreatedBy = "Laura",
        Created = new DateTime(2024, 07, 7)
    },
    new Todo
    {
        Id = 13,
        Title = "Enhance mobile responsiveness",
        Description = "Enhance mobile responsiveness for better user experience on phones and tablets.",
        Status = (short)TodoStatusEnum.Done,
        CreatedBy = "Michael",
        Created = new DateTime(2024, 07, 6),
        LastModifiedBy = "Natalie",
        LastModified = new DateTime(2024, 07, 9)
    },
    new Todo
    {
        Id = 14,
        Title = "Fix cross-browser compatibility issues",
        Description = "Fix issues related to cross-browser compatibility.",
        Status = (short)TodoStatusEnum.Progressing,
        CreatedBy = "Oscar",
        Created = new DateTime(2024, 07, 5)
    },
    new Todo
    {
        Id = 15,
        Title = "Implement analytics dashboard",
        Description = "Implement an analytics dashboard for tracking user activity.",
        Status = (short)TodoStatusEnum.Opening,
        CreatedBy = "Pamela",
        Created = new DateTime(2024, 07, 4)
    },
    new Todo
    {
        Id = 16,
        Title = "Refine search functionality",
        Description = "Refine search functionality to provide more accurate results.",
        Status = (short)TodoStatusEnum.Done,
        CreatedBy = "Quentin",
        Created = new DateTime(2024, 07, 3),
        LastModifiedBy = "Rachel",
        LastModified = new DateTime(2024, 07, 7)
    },
    new Todo
    {
        Id = 17,
        Title = "Deploy to staging environment",
        Description = "Deploy latest changes to the staging environment for testing.",
        Status = (short)TodoStatusEnum.Progressing,
        CreatedBy = "Sarah",
        Created = new DateTime(2024, 07, 2)
    },
    new Todo
    {
        Id = 18,
        Title = "Review accessibility guidelines",
        Description = "Review and implement accessibility guidelines for web content.",
        Status = (short)TodoStatusEnum.Opening,
        CreatedBy = "Thomas",
        Created = new DateTime(2024, 07, 1)
    },
    new Todo
    {
        Id = 19,
        Title = "Prepare release notes",
        Description = "Prepare release notes for upcoming software release.",
        Status = (short)TodoStatusEnum.Testing,
        CreatedBy = "Ursula",
        Created = new DateTime(2024, 06, 30)
    },
    new Todo
    {
        Id = 20,
        Title = "Set up continuous integration",
        Description = "Set up continuous integration for automated testing and deployment.",
        Status = (short)TodoStatusEnum.Done,
        CreatedBy = "Victor",
        Created = new DateTime(2024, 06, 29),
        LastModifiedBy = "Wendy",
        LastModified = new DateTime(2024, 07, 1)
    },
    new Todo
    {
        Id = 21,
        Title = "Implement email notifications",
        Description = "Implement email notifications for important system events.",
        Status = (short)TodoStatusEnum.Progressing,
        CreatedBy = "Xavier",
        Created = new DateTime(2024, 06, 28)
    },
    new Todo
    {
        Id = 22,
        Title = "Upgrade third-party libraries",
        Description = "Upgrade third-party libraries to their latest stable versions.",
        Status = (short)TodoStatusEnum.Opening,
        CreatedBy = "Yvonne",
        Created = new DateTime(2024, 06, 27)
    },
    new Todo
    {
        Id = 23,
        Title = "Monitor server performance",
        Description = "Monitor server performance metrics and optimize as necessary.",
        Status = (short)TodoStatusEnum.Testing,
        CreatedBy = "Zachary",
        Created = new DateTime(2024, 06, 26)
    },
    new Todo
    {
        Id = 24,
        Title = "Design A/B testing strategy",
        Description = "Design and implement an A/B testing strategy for user interface changes.",
        Status = (short)TodoStatusEnum.Done,
        CreatedBy = "Alice",
        Created = new DateTime(2024, 06, 25),
        LastModifiedBy = "Bob",
        LastModified = new DateTime(2024, 06, 28)
    },
    new Todo
    {
        Id = 25,
        Title = "Refactor client-side code",
        Description = "Refactor client-side JavaScript code for better maintainability.",
        Status = (short)TodoStatusEnum.Progressing,
        CreatedBy = "Charlie",
        Created = new DateTime(2024, 06, 24)
    },
    new Todo
    {
        Id = 26,
        Title = "Enhance error logging",
        Description = "Enhance error logging to capture more detailed information.",
        Status = (short)TodoStatusEnum.Opening,
        CreatedBy = "Eve",
        Created = new DateTime(2024, 06, 23)
    },
    new Todo
{
    Id = 27,
    Title = "Implement file upload functionality",
    Description = "Implement file upload functionality with support for various file types.",
    Status = (short)TodoStatusEnum.Testing,
    CreatedBy = "Grace",
    Created = new DateTime(2024, 06, 22)
},
new Todo
{
    Id = 28,
    Title = "Optimize image loading",
    Description = "Optimize image loading for faster page rendering.",
    Status = (short)TodoStatusEnum.Done,
    CreatedBy = "Henry",
    Created = new DateTime(2024, 06, 21),
    LastModifiedBy = "Ivy",
    LastModified = new DateTime(2024, 06, 24)
},
new Todo
{
    Id = 29,
    Title = "Implement multi-language support",
    Description = "Implement multi-language support for international users.",
    Status = (short)TodoStatusEnum.Progressing,
    CreatedBy = "Jack",
    Created = new DateTime(2024, 06, 20)
},
new Todo
{
    Id = 30,
    Title = "Refine user permissions",
    Description = "Refine user permissions and access control settings.",
    Status = (short)TodoStatusEnum.Opening,
    CreatedBy = "Katherine",
    Created = new DateTime(2024, 06, 19)
},
new Todo
{
    Id = 31,
    Title = "Integrate payment gateway",
    Description = "Integrate a payment gateway for handling transactions.",
    Status = (short)TodoStatusEnum.Testing,
    CreatedBy = "Luke",
    Created = new DateTime(2024, 06, 18)
},
new Todo
{
    Id = 32,
    Title = "Improve search indexing",
    Description = "Improve search indexing to provide faster and more accurate results.",
    Status = (short)TodoStatusEnum.Done,
    CreatedBy = "Megan",
    Created = new DateTime(2024, 06, 17),
    LastModifiedBy = "Nathan",
    LastModified = new DateTime(2024, 06, 20)
},
new Todo
{
    Id = 33,
    Title = "Create user profile pages",
    Description = "Create user profile pages with editable fields and avatar uploads.",
    Status = (short)TodoStatusEnum.Progressing,
    CreatedBy = "Olivia",
    Created = new DateTime(2024, 06, 16)
},
new Todo
{
    Id = 34,
    Title = "Implement automated backups",
    Description = "Implement automated backups of critical data and configurations.",
    Status = (short)TodoStatusEnum.Opening,
    CreatedBy = "Peter",
    Created = new DateTime(2024, 06, 15)
},
new Todo
{
    Id = 35,
    Title = "Enhance SEO strategies",
    Description = "Enhance SEO strategies to improve search engine rankings.",
    Status = (short)TodoStatusEnum.Testing,
    CreatedBy = "Quinn",
    Created = new DateTime(2024, 06, 14)
},
new Todo
{
    Id = 36,
    Title = "Implement real-time notifications",
    Description = "Implement real-time notifications using web sockets.",
    Status = (short)TodoStatusEnum.Done,
    CreatedBy = "Rachel",
    Created = new DateTime(2024, 06, 13),
    LastModifiedBy = "Samuel",
    LastModified = new DateTime(2024, 06, 16)
},
new Todo
{
    Id = 37,
    Title = "Upgrade server infrastructure",
    Description = "Upgrade server infrastructure to handle increased traffic.",
    Status = (short)TodoStatusEnum.Progressing,
    CreatedBy = "Tina",
    Created = new DateTime(2024, 06, 12)
},
new Todo
{
    Id = 38,
    Title = "Implement batch processing",
    Description = "Implement batch processing for handling large datasets efficiently.",
    Status = (short)TodoStatusEnum.Opening,
    CreatedBy = "Ulysses",
    Created = new DateTime(2024, 06, 11)
},
new Todo
{
    Id = 39,
    Title = "Design newsletter subscription feature",
    Description = "Design and implement a newsletter subscription feature.",
    Status = (short)TodoStatusEnum.Testing,
    CreatedBy = "Victoria",
    Created = new DateTime(2024, 06, 10)
},
new Todo
{
    Id = 40,
    Title = "Enhance error handling",
    Description = "Enhance error handling and reporting mechanisms.",
    Status = (short)TodoStatusEnum.Done,
    CreatedBy = "William",
    Created = new DateTime(2024, 06, 9),
    LastModifiedBy = "Xena",
    LastModified = new DateTime(2024, 06, 12)
},
// Add more entries as needed
new Todo
{
    Id = 41,
    Title = "Implement role-based access control",
    Description = "Implement role-based access control (RBAC) for better security management.",
    Status = (short)TodoStatusEnum.Progressing,
    CreatedBy = "Yolanda",
    Created = new DateTime(2024, 06, 8)
},
new Todo
{
    Id = 42,
    Title = "Refactor CSS stylesheets",
    Description = "Refactor CSS stylesheets to use a more modular and maintainable approach.",
    Status = (short)TodoStatusEnum.Opening,
    CreatedBy = "Zane",
    Created = new DateTime(2024, 06, 7)
},
new Todo
{
    Id = 43,
    Title = "Implement automated testing scripts",
    Description = "Implement automated testing scripts for regression and functional testing.",
    Status = (short)TodoStatusEnum.Testing,
    CreatedBy = "Amy",
    Created = new DateTime(2024, 06, 6)
},
new Todo
{
    Id = 44,
    Title = "Enhance mobile app synchronization",
    Description = "Enhance synchronization between web and mobile app versions.",
    Status = (short)TodoStatusEnum.Done,
    CreatedBy = "Ben",
    Created = new DateTime(2024, 06, 5),
    LastModifiedBy = "Catherine",
    LastModified = new DateTime(2024, 06, 8)
},
new Todo
{
    Id = 45,
    Title = "Integrate user feedback system",
    Description = "Integrate a system for collecting and processing user feedback.",
    Status = (short)TodoStatusEnum.Progressing,
    CreatedBy = "Daniel",
    Created = new DateTime(2024, 06, 4)
},
new Todo
{
    Id = 46,
    Title = "Update data privacy policies",
    Description = "Update data privacy policies to comply with new regulations.",
    Status = (short)TodoStatusEnum.Opening,
    CreatedBy = "Emily",
    Created = new DateTime(2024, 06, 3)
},
new Todo
{
    Id = 47,
    Title = "Optimize database schema",
    Description = "Optimize database schema for improved query performance.",
    Status = (short)TodoStatusEnum.Testing,
    CreatedBy = "Finn",
    Created = new DateTime(2024, 06, 2)
},
new Todo
{
    Id = 48,
    Title = "Implement data encryption",
    Description = "Implement data encryption to protect sensitive user information.",
    Status = (short)TodoStatusEnum.Done,
    CreatedBy = "Gabrielle",
    Created = new DateTime(2024, 06, 1),
    LastModifiedBy = "Henry",
    LastModified = new DateTime(2024, 06, 4)
},
new Todo
{
    Id = 49,
    Title = "Design user dashboard",
    Description = "Design and implement a user dashboard for personalized insights.",
    Status = (short)TodoStatusEnum.Progressing,
    CreatedBy = "Ian",
    Created = new DateTime(2024, 05, 31)
},
new Todo
{
    Id = 50,
    Title = "Implement event logging",
    Description = "Implement event logging for tracking application usage.",
    Status = (short)TodoStatusEnum.Opening,
    CreatedBy = "Jasmine",
    Created = new DateTime(2024, 05, 30)
},
};
    }
}