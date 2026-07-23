using Hangfire;
using Hangfire.Common;
using HangfireDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HangfireDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly EmailService _emailService;

        public UserController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("register")]
        public IActionResult Register()
        {
            #region Enqueue - Fire and Forget Job
            //BackgroundJob.Enqueue(() => _emailService.SendWelcomeEmail("a@g.com"));
            #endregion Enqueue - Fire and Forget Job

            #region Schedule - Schedule a job to execute at certain time (delayed jobs)
            //BackgroundJob.Schedule(() => _emailService.SendWelcomeEmail("a@g.com"), TimeSpan.FromMinutes(1));
            #endregion Schedule - Schedule a job to execute at certain time (delayed jobs)

            #region RecurringJob - AddOrUpdate Job - recurring jobs
            //A recurring job is a background job that executes repeatedly according to a predefined schedule, usually specified using a Cron expression.
            //RecurringJob.AddOrUpdate<EmailService>("Send Email", x => x.SendWelcomeEmail("a@g.com"), Cron.Minutely);
            #endregion RecurringJob - AddOrUpdate Job - recurring jobs

            #region Cron Examples
            //Minute → Hour → Day → Month → Week
            //* * * * *
            //│ │ │ │ │
            //│ │ │ │ └── Day of Week(0 - 6)
            //│ │ │ └──── Month(1 - 12)
            //│ │ └────── Day of Month(1 - 31)
            //│ └──────── Hour(0 - 23)
            //└────────── Minute(0 - 59)

            /*
            CRON Format:
            * * * * *
            | | | | |
            | | | | +-- Day of Week (0-6) (Sun=0)
            | | | +---- Month (1-12)
            | | +------ Day of Month (1-31)
            | +-------- Hour (0-23)
            +---------- Minute (0-59)
            */

            // CRON Format: Minute Hour Day Month DayOfWeek
            // * = Every value
            // , = Multiple values (9,18 => 9 AM & 6 PM)
            // - = Range (9-17 => 9 AM to 5 PM)
            // / = Step interval (*/5 => Every 5 minutes, */ 2 => Every 2 hours)

            // * * * * *       => Every minute
            // */5 * * * *     => Every 5 minutes
            // 0 * * * *       => Every hour
            // 0 9 * * *       => Every day at 9:00 AM
            // 0 22 * * *      => Every day at 10:00 PM
            // 0 9 * * 1-5     => Every weekday at 9:00 AM
            // 0 0 1 * *       => First day of every month at midnight
            #endregion Cron Examples

            #region ContinueJobWith - run a job after completion of another job
            string welcomeEmail1Job = BackgroundJob.Enqueue<EmailService>((x) => x.SendWelcomeEmail("a@g.com"));

            string welcomeEmail2Job = BackgroundJob.ContinueJobWith<EmailService>(welcomeEmail1Job, x => x.SendWelcomeEmail("b@g.com"));
            string welcomeEmail3Job = BackgroundJob.ContinueJobWith<EmailService>(welcomeEmail2Job, x => x.SendWelcomeEmail("c@g.com"));
            #endregion

            return Ok("User Registered");
        }
    }
}
