# Hangfire Course Notes

# Module 3 - Lesson 7

# Recurring Jobs & Cron Expressions

------------------------------------------------------------------------

# Objective

Learn how to execute jobs repeatedly using Cron expressions.

------------------------------------------------------------------------

# What is a Recurring Job?

A recurring job executes repeatedly based on a schedule.

Examples:

-   Every minute
-   Every hour
-   Every day
-   Every Monday
-   Every month

------------------------------------------------------------------------

# What is Cron?

Cron is a scheduling format that tells Hangfire **when** to execute a
recurring job.

------------------------------------------------------------------------

# Cron Format

``` text
* * * * *
│ │ │ │ │
│ │ │ │ └── Day of Week (0-6)
│ │ │ └──── Month (1-12)
│ │ └────── Day of Month (1-31)
│ └──────── Hour (0-23)
└────────── Minute (0-59)
```

------------------------------------------------------------------------

# Special Characters

  Symbol   Meaning           Example
  -------- ----------------- -------------
  `*`      Every value       `* * * * *`
  `,`      Multiple values   `9,18`
  `-`      Range             `9-17`
  `/`      Step interval     `*/5`

------------------------------------------------------------------------

# Common Cron Examples

  Cron            Meaning
  --------------- --------------------------
  `* * * * *`     Every minute
  `*/5 * * * *`   Every 5 minutes
  `0 * * * *`     Every hour
  `0 9 * * *`     Every day at 9 AM
  `0 22 * * *`    Every day at 10 PM
  `0 9 * * 1-5`   Every weekday at 9 AM
  `0 0 1 * *`     First day of every month
  `0 2 * * 0`     Every Sunday at 2 AM

------------------------------------------------------------------------

# Helper Methods

``` csharp
Cron.Minutely
Cron.Hourly
Cron.Daily
Cron.Weekly
Cron.Monthly
Cron.Yearly
```

------------------------------------------------------------------------

# Create a Recurring Job

``` csharp
RecurringJob.AddOrUpdate<ReportService>(
    "DailyReport",
    x => x.Generate(),
    Cron.Daily);
```

Custom Cron

``` csharp
RecurringJob.AddOrUpdate<ReportService>(
    "DailyReport",
    x => x.Generate(),
    "0 22 * * *");
```

------------------------------------------------------------------------

# Internal Working

``` text
Recurring Job

↓

Cron Expression

↓

Hangfire Checks Schedule

↓

Time Matched

↓

Enqueue Job

↓

Execute Job
```

------------------------------------------------------------------------

# Job Types Comparison

| Job Type  | Runs Once | Runs Repeatedly |
|------------|:---------:|:---------------:|
| Enqueue    | ✅        | ❌              |
| Schedule   | ✅        | ❌              |
| Recurring  | ❌        | ✅              |

------------------------------------------------------------------------

# Best Practices

-   Use `Cron.*` helpers for simple schedules.
-   Store Cron expressions in configuration.
-   Consider UTC and DST for global applications.

------------------------------------------------------------------------

# Interview Questions

## What is Cron?

Cron is a scheduling format used to define when a recurring job should
execute.

------------------------------------------------------------------------

## Difference between Schedule() and RecurringJob?

-   Schedule executes once.
-   RecurringJob executes repeatedly based on a Cron schedule.

------------------------------------------------------------------------

## What does `0 22 * * *` mean?

Run every day at **10:00 PM**.

------------------------------------------------------------------------

# Key Takeaways

-   Recurring jobs repeat automatically.
-   Cron defines the schedule.
-   Learn `*`, `,`, `-`, `/`.
-   Prefer configuration-driven Cron expressions.
