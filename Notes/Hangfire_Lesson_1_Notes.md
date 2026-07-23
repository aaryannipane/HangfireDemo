# Hangfire Course Notes

# Module 1 - Lesson 1

# Background Jobs Fundamentals

------------------------------------------------------------------------

## What is a Background Job?

A background job is a task that executes **outside the user's HTTP
request**.

The user does not wait for it to complete. Instead, the task is queued
and executed later by a background worker.

**Benefits** - Improves application performance - Improves user
experience - Reduces request time - Handles long-running tasks

------------------------------------------------------------------------

## Why Do We Need Background Jobs?

### Without Background Job

``` text
User Register
      │
      ▼
Save User
      │
      ▼
Generate PDF
      │
      ▼
Send Email
      │
      ▼
Send SMS
      │
      ▼
Return Response

Response Time ≈ 8-10 seconds
```

### With Background Job

``` text
User Register
      │
      ▼
Save User
      │
      ▼
Queue Background Job
      │
      ▼
Return Response (300 ms)

-------------------------

Background Worker

↓

Generate PDF

↓

Send Email

↓

Send SMS
```

------------------------------------------------------------------------

## Real World Examples

-   Welcome Email
-   SMS Notification
-   Invoice Generation
-   Report Generation
-   Image Compression
-   Payment Retry
-   Cache Refresh
-   Database Cleanup
-   Data Synchronization

------------------------------------------------------------------------

## When Should You Use Background Jobs?

Use background jobs when:

-   User doesn't need the result immediately.
-   Task is time-consuming.
-   Task can execute independently.
-   Failure can be retried later.

Examples:

-   Send Email
-   Send SMS
-   Generate Invoice
-   Resize Images
-   Export Excel
-   Generate Reports

------------------------------------------------------------------------

## When Should You NOT Use Background Jobs?

Don't use background jobs when:

-   User needs an immediate result.
-   Business logic depends on the result.
-   Validation must happen before continuing.

Examples:

-   Login Authentication
-   Password Validation
-   OTP Verification
-   Shopping Cart Calculation
-   Payment Validation

------------------------------------------------------------------------

## Example (Without Hangfire)

``` csharp
public IActionResult Register(RegisterModel model)
{
    SaveUser(model);

    SendWelcomeEmail(model.Email);

    SendSMS(model.Mobile);

    return Ok();
}
```

Problem:

-   User waits.
-   Slow response.
-   Possible timeout.

------------------------------------------------------------------------

## Example (Using Hangfire)

``` csharp
public IActionResult Register(RegisterModel model)
{
    SaveUser(model);

    BackgroundJob.Enqueue<EmailService>(
        x => x.SendWelcomeEmail(model.Email));

    return Ok();
}
```

Result:

-   User gets response immediately.
-   Email is processed in the background.

------------------------------------------------------------------------

# Why Not Use Other .NET Options?

## 1. async/await

Purpose: Perform asynchronous I/O without blocking the thread.

``` csharp
public async Task<IActionResult> Register()
{
    await _emailService.SendEmailAsync();
    return Ok();
}
```

**Limitation**

The HTTP request still waits until the email is sent.

Use for:

-   Database calls
-   API calls
-   File I/O

Not for:

-   Background jobs
-   Scheduled jobs
-   Recurring jobs

------------------------------------------------------------------------

## 2. Task.Run()

``` csharp
Task.Run(() =>
{
    SendEmail();
});
```

**Problems**

-   Lost if application restarts
-   No retry
-   No scheduling
-   No monitoring
-   No dashboard
-   No history

Good for:

-   CPU-intensive work
-   Parallel processing

Not suitable for production background jobs.

------------------------------------------------------------------------

## 3. BackgroundService / IHostedService

``` csharp
public class Worker : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
```

Good for:

-   Polling
-   Queue consumers
-   Cache refresh
-   Continuous workers

Not ideal for:

-   User-triggered background jobs
-   Scheduling individual jobs
-   Dashboard & retries

------------------------------------------------------------------------

## 4. Hangfire

``` csharp
BackgroundJob.Enqueue(() => SendEmail());
```

Provides:

-   Persistent storage
-   Retry support
-   Dashboard
-   Scheduling
-   Recurring jobs
-   Delayed jobs
-   Job history
-   Monitoring

------------------------------------------------------------------------

## Comparison

| Feature                  | `async/await` | `Task.Run` | `BackgroundService` | Hangfire |
|--------------------------|:-------------:|:----------:|:-------------------:|:---------:|
| Non-blocking I/O         | ✅            | ❌         | ✅                  | ❌        |
| Background execution     | ❌            | ✅         | ✅                  | ✅        |
| Persistent after restart | ❌            | ❌         | ❌                  | ✅        |
| Automatic Retry          | ❌            | ❌         | Manual              | ✅        |
| Scheduling               | ❌            | ❌         | Manual              | ✅        |
| Dashboard                | ❌            | ❌         | ❌                  | ✅        |
| Job History              | ❌            | ❌         | ❌                  | ✅        |                 ✅

------------------------------------------------------------------------

## Advantages

-   Fast response
-   Better user experience
-   Scalable
-   Reliable
-   Retry support

------------------------------------------------------------------------

## Disadvantages

-   User doesn't get immediate result.
-   Requires a background worker.
-   Needs monitoring.

------------------------------------------------------------------------

# Interview Questions

### 1. What is a Background Job?

A background job is a task that executes outside the user's HTTP
request. It is used for long-running or non-critical operations so users
receive a faster response.

### 2. Why do we use Background Jobs?

To reduce response time, improve performance, and execute long-running
tasks asynchronously.

### 3. Give some examples.

-   Sending Emails
-   SMS
-   Invoice Generation
-   Report Generation
-   Image Compression
-   Payment Retry

### 4. Why not use Task.Run()?

Task.Run() runs inside the current process. If the application restarts,
the task is lost. It also lacks persistence, retries, scheduling, and
monitoring.

### 5. Why not use async/await?

async/await makes operations asynchronous but the HTTP request still
waits for completion. It doesn't create persistent background jobs.

### 6. Can every task be moved to a background job?

No. Authentication, validation, payment verification, and other
operations required for the current request should remain synchronous.

------------------------------------------------------------------------

# Key Takeaways

-   Background Job = Execute later.
-   User doesn't wait.
-   Best for long-running tasks.
-   async/await ≠ Background Job.
-   Task.Run() ≠ Reliable Background Processing.
-   Hangfire = Persistent + Retry + Scheduling + Monitoring.
