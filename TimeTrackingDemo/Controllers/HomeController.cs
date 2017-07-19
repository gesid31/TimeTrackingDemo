using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TimeTrackingDemo.Models;
using System.Web.Security;

namespace TimeTrackingDemo.Controllers
{
    public class HomeController : Controller
    {
        readonly string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SidEntities"].ConnectionString;
        // GET: Home
        //[Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Staff()
        {
            demoEntities db = new demoEntities();
            return View(db.Users.ToList());
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User reg)
        {
            {
                if (ModelState.IsValid)
                {
                    demoEntities db = new demoEntities();
                    db.Users.Add(reg);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User reg)
        {
            var dataSet = new DataSet();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "CheckAccess";
                //connection.Open();
                command.Parameters.Add("@user", SqlDbType.NVarChar).Value = reg.UserName;
                command.Parameters.Add("@password", SqlDbType.NVarChar).Value = reg.Password;
                connection.Open();
                command.ExecuteNonQuery();

                var reader = new SqlDataAdapter(command);
                reader.Fill(dataSet);

            }
            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                Session["UserName"] = reg.UserName;
                FormsAuthentication.SetAuthCookie(reg.UserName, false);
                return RedirectToAction("Welcome", "Time");
            }
            else
            {
                ModelState.AddModelError("","Invalid Credentials");
                return RedirectToAction("Login");

            }
        }

       
        [Authorize]
        public ActionResult SignOut()
        {
            Session["UserName"] = null;
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }

       
        public ActionResult Edit(string userName)
        {
            demoEntities db = new demoEntities();
            if (userName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(userName);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FirstName,LastName,UserName,Password,ConfirmPassword")] User user)
        {
            if (ModelState.IsValid)
            {
            demoEntities db = new demoEntities();
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Login", "Home");
            }
        return View(user);
        }


        // GET: User/Delete/5

        public ActionResult Delete(string userName)
        {
            demoEntities db = new demoEntities();
            if (userName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(userName);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string userName)
        {
            demoEntities db = new demoEntities();
            //User user = db.Users.Find(id);
            //var user = db.Users.Where(x => x.UserID == id).First();
            var user = db.Users.First(x => x.UserName == userName);

            db.Users.Remove(user);
            db.SaveChanges();

            var user2 = db.Users.ToList();

            return RedirectToAction("Staff", user2);

        }


    }
}