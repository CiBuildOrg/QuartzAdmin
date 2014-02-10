using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace sample.scheduler.core
{
    public class ConsoleJob1 : IJob
    {
        #region IJob Members

		public void Execute(IJobExecutionContext context)
        {
            string extraText = "[none]";
            if (context.MergedJobDataMap.Contains("ExtraText"))
            {
                extraText = context.MergedJobDataMap["ExtraText"].ToString();
            }
            Console.WriteLine(string.Format("Hello from ConsoleJob1 - {0}\nExtra Text:{1}", DateTime.Now, extraText));
        }

        #endregion
    }
}
