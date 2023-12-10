using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BDdoc.Models;
using System.Threading.Tasks;
using PagedList;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;

namespace BDdoc.Controllers
{
    public class DoctorsController : Controller
    {
        private bddEntities db = new bddEntities();

        // GET: Doctors
        public ActionResult Index(string id, string day, string time, int docFee = 0, string SearchString = "", string sortOrder = "", string currentFilter = "", int? page = 1)
        {
            var doctors = from m in db.Doctors
                          select m;



            if (!string.IsNullOrWhiteSpace(id))
            {
                doctors = db.Doctors.Where(p => p.Speciality.Contains(id));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(SearchString))
                {

                    doctors = db.Doctors.Where(p => p.Name.Contains(SearchString));

                    if (!doctors.Any())
                    {
                        doctors = db.Doctors.Where(p => p.Speciality.Contains(SearchString));
                    }

                    if (!doctors.Any())
                    {
                        doctors = db.Doctors.Where(p => p.Location.Contains(SearchString));
                    }
                }
            }

            ViewBag.CurrentSort = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["FeeSortParm"] = sortOrder == "Fee" ? "fee_desc" : "Fee";

            if (SearchString == "")
            {
                SearchString = currentFilter;
            }

            ViewBag.CurrentFilter = SearchString;

            switch (sortOrder)
            {
                case "name_desc":
                    doctors = doctors.OrderByDescending(s => s.Name);
                    break;
                case "Fee":
                    doctors = doctors.OrderBy(s => s.Fee);
                    break;
                case "fee_desc":
                    doctors = doctors.OrderByDescending(s => s.Fee);
                    break;
                default:
                    doctors = doctors.OrderBy(s => s.Name);
                    break;
            }

            //Note : you can bind same list from database


            //var doc = new DocList();

            //movieGenreVM.genres = new SelectList(await genreQuery.Distinct().ToListAsync());


            var doc = doctors.AsNoTracking().ToList();

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(doc.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }

        // GET: Doctors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            var feedz = db.Feedbacks.Where(p => p.ID == id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            DocList docz = new DocList();
            docz.doctor = doctor;
            docz.likes = (feedz.Count() == 0)?0:(feedz.Where(p => p.Popularity == "1").Count() * 100) / feedz.Count();
            docz.feeds = feedz.ToList();
            return View(docz);
        }

        //availability
        [AcceptVerbs(HttpVerbs.Post)]
        public string Availability(string apptm, int id)
        {
            int outz = (from p in db.Appointments
                         where (p.ID == id && p.Time == apptm) select p).Count();
            if(outz > 0)
            {
                return "0";
            }
            DateTime dtCurr = DateTime.Now;
            DateTime inputDate = DateTime.ParseExact(apptm, "dd/MM/yyyy - hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            if (DateTime.Compare(dtCurr, inputDate) > 0)
            {
                return "0";
            }
            return "1";
        }

        private void SendEmail(string address, string subject, string message)
        {
            string email = "bddoctors2017@gmail.com";
            string password = "iisexpress";

            var loginInfo = new NetworkCredential("bddoctors2017@gmail.com", password);
            var msg = new MailMessage();
            var smtpClient = new SmtpClient("smtp.gmail.com", 25);

            msg.From = new MailAddress(email);
            msg.To.Add(new MailAddress(address));
            msg.Subject = subject;
            msg.Body = message;
            msg.IsBodyHtml = true;

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(msg);
        }

        // GET: Doctors/Book/5
        public ActionResult Book(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewData["id"] = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Book([Bind(Include = "Time,ID,Patient_Name,Email,Phone")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointment);
                db.SaveChanges();
                SendEmail(
                    (db.Doctors.Find(appointment.ID)).Email, 
                    "Appointment- From Bddoctors", 
                    "Appointment Time: <b>" + appointment.Time + "</b><br />" + "Name: <b>" + appointment.Patient_Name + "</b><br />" + "Phone: <b>" + appointment.Phone + "</b><br />" + "Email: <b>" + appointment.Email + "</b>"
                    );
                return RedirectToAction("Index");
            }
            return View(appointment);
        }

        // GET: Doctors/Feedback/5
        public ActionResult Feedback(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View();
        }
        //for feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Feedback([Bind(Include = "ID,comment,Popularity,Reason,patient_Name,patient_Mobile,patient_email")] Feedback feedback)
        {
            //if (ModelState.IsValid)
            //{
            db.Feedbacks.Add(feedback);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = RouteData.Values["id"] });
            //}

            //return View();
        }

        //[HttpPost]
        //public ActionResult Index(Feedback feedback)
        //{
        //    string constr = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        string query = "INSERT INTO Feedback(comment, Reason, patient_email, patient_Mobile,Popularity,patient_Name) VALUES(@comment, @reason, @email, @mobile,@name)";
        //        query += " SELECT SCOPE_IDENTITY()";
        //        using (SqlCommand cmd = new SqlCommand(query))
        //        {
        //            cmd.Connection = con;
        //            con.Open();
        //            cmd.Parameters.AddWithValue("@coment",feedback.comment );
        //            cmd.Parameters.AddWithValue("@reason",feedback.Reason );
        //            cmd.Parameters.AddWithValue("@email", feedback.patient_email);
        //            cmd.Parameters.AddWithValue("@mobile", feedback.patient_Mobile);
        //            cmd.Parameters.AddWithValue("@name", feedback.patient_Name);
        //            //cmd.Parameters.AddWithValue("@email", feedback.patient_email);
        //            feedback.feedback_ID = Convert.ToInt32(cmd.ExecuteScalar());
        //            con.Close();
        //        }
        //    }
        //    return View(feedback);
        //}
        // GET: Doctors/Create
        public ActionResult Create()
        {
            return View();
        }
        //public ActionResult relation()
        //{
        //    var doc_hos = from doc in db.Doctors
        //                  join hos in db.Hospitals on
        //                  doc.hospital_ID equals hos.hospital_ID
        //                  select new { doc.Name, hos.hospital_Name };
        //    return View(doc_hos);
        //}
        // POST: Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Speciality,AppTime,Fee,Location,Email,Hospital")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctor);
        }


        // GET: Doctors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Speciality,AppTime,Fee,Location")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //public ActionResult doc_hos()
        //{
        //    var doc_hos =
        //        from doc in db.Doctors
        //        join hos in db.Hospitals on
        //        doc.hospital_ID equals hos.hospital_ID
        //        select new { doc.Name, hos.hospital_Name };

        //    return View(doc_hos.ToList());
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
