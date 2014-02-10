using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quartz;
using Quartz.Impl;
using Moq;
using System.Web;
using System.Collections.Specialized;
using QuartzAdmin.web.Tests.Fakes;
using QuartzAdmin.web.Controllers;
using QuartzAdmin.web.Models;
using System.Web.Mvc;
using System.Web.Routing;
using Quartz.Job;
using sample.scheduler.core;

namespace QuartzAdmin.web.Tests
{
    /// <summary>
    /// Summary description for ExecuteAJob
    /// </summary>
    [TestClass]
    public class ExecuteAJob
    {
        public ExecuteAJob()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestInitialize()]
        public void TestSetup()
        {
            StartTestScheduler();
        }
        [TestCleanup()]
        public void TestTeardown()
        {
            StopTestScheduler();
        }


        [TestMethod]
        public void Execute_a_job()
        {
            // Arrange
            // - Add a job into the test scheduler
            IScheduler sched = GetTestScheduler();
			IJobDetail job = JobBuilder.Create<NoOpJob>()
				.WithIdentity("TestJob", "TestGroup")
				.Build();			

            sched.AddJob(job, true);
            // - Setup the mock HTTP Request
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            NameValueCollection formParameters = new NameValueCollection();
            // NOTE: adding items to the formParameter collection is possible here
            request.SetupGet(x => x.Form).Returns(formParameters);

            
            // - Create the fake instance repo and the job execution controller
            IInstanceRepository instanceRepo = new FakeInstanceRepository();
            instanceRepo.Save(GetTestInstance());
            JobExecutionController jec = new JobExecutionController(instanceRepo);

            // - Set the fake request for the controller
            jec.ControllerContext = new ControllerContext(context.Object, new RouteData(), jec);

            // Act
            ActionResult result = jec.RunNow("MyTestInstance", "TestGroup", "TestJob");

            //Assert
            Assert.IsTrue(result is ContentResult && ((ContentResult)result).Content == "Job execution started");

        }

        [TestMethod]
        public void Execute_a_job_with_job_data_map()
        {
            // Arrange
            // - Add a job into the test scheduler
            IScheduler sched = GetTestScheduler();
			IJobDetail job = JobBuilder.Create<NoOpJob>()
				.WithIdentity("TestJob2", "TestGroup")
				.UsingJobData("MyParam1", "Initial Data")
				.Build();

            sched.AddJob(job, true);
            // - Setup the mock HTTP Request
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            NameValueCollection formParameters = new NameValueCollection();

            formParameters.Add("jdm_MyParam1", "Working on the railroad");
            request.SetupGet(x => x.Form).Returns(formParameters);


            // - Create the fake instance repo and the job execution controller
            IInstanceRepository instanceRepo = new FakeInstanceRepository();
            instanceRepo.Save(GetTestInstance());
            JobExecutionController jec = new JobExecutionController(instanceRepo);

            // - Set the mocked context for the controller
            jec.ControllerContext = new ControllerContext(context.Object, new RouteData(), jec);

            // Act
            ActionResult result = jec.RunNow("MyTestInstance", "TestGroup", "TestJob2");
            // - Get the triggers of the job
            IList<ITrigger> trigOfJob = sched.GetTriggersOfJob(JobKey.Create("TestJob2", "TestGroup"));

            //Assert
            Assert.IsTrue(result is ContentResult && ((ContentResult)result).Content == "Job execution started");
            Assert.IsTrue(trigOfJob.Count() > 0);
            Assert.AreEqual(trigOfJob[0].JobDataMap["MyParam1"], "Working on the railroad");

        }



        private InstanceModel GetTestInstance()
        {
            InstanceModel instance = new InstanceModel();
            instance.InstanceName = "MyTestInstance";
            instance.InstanceProperties.Add(new InstancePropertyModel() { PropertyName = "quartz.scheduler.instanceName", PropertyValue = "SampleQuartzScheduler" });
            instance.InstanceProperties.Add(new InstancePropertyModel() { PropertyName = "quartz.threadPool.type", PropertyValue = "Quartz.Simpl.SimpleThreadPool, Quartz" });
            instance.InstanceProperties.Add(new InstancePropertyModel() { PropertyName = "quartz.scheduler.proxy", PropertyValue = "true" });
            instance.InstanceProperties.Add(new InstancePropertyModel() { PropertyName = "quartz.scheduler.proxy.address", PropertyValue = "tcp://localhost:567/QuartzScheduler" });
            return instance;

        }

        private void StartTestScheduler()
        {
            IScheduler sched = GetTestScheduler();

            sched.Start();

            // construct job info
			IJobDetail jobDetail = JobBuilder.Create<ConsoleJob1>()
				.WithIdentity("myJob")
				.Build();

			ITrigger trigger = TriggerBuilder.Create()
				.WithIdentity("myTrigger")
				.WithSimpleSchedule(x => x
					.WithIntervalInHours(1))
				.StartAt(DateTimeOffset.UtcNow)
				.Build();
				
            sched.ScheduleJob(jobDetail, trigger);
        }

        private void StopTestScheduler()
        {
            IScheduler sched = GetTestScheduler();
            sched.Shutdown();

        }

        private IScheduler GetTestScheduler()
        {
            NameValueCollection props = new NameValueCollection();

            ISchedulerFactory schedFact = new StdSchedulerFactory(props);

            props.Add("quartz.scheduler.instanceName", "SampleQuartzScheduler");
            props.Add("quartz.scheduler.exporter.type", "Quartz.Simpl.RemotingSchedulerExporter, Quartz");
            props.Add("quartz.scheduler.exporter.port", "567");
            props.Add("quartz.scheduler.exporter.bindName", "QuartzScheduler");
            props.Add("quartz.scheduler.exporter.channelType", "tcp");

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            return sched;
        }

    }
}
