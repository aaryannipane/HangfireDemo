# Hangfire Course Notes

# Module 3 - Lesson 6

# Delayed Jobs (`BackgroundJob.Schedule()`)

------------------------------------------------------------------------

# Objective

Learn how to execute a job **once** after a delay or at a specific
future time.

------------------------------------------------------------------------

# What is a Delayed Job?

A delayed job is a background job that executes **only once**, but
**after a specified delay**.

``` text
Create Job
    │
    ▼
Scheduled
    │
    ▼
Wait
    │
    ▼
Enqueued
    │
    ▼
Processing
    │
    ▼
Succeeded
```

------------------------------------------------------------------------

# Syntax

``` csharp
BackgroundJob.Schedule(
    () => SendReminder(),
    TimeSpan.FromMinutes(5));
```

Schedule at a specific date/time:

``` csharp
BackgroundJob.Schedule(
    () => SendReminder(),
    DateTime.Now.AddHours(2));
```

------------------------------------------------------------------------

# Real World Examples

-   Payment Reminder
-   Trial Expiry Email
-   OTP Reminder
-   Follow-up Email
-   Auction Winner Notification
-   Delayed Push Notification

------------------------------------------------------------------------

# Enqueue vs Schedule

  | Feature               | Enqueue | Schedule |
|-----------------------|:-------:|:--------:|
| Execute Immediately   | ✅      | ❌       |
| Execute Once          | ✅      | ✅       |
| Execute After Delay   | ❌      | ✅       |

------------------------------------------------------------------------

# Internal Working

``` text
Controller
    │
    ▼
Create Job
    │
    ▼
State = Scheduled
    │
    ▼
Scheduled Time Reached
    │
    ▼
Move To Queue
    │
    ▼
Hangfire Server Executes Job
```

------------------------------------------------------------------------

# Best Practices

-   Use for reminders and follow-ups.
-   Never use `Thread.Sleep()` for delayed execution.
-   Prefer UTC when scheduling absolute times.

------------------------------------------------------------------------

# Interview Questions

## What is a Delayed Job?

A job that executes once after a specified delay or future date/time.

------------------------------------------------------------------------

## Difference between Enqueue() and Schedule()?

-   `Enqueue()` executes as soon as a worker is available.
-   `Schedule()` waits until the specified time before entering the
    queue.

------------------------------------------------------------------------

# Key Takeaways

-   Executes only once.
-   Waits before execution.
-   Uses `BackgroundJob.Schedule()`.
