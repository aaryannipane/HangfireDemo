# Hangfire Course Notes

# Module 1 - Lesson 2

# Why Not Task.Run()? Why Do We Need Hangfire?

------------------------------------------------------------------------

# Objective

Understand why Hangfire is preferred over Task.Run() and other
in-process background execution techniques for production applications.

------------------------------------------------------------------------

# What is Task.Run()?

`Task.Run()` executes code on a ThreadPool thread.

``` csharp
Task.Run(() =>
{
    Console.WriteLine("Running in background");
});
```

It moves work to another thread **inside the same application process**.

------------------------------------------------------------------------

# Example

``` csharp
public IActionResult Register(RegisterModel model)
{
    SaveUser(model);

    Task.Run(() =>
    {
        EmailService.SendWelcomeEmail(model.Email);
    });

    return Ok();
}
```

Flow:

``` text
Register Request
      │
      ▼
Save User
      │
      ▼
Task.Run()
      │
      ▼
Return Response

-------------------

ThreadPool Thread

↓

Send Email
```

------------------------------------------------------------------------

# Problems with Task.Run()

## 1. Task is lost after application restart

If IIS recycles, the application crashes, or a deployment occurs, the
task is terminated.

``` text
Task.Run()

↓

Application Restart

↓

Task Lost
```

------------------------------------------------------------------------

## 2. No Retry Support

If sending the email fails, there is no automatic retry.

------------------------------------------------------------------------

## 3. No Monitoring

You cannot easily answer:

-   Which jobs failed?
-   Which jobs succeeded?
-   Which jobs are running?

------------------------------------------------------------------------

## 4. No Scheduling

Task.Run() cannot execute:

-   Tomorrow at 10 AM
-   Every day at 9 PM
-   Every Monday

------------------------------------------------------------------------

## 5. No Job History

After execution, there is no built-in history of the task.

------------------------------------------------------------------------

## 6. Not Suitable for Multiple Servers

In a load-balanced environment, Task.Run() executes only on the server
that received the request.

If that server crashes, the task is lost.

------------------------------------------------------------------------

# How Hangfire Solves These Problems

``` text
Application

↓

Hangfire Client

↓

SQL Server Storage

↓

Hangfire Server

↓

Execute Job
```

Benefits:

-   Persistent Storage
-   Automatic Retry
-   Dashboard
-   Scheduling
-   Recurring Jobs
-   Delayed Jobs
-   Job History
-   Multi-server support

------------------------------------------------------------------------

# Example

``` csharp
BackgroundJob.Enqueue<EmailService>(
    x => x.SendWelcomeEmail("user@example.com"));
```

Flow:

``` text
Controller

↓

Create Job

↓

Store in SQL Server

↓

Return Response

-------------------

Hangfire Worker

↓

Execute Email

↓

Mark Job Succeeded
```

------------------------------------------------------------------------

# Task.Run() vs Hangfire

  Feature                Task.Run()   Hangfire
  ---------------------- ------------ ----------
  Background execution   ✅           ✅
  Persistent             ❌           ✅
  Survives restart       ❌           ✅
  Retry support          ❌           ✅
  Dashboard              ❌           ✅
  Scheduling             ❌           ✅
  Recurring Jobs         ❌           ✅
  Job History            ❌           ✅
  Production Ready       ❌           ✅

------------------------------------------------------------------------

# When Should You Use Task.Run()?

Use Task.Run() for:

-   CPU-intensive calculations
-   Parallel processing
-   Short-lived background work
-   Desktop applications

Example:

``` csharp
Task.Run(() =>
{
    CalculateLargeReport();
});
```

------------------------------------------------------------------------

# When Should You Use Hangfire?

Use Hangfire for:

-   Sending Emails
-   SMS
-   Invoice Generation
-   Report Generation
-   Payment Retry
-   Image Processing
-   Scheduled Jobs
-   Recurring Jobs
-   Long-running background work

------------------------------------------------------------------------

# Advantages of Hangfire

-   Reliable
-   Persistent
-   Retry support
-   Dashboard
-   Scheduling
-   Easy monitoring
-   Scalable

------------------------------------------------------------------------

# Disadvantages

-   Additional database/storage required
-   Background server must be running
-   Slightly more setup than Task.Run()

------------------------------------------------------------------------

# Interview Questions

## 1. Why not use Task.Run() instead of Hangfire?

Task.Run() executes inside the current application process. If the
application restarts, the task is lost. It also lacks retries,
scheduling, monitoring, persistence, and job history. Hangfire provides
all these features.

------------------------------------------------------------------------

## 2. Is Task.Run() bad?

No. It is useful for CPU-bound or short-lived background work inside the
current process. It is not intended as a reliable background job
framework.

------------------------------------------------------------------------

## 3. What happens to Task.Run() if IIS restarts?

The task stops immediately and unfinished work is lost.

------------------------------------------------------------------------

## 4. Which would you use for sending registration emails?

Hangfire, because email delivery should be reliable and retry
automatically if it fails.

------------------------------------------------------------------------

## 5. Can Task.Run() schedule a job for tomorrow?

No. It has no scheduling capability.

------------------------------------------------------------------------

# Common Mistakes

-   Using Task.Run() for payment processing.
-   Using Task.Run() for invoice generation.
-   Assuming Task.Run() survives application restarts.
-   Ignoring exceptions inside Task.Run().

------------------------------------------------------------------------

# Key Takeaways

-   Task.Run() = Another thread in the same process.
-   Hangfire = Persistent background job processing.
-   Task.Run() is not a replacement for Hangfire.
-   Use Hangfire when reliability, retries, scheduling, and monitoring
    are required.
