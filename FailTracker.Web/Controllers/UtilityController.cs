using System.Web.Mvc;
using Microsoft.Web.Mvc;
using FailTracker.Core.Data;
using FailTracker.Core.Domain;
using NHibernate.Linq;

namespace FailTracker.Web.Controllers
{
	public class UtilityController : Controller
	{
		public ActionResult Layout()
		{
			return View();
		}

		[HttpGet]
		public ActionResult ResetDatabase()
		{
			return View();
		}

		[HttpPost]
		public ActionResult ResetDatabase(FormCollection form)
		{
			NHibernateBootstrapper.CreateSchema();
			using (var session = NHibernateBootstrapper.GetSession())
			{
				var users = new[]
				            	{
				            		Core.Domain.User.CreateNewUser("admin@failtracker.com", "admin"),
				            		Core.Domain.User.CreateNewUser("user@failtracker.com", "user")
				            	};
				users.ForEach(u => session.Save(u));
				
				(new[] {
				 		Issue.CreateNewStory("Project support", users[0], "As someone who manages many software projects, I want to be able to organize issues and bugs into projects within Fail Tracker.")
								.ReassignTo(users[0])
								.SetSizeTo(PointSize.Eight),
				 		Issue.CreateNewBug("Site rendering problems in IE6", users[1], "The site does not render the same in al versions of IE!")
								.SetSizeTo(PointSize.OneHundred)
								.ReassignTo(users[1]),
				 		Issue.CreateNewStory("Enable user invite", users[0], "I want to be able to invite users to join Fail Tracker through a form on the site.")
								.ReassignTo(users[0])
								.SetSizeTo(PointSize.Five),
				 		Issue.CreateNewStory("Support unassigned stories", users[0], "I want to be able to leave stories and bugs unassigned.")
								.SetSizeTo(PointSize.Five),
				 	}).ForEach(i => session.Save(i));

				session.Flush();
			}

			return this.RedirectToAction<IssuesController>(c => c.Dashboard());
		}
	}
}