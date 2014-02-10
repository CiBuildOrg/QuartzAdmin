using System.Collections.Generic;
using System.Linq;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using JobKeySet = Quartz.Collection.ISet<Quartz.JobKey>;
using TriggerKeySet = Quartz.Collection.ISet<Quartz.TriggerKey>;

namespace QuartzAdmin.web.Models
{
    [ActiveRecord(Table="tbl_instances")]
    public class InstanceModel : ActiveRecordValidationBase<InstanceModel>
    {
        public InstanceModel()
        {
            InstanceProperties = new HashedSet<InstancePropertyModel>();
        }

		[PrimaryKey("instanceid", Generator = PrimaryKeyType.Sequence, SequenceName = "tbl_instances_instanceid_seq")]
        public virtual int InstanceID { get; set; }

		[Property("instancename"), ValidateNonEmpty]
        public virtual string InstanceName { get; set; }

        [HasMany(typeof(InstancePropertyModel), Table = "tbl_instanceproperties",
                 ColumnKey = "instanceid",
                 Cascade = ManyRelationCascadeEnum.All, Inverse=true)]
        public virtual Iesi.Collections.Generic.ISet<InstancePropertyModel> InstanceProperties { get; set; }

        private IScheduler _CurrentScheduler = null;
        public IScheduler GetQuartzScheduler()
        {
            if (_CurrentScheduler == null)
            {
                System.Collections.Specialized.NameValueCollection props = new System.Collections.Specialized.NameValueCollection();

                foreach (InstancePropertyModel prop in this.InstanceProperties)
                {
                    props.Add(prop.PropertyName, prop.PropertyValue);
                }
                ISchedulerFactory sf = new StdSchedulerFactory(props);
                _CurrentScheduler = sf.GetScheduler();
            }

            return _CurrentScheduler;

        }

        public IQueryable<string> FindAllGroups()
        {
            IScheduler sched = this.GetQuartzScheduler();

            List<string> groups = new List<string>();

			foreach (string jg in sched.GetJobGroupNames())
            {
                groups.Add(jg);
            }

			foreach (string tg in sched.GetTriggerGroupNames())
            {
                if (!groups.Contains(tg))
                {
                    groups.Add(tg);
                }
            }

			return sched.GetJobGroupNames().AsQueryable();
        }

		public List<IJobDetail> GetAllJobs(string groupName)
        {
			List<IJobDetail> jobs = new List<IJobDetail>();
            IScheduler sched = this.GetQuartzScheduler();
			JobKeySet jobNames = sched.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName));

            foreach (JobKey jobName in jobNames)
            {
				jobs.Add(sched.GetJobDetail(jobName));
            }

            return jobs;
        }

		public List<IJobDetail> GetAllJobs()
        {
			List<IJobDetail> jobs = new List<IJobDetail>();
            var groups = FindAllGroups();
            foreach (string group in groups)
            {
				List<IJobDetail> groupJobs = GetAllJobs(group);
                jobs.AddRange(groupJobs);
            }
            return jobs;
        }

		public List<ITrigger> GetAllTriggers(string groupName)
        {
			List<ITrigger> triggers = new List<ITrigger>();
            IScheduler sched = this.GetQuartzScheduler();
            TriggerKeySet triggerNames = sched.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(groupName));

			foreach (TriggerKey triggerName in triggerNames)
            {
				triggers.Add(sched.GetTrigger(triggerName));
            }

            return triggers;
        }

		public List<ITrigger> GetAllTriggers()
        {
			List<ITrigger> triggers = new List<ITrigger>();
            var groups = FindAllGroups();
            foreach (string group in groups)
            {
				List<ITrigger> groupTriggers = GetAllTriggers(group);
                triggers.AddRange(groupTriggers);
            }

            return triggers;
        }



    }
}
