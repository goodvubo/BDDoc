using BDdoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BDdoc.Controllers
{
    public class HomeController : Controller
    {
        private bddEntities db = new bddEntities();
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string Index(string SearchString = "")
        {

                          
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(Json(
                from p in db.Doctors
                where (p.Name.ToLower().Contains(SearchString.ToLower()) || p.Speciality.ToLower().Contains(SearchString.ToLower()))
                select new { key = p.Name, value = p.Name }
                ));

            output = output.Replace("{", "[").Replace("}", "]").Replace(":", ",");
            return output;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}