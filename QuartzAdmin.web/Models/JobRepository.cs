using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;

namespace QuartzAdmin.web.Models
{
	public class JobRepository
	{
		private InstanceModel quartzInstance;
		public JobRepository(string instanceName)
		{
			InstanceRepository repo = new InstanceRepository();
			quartzInstance = repo.GetInstance(instanceName);
		}

		public JobRepository(InstanceModel instance)
		{
			quartzInstance = instance;
		}


		public IJobDetail GetJob(string jobName, string groupName)
		{
			IScheduler sched = quartzInstance.GetQuartzScheduler();

			return sched.GetJobDetail(JobKey.Create(jobName, groupName));

		}

		public void RunJobNow(string jobName, string groupName)
		{
			IScheduler sched = quartzInstance.GetQuartzScheduler();
			sched.TriggerJob(JobKey.Create(jobName, groupName));
		}
		public void RunJobNow(string jobName, string groupName, JobDataMap jdm)
		{

			IScheduler sched = quartzInstance.GetQuartzScheduler();
			sched.TriggerJob(JobKey.Create(jobName, groupName), jdm);
		}

		public void DeleteJob(string jobName, string groupName)
		{
			IScheduler sched = quartzInstance.GetQuartzScheduler();
			sched.DeleteJob(JobKey.Create(jobName, groupName));
		}

	}
}
