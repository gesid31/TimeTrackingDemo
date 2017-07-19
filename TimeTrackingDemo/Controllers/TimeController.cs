using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTrackingDemo.Models;
using TimeTrackingDemo.ViewModels;

namespace TimeTrackingDemo.Controllers
{
    public class TimeController : Controller
    {
        // GET: Time
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Welcome()
        {
           
            var userName = User.Identity.Name;
            tblTimeSheetVM viewModel = new tblTimeSheetVM();
            return View(viewModel.GetTimeSheet(userName));

        }

        [HttpPost]
        [Authorize]
        public ActionResult ClockOut(tblTimeSheet model)
        {
            model.UserName = User.Identity.Name;
            model.TimeSheetOUT= DateTime.Now;
            model.Status = "Clocked Out";
            //string diff = "10:48:00";
            TimeSpan? diff = model.TimeSheetOUT - model.TimeSheetIN;
            //DateTime dateTime = DateTime.ParseExact(diff, "HH:mm:ss", CultureInfo.InvariantCulture);
            model.TotalHours = Convert.ToInt32(diff);

            DemoEntities1 db = new DemoEntities1();
                db.Entry(model).State = EntityState.Modified;
                return RedirectToAction("Welcome", "Time");
        }

        [HttpPost]
        public ActionResult ClockIn(tblTimeSheet model)
        {
            try
            {
               model.UserName = User.Identity.Name;
                model.Date= DateTime.Now;
                model.TimeSheetIN = DateTime.Now;
                model.Status = "Clocked In";
                DemoEntities1 db = new DemoEntities1();
                db.tblTimeSheets.Add(model);
                db.SaveChanges();
                return RedirectToAction("Welcome", "Time");
            }
            catch (DbEntityValidationException e)
            {
                string error = string.Empty;
                foreach (var eve in e.EntityValidationErrors)
                {
                      error += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        error += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }
    }
}