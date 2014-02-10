using System.Web.Mvc;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Controllers
{
    public class TriggerController : Controller
    {
        InstanceRepository instanceRepo = new InstanceRepository();

        //
        // GET: /Trigger/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string instanceName, string groupName, string itemName)
        {
            InstanceModel instance = instanceRepo.GetInstance(instanceName);
            TriggerRepository trigRepo = new TriggerRepository(instance);

            TriggerFireTimesModel m = new TriggerFireTimesModel();
            m.Trigger = trigRepo.GetTrigger(itemName, groupName);

            CalendarRepository calRepo = new CalendarRepository(instance);
			if(!string.IsNullOrWhiteSpace(m.Trigger.CalendarName))
				m.Calendar = calRepo.GetCalendar(m.Trigger.CalendarName);
            
			m.Instance = instance;

            ViewData["groupName"] = groupName;

            if (m.Trigger == null)
            {
                ViewData["triggerName"] = itemName;
                return View("NotFound");
            }
            else
            {
                return View(m);
            }
        }

        public ActionResult FireTimes(string instanceName, string groupName, string itemName)
        {
            InstanceModel instance = instanceRepo.GetInstance(instanceName);
            TriggerRepository trigRepo = new TriggerRepository(instance);

            TriggerFireTimesModel m = new TriggerFireTimesModel();
            m.Trigger = trigRepo.GetTrigger(itemName, groupName);

            CalendarRepository calRepo = new CalendarRepository(instance);
            m.Calendar = calRepo.GetCalendar(m.Trigger.CalendarName);

            ViewData["groupName"] = groupName;

            if (m.Trigger == null)
            {
                ViewData["triggerName"] = itemName;
                return View("NotFound");
            }
            else
            {
                return View(m);
            }

        }

    }
}
