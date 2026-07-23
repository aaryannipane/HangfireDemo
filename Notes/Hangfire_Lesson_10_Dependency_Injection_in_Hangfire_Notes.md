# Hangfire Course Notes

# Module 4 - Lesson 10

# Dependency Injection (DI) in Hangfire

------------------------------------------------------------------------

# Objective

Learn how Hangfire integrates with ASP.NET Core Dependency Injection
(DI), why `BackgroundJob.Enqueue<T>()` is preferred, how service
lifetimes work, and best practices for enterprise applications.

------------------------------------------------------------------------

# Learning Objectives

By the end of this lesson, you will understand:

-   How Hangfire uses Dependency Injection
-   Why `BackgroundJob.Enqueue<T>()` is preferred
-   Why `new Service()` is a bad practice
-   Service lifetimes (Scoped, Transient, Singleton)
-   How `DbContext` works inside Hangfire jobs
-   Why interfaces are recommended
-   Why passing IDs is better than passing entities

------------------------------------------------------------------------

# What is Dependency Injection?

Dependency Injection (DI) is a design pattern where objects receive
their dependencies from a container instead of creating them manually.

❌ Without DI

``` csharp
public class UserService
{
    private readonly EmailService _emailService = new EmailService();
}
```

✅ With DI

``` csharp
public class UserService
{
    private readonly IEmailService _emailService;

    public UserService(IEmailService emailService)
    {
        _emailService = emailService;
    }
}
```

------------------------------------------------------------------------

# Registering Services

``` csharp
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
```

The DI container now knows which implementation to create.

------------------------------------------------------------------------

# How Hangfire Uses DI

``` csharp
BackgroundJob.Enqueue<IEmailService>(
    x => x.SendWelcomeEmail("user@example.com"));
```

Hangfire **does not serialize the service instance**.

Instead, it stores:

``` text
Service Type : IEmailService
Method       : SendWelcomeEmail
Arguments    : user@example.com
```

When the worker executes the job:

``` text
Hangfire Worker
        │
        ▼
Create DI Scope
        │
        ▼
Resolve IEmailService
        │
        ▼
DI returns EmailService
        │
        ▼
Execute Method
        │
        ▼
Dispose Scope
```

------------------------------------------------------------------------

# Why Not Use `new`?

❌ Bad

``` csharp
BackgroundJob.Enqueue(() =>
{
    new EmailService().SendWelcomeEmail("user@example.com");
});
```

Problems:

-   No Dependency Injection
-   No constructor injection
-   No ILogger
-   No IConfiguration
-   No DbContext
-   Hard to test

------------------------------------------------------------------------

# Recommended Approach

``` csharp
BackgroundJob.Enqueue<IEmailService>(
    x => x.SendWelcomeEmail("user@example.com"));
```

Benefits:

-   Uses DI
-   Constructor injection works
-   Easy testing
-   Follows SOLID principles

------------------------------------------------------------------------

# Why Use Interfaces?

Interface

``` csharp
public interface IEmailService
{
    void SendWelcomeEmail(string email);
}
```

Implementation

``` csharp
public class EmailService : IEmailService
{
    public void SendWelcomeEmail(string email)
    {
        Console.WriteLine(email);
    }
}
```

Registration

``` csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

Enqueue

``` csharp
BackgroundJob.Enqueue<IEmailService>(
    x => x.SendWelcomeEmail("user@example.com"));
```

Advantages:

-   Loose coupling
-   Easy mocking
-   Easy implementation replacement
-   Better maintainability

------------------------------------------------------------------------

# Service Lifetimes

  -------------------------------------------------------------------------
  Lifetime         One Instance Per      Common Usage
  ----------- -------------------------- ----------------------------------
  Scoped          Job / HTTP Request     DbContext, Repositories, Services

  Transient        Every Resolution      Utilities, Helpers

  Singleton       Entire Application     Cache, Configuration
                       Lifetime          
  -------------------------------------------------------------------------

------------------------------------------------------------------------

# DbContext in Hangfire

``` csharp
public class EmailService : IEmailService
{
    private readonly AppDbContext _db;

    public EmailService(AppDbContext db)
    {
        _db = db;
    }
}
```

Works correctly because Hangfire creates a **new DI scope for every
job**.

``` text
Job Starts
    │
    ▼
Create Scope
    │
    ▼
Create DbContext
    │
    ▼
Execute Job
    │
    ▼
Dispose DbContext
```

------------------------------------------------------------------------

# Why Pass IDs Instead of Entities?

❌ Bad

``` csharp
BackgroundJob.Enqueue<IEmailService>(
    x => x.SendWelcomeEmail(user));
```

Problems:

-   Large serialized payload
-   Stale data
-   Higher memory usage

✅ Better

``` csharp
BackgroundJob.Enqueue<IEmailService>(
    x => x.SendWelcomeEmail(user.Id));
```

Read the latest data inside the job.

------------------------------------------------------------------------

# Internal Working

``` text
Enqueue<T>()

        │
        ▼
Store Service Type

        │
        ▼
Store Method

        │
        ▼
Store Arguments

        │
        ▼
Hangfire Worker

        │
        ▼
Create DI Scope

        │
        ▼
Resolve Service

        │
        ▼
Execute Method

        │
        ▼
Dispose Scope
```

------------------------------------------------------------------------

# Common Mistakes

  -----------------------------------------------------------------------
  Mistake                                      Better Approach
  ---------------------------------- ------------------------------------
  Using `new Service()`                    Use Dependency Injection

  Using concrete classes everywhere          Depend on interfaces

  Forgetting DI registration           Register services in Program.cs

  Passing EF entities                              Pass IDs

  Using HttpContext inside           Pass required values before queuing
  background jobs                    
  -----------------------------------------------------------------------

------------------------------------------------------------------------

# Best Practices

-   Use constructor injection.
-   Register services with DI.
-   Prefer interfaces over concrete classes.
-   Use `BackgroundJob.Enqueue<T>()`.
-   Pass IDs instead of entities.
-   Keep services stateless where possible.
-   Use Scoped lifetime for DbContext.

------------------------------------------------------------------------

# Interview Questions

## 1. How does Hangfire use Dependency Injection?

Hangfire stores the service type, method, and arguments. During
execution, it creates a new DI scope, resolves the service from the
container, executes the method, and disposes the scope.

------------------------------------------------------------------------

## 2. Why use `BackgroundJob.Enqueue<T>()`?

It allows Hangfire to resolve services using the DI container, enabling
constructor injection and proper lifetime management.

------------------------------------------------------------------------

## 3. Can we use interfaces?

Yes. This is the recommended approach.

``` csharp
builder.Services.AddScoped<IEmailService, EmailService>();

BackgroundJob.Enqueue<IEmailService>(
    x => x.SendWelcomeEmail("user@example.com"));
```

------------------------------------------------------------------------

## 4. Can DbContext be used in Hangfire?

Yes. Hangfire creates a scoped lifetime for every job, so DbContext is
created and disposed correctly.

------------------------------------------------------------------------

## 5. Why pass IDs instead of entities?

Passing IDs reduces serialization size and ensures the latest database
state is used during execution.

------------------------------------------------------------------------

# Quick Revision

  ------------------------------------------------------------------------
  API / Concept                                   Purpose
  ------------------------------ -----------------------------------------
  `BackgroundJob.Enqueue<T>()`       Queue a DI-enabled background job

  `AddScoped<T>()`                        Register scoped service

  Interface (`IEmailService`)         Loose coupling and testability

  DI Scope                               Created per Hangfire job

  Pass IDs                            Avoid stale serialized objects
  ------------------------------------------------------------------------

------------------------------------------------------------------------

# Key Takeaways

-   Hangfire integrates seamlessly with ASP.NET Core DI.
-   Prefer interfaces over concrete classes.
-   Use `BackgroundJob.Enqueue<T>()`.
-   Never use `new Service()` inside jobs.
-   DbContext works because Hangfire creates a DI scope.
-   Pass IDs instead of entities for production applications.
