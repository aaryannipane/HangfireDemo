# Hangfire Course Notes

# Module 2 - Lesson 5

# Fire-and-Forget Jobs (`BackgroundJob.Enqueue()`)

------------------------------------------------------------------------

# Objective

Learn how to create your first Hangfire background job, understand how
`BackgroundJob.Enqueue()` works internally, and when to use
Fire-and-Forget jobs.

------------------------------------------------------------------------

# What is a Fire-and-Forget Job?

A Fire-and-Forget job is executed **only once**, as soon as a Hangfire
worker is available.

It is the most commonly used Hangfire job type.

``` text
HTTP Request

↓

Create Job

↓

Return Response

--------------------

Hangfire Worker

↓

Execute Job Once

↓

Finished
```

------------------------------------------------------------------------

# Real World Examples

-   Welcome Email
-   Password Reset Email
-   SMS Notification
-   Generate Invoice
-   Resize Image
-   Upload File
-   Audit Logging
-   Push Notification

------------------------------------------------------------------------

# Basic Syntax

``` csharp
BackgroundJob.Enqueue(() => Method());
```

Using Dependency Injection (Recommended)

``` csharp
BackgroundJob.Enqueue<EmailService>(
    x => x.SendWelcomeEmail("user@example.com"));
```

------------------------------------------------------------------------

# Example

## Service

``` csharp
public class EmailService
{
    public void SendWelcomeEmail(string email)
    {
        Console.WriteLine($"Welcome email sent to {email}");
    }
}
```

Register the service

``` csharp
builder.Services.AddScoped<EmailService>();
```

Controller

``` csharp
[HttpPost]
public IActionResult Register(RegisterModel model)
{
    SaveUser(model);

    BackgroundJob.Enqueue<EmailService>(
        x => x.SendWelcomeEmail(model.Email));

    return Ok();
}
```

------------------------------------------------------------------------

# Internal Working

When you call:

``` csharp
BackgroundJob.Enqueue<EmailService>(
    x => x.SendWelcomeEmail(model.Email));
```

Hangfire **does not execute** the method immediately.

Instead it performs:

``` text
Controller

↓

Create Job

↓

Serialize Method + Arguments

↓

Store Job in SQL Server

↓

Return HTTP Response

---------------------------

Hangfire Server

↓

Read Job

↓

Resolve EmailService (DI)

↓

Execute Method

↓

Update Job Status
```

------------------------------------------------------------------------

# Job Lifecycle

Successful execution:

``` text
Created

↓

Enqueued

↓

Processing

↓

Succeeded
```

If an exception occurs:

``` text
Created

↓

Enqueued

↓

Processing

↓

Failed
```

------------------------------------------------------------------------

# Why Use Dependency Injection?

Instead of:

``` csharp
BackgroundJob.Enqueue(() =>
    new EmailService().SendWelcomeEmail("abc@test.com"));
```

Use:

``` csharp
BackgroundJob.Enqueue<EmailService>(
    x => x.SendWelcomeEmail("abc@test.com"));
```

Benefits:

-   Uses ASP.NET Core DI
-   Supports constructor injection
-   Easier testing
-   Cleaner code

------------------------------------------------------------------------

# Why Not Task.Run()?

  Task.Run()        Hangfire Enqueue
  ----------------- --------------------
  Lost on restart   Persistent
  No retries        Automatic retries
  No dashboard      Dashboard
  No scheduling     Scheduling support
  No job history    Complete history

Use `Task.Run()` for short-lived CPU work.

Use Hangfire for reliable production background jobs.

------------------------------------------------------------------------

# Dashboard

After creating a job, the Dashboard displays:

-   Enqueued
-   Processing
-   Succeeded
-   Failed

Clicking a job shows:

-   Job Id
-   Method
-   Parameters
-   State History
-   Processing Time
-   Exception Details (if failed)

------------------------------------------------------------------------

# Common Mistakes

  -----------------------------------------------------------------------
  Mistake                  Better Approach
  ------------------------ ----------------------------------------------
  Writing business logic   Move logic to a service
  inside controller        

  Using                    Use Dependency Injection
  `new EmailService()`     

  Forgetting to register   Register in DI container
  service                  

  Expecting `Enqueue()` to It only creates the job
  execute immediately      
  -----------------------------------------------------------------------

------------------------------------------------------------------------

# Best Practices

-   Keep jobs small and focused.
-   Put business logic in services.
-   Use dependency injection.
-   Keep controllers thin.
-   Monitor failed jobs from the Dashboard.

------------------------------------------------------------------------

# Interview Questions

## 1. What is a Fire-and-Forget Job?

A background job that executes **once** as soon as a worker becomes
available.

------------------------------------------------------------------------

## 2. Does `BackgroundJob.Enqueue()` execute the method immediately?

No. It serializes the method call, stores it in persistent storage, and
returns. The Hangfire Server executes it later.

------------------------------------------------------------------------

## 3. Who executes the job?

The Hangfire Server.

------------------------------------------------------------------------

## 4. Can Hangfire use Dependency Injection?

Yes. Hangfire integrates with ASP.NET Core Dependency Injection and
resolves registered services when executing jobs.

------------------------------------------------------------------------

## 5. Why keep business logic in services instead of controllers?

To follow the Single Responsibility Principle, improve reusability,
simplify testing, and allow the same logic to be used by controllers and
background jobs.

------------------------------------------------------------------------

# Quick Revision

  API                         Purpose
  --------------------------- ------------------------------
  `BackgroundJob.Enqueue()`   Create a Fire-and-Forget job
  `AddScoped<T>()`            Register service for DI
  Hangfire Server             Executes queued jobs
  Dashboard                   Monitor job execution

------------------------------------------------------------------------

# Key Takeaways

-   Fire-and-Forget jobs execute only once.
-   `Enqueue()` creates a job; it does not execute it.
-   Jobs are executed by the Hangfire Server.
-   Prefer `BackgroundJob.Enqueue<T>()` with Dependency Injection.
-   Monitor execution using the Hangfire Dashboard.
-   Keep business logic inside services, not controllers.
