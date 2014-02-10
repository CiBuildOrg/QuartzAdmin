using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Quartz;
using Quartz.Collection;
using Quartz.Impl;
using Quartz.Impl.Calendar;
using Quartz.Impl.Matchers;
using Quartz.Job;
using JobKeySet = Quartz.Collection.ISet<Quartz.JobKey>;

namespace sample.scheduler.core
{
    public class QuartzHost
    {
        public void StartScheduler()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = factory.GetScheduler();

            string myJobName = "MyFirstJob";
            string myGroupName="MyGroup";
			JobKeySet jobNames = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(myGroupName));

            if (!scheduler.CheckExists(JobKey.Create(myJobName, myGroupName)))
            {
				IJobDetail job = JobBuilder.Create<ConsoleJob1>()
					.WithIdentity(myJobName, myGroupName)
					.UsingJobData("ExtraText", "Plinko")
					.Build();

				ITrigger trigger = TriggerBuilder.Create()
				  .WithIdentity("myFirstTrigger", myGroupName)
				  .StartNow()
				  .WithSimpleSchedule(x => x
					.WithIntervalInMinutes(2)
					.RepeatForever())
				  .Build();

                scheduler.ScheduleJob(job, trigger);
            }

            if (!jobNames.Any(k => k.Name == "HelloWorld1"))
            {
				IJobDetail job = JobBuilder.Create<NoOpJob>()
					.WithIdentity("HelloWorld1", myGroupName)
					.Build();

				ITrigger trigger = TriggerBuilder.Create()
				  .WithIdentity("HelloWorld1Trigger", myGroupName)
				  .StartNow()
				  .WithSimpleSchedule(x => x
					.WithIntervalInMinutes(15)
					.RepeatForever())
				  .Build();

				scheduler.ScheduleJob(job, trigger);
            }

            if (!scheduler.CheckExists(JobKey.Create("HelloWorld2", myGroupName)))
            {
				HolidayCalendar calendar = new HolidayCalendar();
				calendar.AddExcludedDate(DateTime.Now.ToUniversalTime());
				calendar.AddExcludedDate(DateTime.Now.AddDays(4).ToUniversalTime());
				scheduler.AddCalendar("randomHolidays", calendar, true, true);

				IJobDetail job = JobBuilder.Create<NoOpJob>()
					.WithIdentity("HelloWorld2", myGroupName)
					.Build();

				ITrigger trigger = TriggerBuilder.Create()
					.WithIdentity("HelloWorld2Trigger", myGroupName)
					.StartNow()
					.WithDailyTimeIntervalSchedule(x => x
						.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(15, 0)))
					.ModifiedByCalendar("randomHolidays")
					.Build();

                scheduler.ScheduleJob(job, trigger);
            }

            if (!scheduler.CheckExists(JobKey.Create("TimeTrackerReminder", myGroupName)))
            {
				IJobDetail job = JobBuilder.Create<NoOpJob>()
					.WithIdentity("TimeTrackerReminder", myGroupName)
					.Build();

				ITrigger trigger = TriggerBuilder.Create()
					.WithIdentity("EveryMondayAtEight", myGroupName)
					.StartNow()
					.WithDailyTimeIntervalSchedule(x => x
						.OnDaysOfTheWeek(DayOfWeek.Monday)
						.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(8, 0)))
					.Build();

                scheduler.ScheduleJob(job, trigger);
            }

			//if (!scheduler.CheckExists(JobKey.Create("UnscheduledJob", myGroupName)))
			//{
			//	IJobDetail job = JobBuilder.Create<NoOpJob>()
			//		.WithIdentity("UnscheduledJob", myGroupName)
			//		.Build();

			//	scheduler.AddJob(job, true);
			//}

            if (!scheduler.CheckExists(JobKey.Create("TwoAliens", myGroupName)))
            {
				IJobDetail job = JobBuilder.Create<TwoAlienJob>()
					.WithIdentity("TwoAliens", myGroupName)
					.Build();

				ITrigger trigger = TriggerBuilder.Create()
					.WithIdentity("EveryFullMoon", myGroupName)
					.StartNow()
					.WithCronSchedule("0 59 23 28 1/1 ? *")
					.Build();

                scheduler.ScheduleJob(job, trigger);
            }


            scheduler.Start();

        }
    }
}
