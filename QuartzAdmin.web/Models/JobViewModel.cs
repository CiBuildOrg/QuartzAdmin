﻿using System.Collections.Generic;
using Quartz;

namespace QuartzAdmin.web.Models
{
    public class JobViewModel
    {
        public IJobDetail JobDetail { get; set; }
		public IList<ITrigger> Triggers { get; set; }
    }
}
