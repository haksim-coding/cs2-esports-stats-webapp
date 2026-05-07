---
name: ef-workflow
description: "Use when working on Entity Framework, DbContext, EF Core migrations, SQL Server setup, connection strings, model annotations, navigation properties, seeding, repository swaps, or database update scripts."
user-invocable: true
---

# EF Workflow

Use this skill when you need to set up, extend, or troubleshoot Entity Framework in an ASP.NET Core app.

## What this skill covers
- EF Core package setup
- DbContext creation and registration
- SQL Server connection strings, including local or Docker-backed SQL Server
- Model annotations and relationship cleanup
- Migration creation and database updates
- Seed data and initial schema validation
- Replacing mock repositories with EF repositories

## How to use it
Invoke this skill when you are about to touch any of the following:
- `DbContext`
- entity annotations like `[Key]`, `[ForeignKey]`, or navigation properties
- `Program.cs` service registration for EF
- `appsettings*.json` connection strings
- `dotnet ef migrations add`
- `dotnet ef database update`
- repository implementations that should read or write from SQL Server

## Recommended workflow
1. Check the current model shape and list the entities that should become tables.
2. Add or fix EF packages in the project.
3. Create the DbContext and register it in `Program.cs`.
4. Add or correct entity annotations and navigation properties.
5. Create the first migration.
6. Apply the migration to the Docker SQL Server.
7. Swap mock repositories for EF repositories one by one.
8. Run a build and fix any model binding, relationship, or Razor issues.

## Practical rules
- Keep `Id` as the primary key unless the lab requires otherwise.
- Use `virtual` navigation properties and `ICollection<T>` for collection relationships where the model needs lazy-loading style navigation or clear relationship mapping.
- Match the connection string name in code and configuration exactly.
- Put migration commands in the project that owns the DbContext, and point `--startup-project` at the web app that contains configuration.
- After model changes, always regenerate or update the migration before judging the database state.

## Validation checklist
- `dotnet build`
- `dotnet ef migrations add <Name> --startup-project <WebProject> --context <DbContextName>`
- `dotnet ef database update --startup-project <WebProject> --context <DbContextName>`
- Run the app and verify a few list/detail pages still render

## When to stop and inspect
- If a relationship is not mapping as expected
- If EF cannot construct the DbContext
- If migrations target the wrong assembly
- If a repository still depends on mock in-memory data after the swap
