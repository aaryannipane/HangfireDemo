# Hangfire Course Notes

# Module 3 - Lesson 8

# Advanced Cron Expressions

------------------------------------------------------------------------

# Objective

Learn to build custom Cron expressions for real production schedules.

------------------------------------------------------------------------

# Cron Refresher

``` text
* * * * *
│ │ │ │ │
│ │ │ │ └── Day of Week
│ │ │ └──── Month
│ │ └────── Day of Month
│ └──────── Hour
└────────── Minute
```

------------------------------------------------------------------------

# Special Characters

| Symbol | Meaning         | Example     | Result                 |
|:------:|-----------------|-------------|------------------------|
| `*`    | Every value     | `* * * * *` | Every minute           |
| `,`    | Multiple values | `9,18`      | 9 AM & 6 PM            |
| `-`    | Range           | `9-17`      | 9 AM through 5 PM      |
| `/`    | Step interval   | `*/5`       | Every 5 minutes        |

------------------------------------------------------------------------

# Common Production Expressions

| Cron                  | Meaning                                     |
|-----------------------|---------------------------------------------|
| `*/5 * * * *`         | Every 5 minutes                             |
| `*/15 * * * *`        | Every 15 minutes                            |
| `0 */2 * * *`         | Every 2 hours                               |
| `0 9 * * 1-5`         | Weekdays at 9 AM                            |
| `0 22 * * *`          | Daily at 10 PM                              |
| `0 2 * * 0`           | Sunday at 2 AM                              |
| `0 0 1 * *`           | First day of every month                    |
| `*/10 9-17 * * 1-5`   | Every 10 minutes during office hours (9–5)  |

------------------------------------------------------------------------

# Example

``` csharp
RecurringJob.AddOrUpdate<CacheService>(
    "RefreshCache",
    x => x.Refresh(),
    "*/5 * * * *");
```

------------------------------------------------------------------------

# Store Cron in Configuration

``` json
{
  "Jobs": {
    "DailyReportCron": "0 22 * * *"
  }
}
```

``` csharp
RecurringJob.AddOrUpdate<ReportService>(
    "DailyReport",
    x => x.Generate(),
    configuration["Jobs:DailyReportCron"]);
```

Benefits:

-   No code changes
-   No redeployment
-   Easy business updates

------------------------------------------------------------------------

# Best Practices

-   Use `Cron.*` helpers for simple schedules.
-   Store custom Cron values in configuration.
-   Consider server time zone and DST.
-   Test Cron expressions before production.

------------------------------------------------------------------------

# Interview Questions

## Difference between `*` and `*/5`?

`*` means every value. `*/5` means every fifth value.

## What does `0 9-17 * * 1-5` mean?

Every hour (minute 0), from 9 AM to 5 PM, Monday through Friday.

## Why store Cron in configuration?

To allow schedule changes without rebuilding or redeploying the
application.

------------------------------------------------------------------------

# Key Takeaways

-   Learn `*`, `,`, `-`, `/`.
-   Prefer configuration-driven schedules.
-   Understand production scheduling scenarios.
