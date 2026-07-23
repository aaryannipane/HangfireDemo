# Hangfire Course Notes

# Module 4 - Lesson 11

# Automatic Retries in Hangfire

------------------------------------------------------------------------

# Objective

Learn how Hangfire automatically retries failed jobs, when retries
should be used, how to configure them, and how to build safe retryable
jobs.

------------------------------------------------------------------------

# Learning Objectives

-   Why retries are important
-   Default retry behavior
-   Retry lifecycle
-   Configure retry attempts
-   Disable retries
-   Idempotency
-   Production best practices

------------------------------------------------------------------------

# Why Do We Need Retries?

Many background jobs fail because of temporary (transient) failures.

Examples:

-   SMTP server unavailable
-   Database timeout
-   HTTP 503
-   HTTP 429 (Rate Limit)
-   Temporary network failure

Without retries:

``` text
Job
↓
Failed
↓
Done
```

With retries:

``` text
Job
↓
Failed
↓
Retry
↓
Retry
↓
Succeeded
```

------------------------------------------------------------------------

# Default Retry Behavior

By default Hangfire retries failed jobs **10 times**.

Lifecycle:

``` text
Created
↓
Enqueued
↓
Processing
↓
Exception
↓
Scheduled Retry
↓
Enqueued
↓
Processing
↓
Succeeded / Failed
```

------------------------------------------------------------------------

# Configure Retry Count

``` csharp
using Hangfire;

public class EmailService
{
    [AutomaticRetry(Attempts = 3)]
    public void SendEmail()
    {
    }
}
```

------------------------------------------------------------------------

# Disable Retries

``` csharp
[AutomaticRetry(Attempts = 0)]
public void DeleteUser()
{
}
```

Use when retrying will never succeed (validation errors, invalid email,
etc.).

------------------------------------------------------------------------

# Retry Delay

Hangfire retries using increasing delays instead of retrying
immediately.

``` text
Attempt 1
↓
Wait
↓
Attempt 2
↓
Wait Longer
↓
Attempt 3
```

------------------------------------------------------------------------

# Idempotency

An operation is **idempotent** if executing it multiple times produces
the same final result.

❌ Bad

``` csharp
balance += 1000;
```

✅ Good

``` csharp
if(order.Status != OrderStatus.Completed)
{
    order.Status = OrderStatus.Completed;
}
```

------------------------------------------------------------------------

# When Should You Retry?

| Scenario               | Retry | Reason               |
|------------------------|:-----:|----------------------|
| SMTP Failure           | ✅    | Temporary failure    |
| Database Timeout       | ✅    | Temporary failure    |
| HTTP 503               | ✅    | Service unavailable  |
| HTTP 429               | ✅    | Rate limiting        |
| Invalid Email Address  | ❌    | Permanent error      |
| Validation Error       | ❌    | Business validation  |
| NullReferenceException | ❌    | Application bug      |

------------------------------------------------------------------------

# Why Not Write Your Own Retry Loop?

``` csharp
for(int i = 0; i < 3; i++)
{
    try
    {
        SendEmail();
        break;
    }
    catch
    {
    }
}
```

Problems:

-   No persistence
-   No monitoring
-   No dashboard
-   Hard to maintain

------------------------------------------------------------------------

# Best Practices

-   Retry only transient failures.
-   Make jobs idempotent.
-   Don't swallow exceptions.
-   Log failures.
-   Use transaction IDs for payment jobs.

------------------------------------------------------------------------

# Common Mistakes

  Mistake                            Better Approach
  -------------------------- -------------------------------
  Retrying every exception    Retry only transient failures
  Swallowing exceptions       Let Hangfire detect failures
  Non-idempotent jobs             Make jobs repeat-safe
  Infinite retry loops          Configure retry attempts

------------------------------------------------------------------------

# Interview Questions

## Why does Hangfire retry jobs?

To automatically recover from temporary failures without manual
intervention.

------------------------------------------------------------------------

## What is the default retry count?

10 attempts.

------------------------------------------------------------------------

## How do you configure retries?

``` csharp
[AutomaticRetry(Attempts = 3)]
```

------------------------------------------------------------------------

## How do you disable retries?

``` csharp
[AutomaticRetry(Attempts = 0)]
```

------------------------------------------------------------------------

## What is idempotency?

An operation that produces the same final result even if executed
multiple times.

------------------------------------------------------------------------

## Should payment jobs be retried?

Yes, but only with idempotency keys or transaction IDs to prevent
duplicate charges.

------------------------------------------------------------------------

# Quick Revision

| API / Concept        | Purpose                           |
|----------------------|-----------------------------------|
| `[AutomaticRetry]`   | Configure retry behavior          |
| `Attempts = 3`       | Retry three times                 |
| `Attempts = 0`       | Disable retries                   |
| Idempotency          | Safe repeated execution           |
| Transient Failure    | Temporary error suitable for retry|

------------------------------------------------------------------------

# Key Takeaways

-   Default retry count is 10.
-   Retry only transient failures.
-   Make jobs idempotent.
-   Don't swallow exceptions.
-   Configure retries based on business requirements.
