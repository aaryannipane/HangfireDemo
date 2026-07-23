# Hangfire Course Notes

# Module 3 - Lesson 9

# Continuation Jobs (`BackgroundJob.ContinueJobWith()`)

------------------------------------------------------------------------

# Objective

Learn how to execute a job only after another job completes
successfully.

------------------------------------------------------------------------

# What is a Continuation Job?

A continuation job depends on another job.

``` text
Payment
   │
   ▼
Invoice
   │
   ▼
Email
   │
   ▼
SMS
```

Each job starts only after the previous one succeeds.

------------------------------------------------------------------------

# Syntax

``` csharp
var paymentJob = BackgroundJob.Enqueue<OrderService>(
    x => x.ProcessPayment());

BackgroundJob.ContinueJobWith<OrderService>(
    paymentJob,
    x => x.GenerateInvoice());
```

------------------------------------------------------------------------

# Multiple Continuations

``` csharp
var payment = BackgroundJob.Enqueue<OrderService>(x => x.ProcessPayment());

var invoice = BackgroundJob.ContinueJobWith<OrderService>(
    payment,
    x => x.GenerateInvoice());

var email = BackgroundJob.ContinueJobWith<NotificationService>(
    invoice,
    x => x.SendEmail());

BackgroundJob.ContinueJobWith<NotificationService>(
    email,
    x => x.SendSms());
```

------------------------------------------------------------------------

# Why Not Just Enqueue Everything?

❌ Wrong

``` csharp
BackgroundJob.Enqueue(() => Payment());
BackgroundJob.Enqueue(() => Invoice());
BackgroundJob.Enqueue(() => Email());
```

Execution order is **not guaranteed**.

✅ Correct

Use continuation jobs to enforce dependencies.

------------------------------------------------------------------------

# Internal Flow

``` text
Create Parent Job

↓

Create Child Job

↓

Child waits

↓

Parent Succeeds

↓

Child Enqueued

↓

Child Executes
```

------------------------------------------------------------------------

# What if Parent Fails?

``` text
Payment

↓

Failed

↓

Invoice NOT Executed

↓

Email NOT Executed
```

Continuation jobs do not run until the parent completes successfully.

------------------------------------------------------------------------

# Real World Examples

-   Payment → Invoice → Email
-   Generate PDF → Upload → Notify User
-   Order → Warehouse → Shipping
-   Loan Approval → Documents → Email

------------------------------------------------------------------------

# Best Practices

-   Keep each job small.
-   Use continuation jobs for workflows.
-   Don't mix workflow orchestration with business logic.
-   Avoid extremely long job chains.

------------------------------------------------------------------------

# Interview Questions

## What is a continuation job?

A background job that executes only after another background job
succeeds.

## Why use continuation jobs instead of multiple Enqueue() calls?

Because they guarantee execution order for dependent tasks.

## What happens if the parent job fails?

The continuation job is not executed.

## Why not enqueue the next job inside the current method?

It tightly couples business logic with workflow orchestration, reducing
reusability and making testing harder.

------------------------------------------------------------------------

# Key Takeaways

-   Use `ContinueJobWith()` for dependent workflows.
-   Execution order is guaranteed.
-   Parent failure prevents child execution.
-   Prefer orchestration outside business logic.
