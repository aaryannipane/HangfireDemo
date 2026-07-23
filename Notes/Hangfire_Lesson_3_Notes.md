# Hangfire Course Notes

# Module 2 - Lesson 3

# Hangfire Architecture

------------------------------------------------------------------------

# Objective

Understand how Hangfire works internally and how a job moves from your
application to execution.

------------------------------------------------------------------------

# Hangfire Architecture

``` text
                ASP.NET Core Application
+---------------------------------------------+
| Controller                                  |
|      |                                      |
|      v                                      |
| BackgroundJob.Enqueue()                     |
+------------------|--------------------------+
                   |
                   v
          Hangfire Client
                   |
                   v
         SQL Server Storage
         -------------------
         Jobs
         States
         Queues
         Servers
                   ^
                   |
          Hangfire Server
             (Workers)
                   |
                   v
         Execute Your Method
```

------------------------------------------------------------------------

# Core Components

  -----------------------------------------------------------------------
  Component                    Responsibility
  ---------------------------- ------------------------------------------
  Application                  Creates background jobs.

  Hangfire Client              Serializes the method call and stores the
                               job.

  Storage (SQL                 Persists jobs, states, retries, queues,
  Server/Redis/etc.)           and metadata.

  Hangfire Server              Polls storage, executes jobs, updates
                               status.

  Dashboard                    Monitors and manages jobs. It does **not**
                               execute jobs.
  -----------------------------------------------------------------------

------------------------------------------------------------------------

# Component 1 - Application

Your ASP.NET Core application only **creates** the job.

``` csharp
BackgroundJob.Enqueue(() => SendEmail());
```

It does **not** execute the method.

------------------------------------------------------------------------

# Component 2 - Hangfire Client

Responsibilities:

-   Creates the job
-   Serializes method information
-   Stores job in storage

Example stored information:

``` text
Class  : EmailService
Method : SendWelcomeEmail
Args   : user@example.com
State  : Enqueued
```

------------------------------------------------------------------------

# Component 3 - Storage

Supported storage providers:

-   SQL Server
-   PostgreSQL
-   Redis
-   MySQL

Storage contains:

-   Jobs
-   Queues
-   States
-   Retry information
-   Server information

Think of storage as a shared **to-do list**.

------------------------------------------------------------------------

# Component 4 - Hangfire Server

The Hangfire Server continuously checks storage.

``` text
while(true)
{
    Check Storage

    Job Found?

        Yes

        Execute Job
}
```

Responsibilities:

-   Poll jobs
-   Execute jobs
-   Update status
-   Retry failed jobs

Without the Hangfire Server, jobs remain in the queue.

------------------------------------------------------------------------

# Component 5 - Dashboard

Dashboard is only for:

-   Monitoring
-   Retrying failed jobs
-   Viewing history
-   Deleting jobs

It **does not execute jobs**.

------------------------------------------------------------------------

# Complete Job Lifecycle

``` text
User

↓

Controller

↓

BackgroundJob.Enqueue()

↓

Hangfire Client

↓

SQL Server

(Job Stored)

↓

HTTP Response Returned

----------------------------

Hangfire Server

↓

Read Job

↓

Execute Method

↓

Succeeded / Failed
```

------------------------------------------------------------------------

# What Happens After Application Restart?

``` text
Job Stored

↓

Application Stops

↓

Application Starts

↓

Hangfire Server Starts

↓

Pending Job Found

↓

Execute Job
```

Jobs are not lost because they are stored persistently.

------------------------------------------------------------------------

# Multiple Servers

``` text
              SQL Server
                   |
      --------------------------
      |           |            |
      v           v            v
  Server A    Server B    Server C
 Hangfire     Hangfire    Hangfire
  Server       Server      Server
```

Any Hangfire Server can process the next available job.

------------------------------------------------------------------------

# Restaurant Analogy

  Restaurant            Hangfire
  --------------------- ------------------
  Customer              Your Application
  Order                 Background Job
  Kitchen Ticket        Storage
  Chef                  Hangfire Server
  Order Display Board   Dashboard

------------------------------------------------------------------------

# Internal Flow

``` text
Application

↓

Hangfire Client

↓

Storage

↓

Hangfire Server

↓

Execute Method

↓

Update Job State
```

------------------------------------------------------------------------

# Why Not Execute Directly From Controller?

If the controller executes the method:

-   User waits.
-   Slow response.
-   Request timeout risk.

Instead:

-   Controller creates the job.
-   Hangfire executes it later.

------------------------------------------------------------------------

# Advantages

-   Reliable execution
-   Persistent storage
-   Automatic recovery after restart
-   Multi-server support
-   Monitoring through Dashboard

------------------------------------------------------------------------

# Disadvantages

-   Requires storage (SQL Server, Redis, etc.)
-   Requires Hangfire Server to be running

------------------------------------------------------------------------

# Interview Questions

## 1. What are the main components of Hangfire?

-   Application
-   Hangfire Client
-   Storage
-   Hangfire Server
-   Dashboard

------------------------------------------------------------------------

## 2. Who executes the job?

**Answer:** The Hangfire Server (worker process).

------------------------------------------------------------------------

## 3. Does `BackgroundJob.Enqueue()` execute the method immediately?

**Answer:** No. It creates and stores the job. The Hangfire Server
executes it later.

------------------------------------------------------------------------

## 4. What is the role of SQL Server?

**Answer:** It stores jobs, queues, states, retries, and metadata.

------------------------------------------------------------------------

## 5. Does the Dashboard execute jobs?

**Answer:** No. It only displays and manages jobs.

------------------------------------------------------------------------

# Common Mistakes

-   Thinking `Enqueue()` immediately executes the method.
-   Thinking SQL Server executes jobs.
-   Thinking Dashboard executes jobs.
-   Forgetting that `AddHangfireServer()` is required.

------------------------------------------------------------------------

# Key Takeaways

-   Application creates jobs.
-   Hangfire Client stores jobs.
-   Storage persists jobs.
-   Hangfire Server executes jobs.
-   Dashboard only monitors jobs.
-   Jobs survive application restarts.
-   Multiple Hangfire Servers can share the same storage.
