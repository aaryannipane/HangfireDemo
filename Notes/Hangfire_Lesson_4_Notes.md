# Hangfire Course Notes

# Module 2 - Lesson 4

# Installing & Configuring Hangfire

------------------------------------------------------------------------

# Objective

Learn how to install and configure Hangfire in an ASP.NET Core
application and understand what each configuration line does.

------------------------------------------------------------------------

# Prerequisites

-   ASP.NET Core Web API / MVC
-   SQL Server
-   Visual Studio
-   Valid connection string

------------------------------------------------------------------------

# Required NuGet Packages

  Package               Purpose
  --------------------- ----------------------------------------
  Hangfire.AspNetCore   Integrates Hangfire with ASP.NET Core.
  Hangfire.SqlServer    Uses SQL Server as Hangfire storage.

Install:

``` powershell
Install-Package Hangfire.AspNetCore
Install-Package Hangfire.SqlServer
```

------------------------------------------------------------------------

# Step 1 - Configure Hangfire

``` csharp
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
```

### What does `AddHangfire()` do?

-   Registers Hangfire services.
-   Configures storage.
-   Registers Hangfire client.

Without it, Hangfire cannot be used.

------------------------------------------------------------------------

# Step 2 - Configure SQL Server Storage

``` csharp
config.UseSqlServerStorage(
    builder.Configuration.GetConnectionString("DefaultConnection"));
```

Purpose:

-   Stores jobs
-   Stores queues
-   Stores retries
-   Stores job history
-   Stores server information

Storage providers:

-   SQL Server
-   PostgreSQL
-   Redis
-   MySQL

------------------------------------------------------------------------

# Step 3 - Start Hangfire Server

``` csharp
builder.Services.AddHangfireServer();
```

Purpose:

-   Starts worker threads.
-   Polls storage.
-   Executes queued jobs.
-   Updates job status.

Without this line:

-   Jobs are created ✅
-   Jobs are stored ✅
-   Jobs are NEVER executed ❌

------------------------------------------------------------------------

# Step 4 - Enable Dashboard

``` csharp
app.UseHangfireDashboard();
```

Default URL

``` text
https://localhost:<port>/hangfire
```

Custom URL

``` csharp
app.UseHangfireDashboard("/jobs");
```

Available at:

``` text
https://localhost:<port>/jobs
```

Dashboard Features

-   Succeeded Jobs
-   Failed Jobs
-   Processing Jobs
-   Scheduled Jobs
-   Recurring Jobs
-   Retries
-   Servers

------------------------------------------------------------------------

# Step 5 - Connection String

Example:

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HangfireDemo;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

### Local SQL Server SSL Issue

Newer versions of `Microsoft.Data.SqlClient` enable encryption by
default.

If you receive:

> The certificate chain was issued by an authority that is not trusted.

Use:

``` text
TrustServerCertificate=True
```

or for local development:

``` text
Encrypt=False
```

Do **not** rely on these settings for production unless appropriate for
your environment.

------------------------------------------------------------------------

# Complete Program.cs

``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard();

app.MapControllers();

app.Run();
```

------------------------------------------------------------------------

# Internal Startup Flow

``` text
Application Starts

↓

Read Connection String

↓

Configure Hangfire

↓

Connect SQL Server

↓

Start Hangfire Server

↓

Enable Dashboard

↓

Wait For Jobs
```

------------------------------------------------------------------------

# What Happens on First Run?

Hangfire automatically creates required tables.

Examples:

-   Job
-   State
-   JobQueue
-   Server
-   Hash
-   Set
-   Counter

No manual SQL script is required.

------------------------------------------------------------------------

# Why Do We Need AddHangfireServer()?

Architecture

``` text
Application

↓

Hangfire Client

↓

Storage

↓

Hangfire Server

↓

Execute Job
```

Without the Hangfire Server, there is no component available to execute
jobs.

------------------------------------------------------------------------

# Common Mistakes

  Mistake                                Result
  -------------------------------------- -------------------------------
  Missing `Hangfire.SqlServer` package   Storage configuration fails.
  Forgetting `AddHangfireServer()`       Jobs stay Enqueued forever.
  Forgetting `UseHangfireDashboard()`    Dashboard unavailable.
  Invalid connection string              Cannot connect to SQL Server.
  Wrong database                         Dashboard shows no jobs.

------------------------------------------------------------------------

# Best Practices

-   Use SQL Server or Redis for production storage.
-   Keep connection strings in configuration.
-   Protect the Dashboard in production.
-   Monitor failed jobs regularly.
-   Do not expose the Dashboard publicly without authentication.

------------------------------------------------------------------------

# Interview Questions

## 1. Which NuGet packages are required?

-   Hangfire.AspNetCore
-   Hangfire.SqlServer

------------------------------------------------------------------------

## 2. What does `AddHangfire()` do?

Registers Hangfire services and configures storage.

------------------------------------------------------------------------

## 3. What does `AddHangfireServer()` do?

Starts the Hangfire worker process that polls storage and executes jobs.

------------------------------------------------------------------------

## 4. What happens if `AddHangfireServer()` is removed?

Jobs are stored successfully but remain in the **Enqueued** state
because no worker is available.

------------------------------------------------------------------------

## 5. What is the Dashboard used for?

Monitoring and managing jobs. It does not execute jobs.

------------------------------------------------------------------------

## 6. Why do we need SQL Server?

To persist jobs, queues, retries, states, and metadata so work survives
application restarts.

------------------------------------------------------------------------

# Quick Revision

  API                      Purpose
  ------------------------ -----------------------------------------
  AddHangfire()            Configure Hangfire services and storage
  UseSqlServerStorage()    Persist jobs in SQL Server
  AddHangfireServer()      Start background workers
  UseHangfireDashboard()   Enable monitoring UI

------------------------------------------------------------------------

# Key Takeaways

-   Install `Hangfire.AspNetCore` and `Hangfire.SqlServer`.
-   Configure persistent storage.
-   `AddHangfireServer()` is mandatory for job execution.
-   Dashboard is for monitoring, not execution.
-   Hangfire creates its SQL tables automatically.
-   Use `TrustServerCertificate=True` for common local SQL certificate
    issues.
