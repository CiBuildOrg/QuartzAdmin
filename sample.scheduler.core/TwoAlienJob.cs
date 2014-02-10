using System;
using Quartz;

namespace sample.scheduler.core
{
	[DisallowConcurrentExecution]
	public class TwoAlienJob : IJob
    {
        #region IJob Members

        public void Execute(IJobExecutionContext context)
        {
            string alienText = @"
            .-''''-.        .-''''-.
           /        \      /        \
          /_        _\    /_        _\
         // \      / \\  // \      / \\
         |\__\    /__/|  |\__\    /__/|
          \    ||    /    \    ||    /
           \        /      \        /
            \  __  /        \  __  /
             '.__.'          '.__.'
              |  |            |  |
              |  |            |  |
";

            Console.WriteLine(alienText);
        }

        #endregion
    }
}
