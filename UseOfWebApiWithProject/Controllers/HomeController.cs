using System;
using System.Collections.Generic;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using UseOfWebApiWithProject.Models;

namespace UseOfWebApiWithProject.Controllers
{
    public class HomeController : Controller
    {
        IEnumerable<Employee> students = null;
        // GET: Home
        public ActionResult Index()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335/Api/");
                //HTTP GET
                var responseTask = client.GetAsync("Employees");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Employee>>();
                    readTask.Wait();

                    students = readTask.Result;
                }
                else
                {
                    students = Enumerable.Empty<Employee>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(students);

        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Employee emp)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44335/Api/Home");
                var postTask = client.PostAsJsonAsync<Employee>("Employees", emp);
                postTask.Wait();
                var Result = postTask.Result;
                if (Result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "some error");
            return View(emp);
        }

    }
}