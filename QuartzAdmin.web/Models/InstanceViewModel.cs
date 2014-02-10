using System.Collections.Generic;
using Quartz;

namespace QuartzAdmin.web.Models
{
    public class InstanceViewModel
    {
        public InstanceModel Instance { get; set; }
        public List<IJobDetail> Jobs { get; set; }
        public List<ITrigger> Triggers { get; set; }
    }
}
