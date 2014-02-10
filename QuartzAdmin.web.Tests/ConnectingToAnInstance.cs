using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quartz;
using Quartz.Impl;
using QuartzAdmin.web.Controllers;
using QuartzAdmin.web.Models;
using QuartzAdmin.web.Tests.Fakes;
using sample.scheduler.core;

namespace QuartzAdmin.web.Tests
{
    /// <summary>
    /// Summary description for ConnectingToAnInstance
    /// </summary>
    [TestClass]
    public class ConnectingToAnInstance
    {
        public ConnectingToAnInstance()
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
        /*
Story: Connecting to an instance

As Operations
I want to connect to a running instance of quartz
So that I can view jobs, schedules, and triggers

Scenario: Verify quartz is running
Given a quartz instance is running
When I connect
Then a valid instance should be returned

Scenario: List all jobs in an instance
Given a quartz instance is running
When I connect and request a list of jobs
Then a list of jobs should be returned

Scenario: List all triggers in an instance
Given a quartz instance is running
When I connect and request a list of triggers
Then a list of triggers should be returned
    */

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
        public void Verify_quartz_is_running()
        {
            // Arrange
            InstanceController controller = GetInstanceController();
            InstanceModel instance = GetTestInstance();
            controller.Repository.Save(instance);

            // Act
            ActionResult result = controller.Connect(instance.InstanceName);
            bool isConnected = false;

            if (result is ViewResult)
            {
                if (((ViewResult)result).ViewData.Model is InstanceViewModel)
                {
                    isConnected = true;
                }
            }

            // Assert
            Assert.IsTrue(isConnected);

        }

        [TestMethod]
        public void List_all_jobs_in_an_instance()
        {
            // Arrange
            InstanceController controller = GetInstanceController();
            InstanceModel instance = GetTestInstance();
            controller.Repository.Save(instance);

            // Act
            ActionResult result = controller.Connect(instance.InstanceName);
            int countOfJobs = 0;

            if (result is ViewResult)
            {
                if (((ViewResult)result).ViewData.Model is InstanceViewModel)
                {
                    countOfJobs = ((InstanceViewModel)((ViewResult)result).ViewData.Model).Jobs.Count;
                }
            }

            // Assert
            Assert.IsTrue(countOfJobs > 0);
        }
        [TestMethod]
        public void List_all_triggers_in_an_instance()
        {
            // Arrange
            InstanceController controller = GetInstanceController();
            InstanceModel instance = GetTestInstance();
            controller.Repository.Save(instance);

            // Act
            ActionResult result = controller.Connect(instance.InstanceName);
            int countOfTriggers = 0;

            if (result is ViewResult)
            {
                if (((ViewResult)result).ViewData.Model is InstanceViewModel)
                {
                    countOfTriggers = ((InstanceViewModel)((ViewResult)result).ViewData.Model).Triggers.Count;
                }
            }

            // Assert
            Assert.IsTrue(countOfTriggers > 0);
        }

        private InstanceController GetInstanceController()
        {
            InstanceController cont = new InstanceController(new FakeInstanceRepository());

            return cont;

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
