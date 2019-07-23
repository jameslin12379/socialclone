using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using socialclone.Models;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using socialclone.oracle;
using System.Text.RegularExpressions;
using StackExchange.Redis;

namespace socialclone.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users/Create
        // if user is logged in redirect to / since users can't register when logged in else
        // return registration form
        public IActionResult Create()
        {
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            if (isAuthenticated == "true")
            {
                // redirect to home
                return Redirect("/");
            }
            else
            {
                ViewData["5"] = session[5];
                ViewData["6"] = session[6];
                ViewData["7"] = session[7];
                ViewData["8"] = session[8];
                ViewData["9"] = session[9];
                ViewData["10"] = session[10];
                ViewData["11"] = session[11];
                ViewData["isAuthenticated"] = isAuthenticated;
                ViewResult vr = View();
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase db = redis.GetDatabase();
                var batch = db.CreateBatch();
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("emaile", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("emailinvalid", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("passworde", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("passwordshort", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("usernamee", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("emailfield", "") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("usernamefield", "") });
                batch.Execute();
                return vr;
                /*ViewData["emaile"] = HttpContext.Items["emaile"] as string;
                ViewData["emailinvalid"] = HttpContext.Items["emailinvalid"] as string;
                ViewData["passworde"] = HttpContext.Items["passworde"] as string;
                ViewData["passwordshort"] = HttpContext.Items["passwordshort"] as string;
                ViewData["usernamee"] = HttpContext.Items["usernamee"] as string;
                ViewData["emailfield"] = HttpContext.Items["emailfield"] as string;
                ViewData["usernamefield"] = HttpContext.Items["usernamefield"] as string;*/
            }
        }

        // POST: Users/Create
        // if user is logged in redirect to / since users can't register when logged in else
        // process post data through validation and if validation fails store error message and input into session store and redirect to /users/create
        // (redirect is a http request to /posts/create and same workflow except program checks
        // if session has errors inside and if yes grab them and in users/create view
        // logic is written so if session store has errors errors will be displayed 
        // and same goes for input) if validation succeeds then data are sanitized
        // and saved into the database, message is saved into redis store, and redirect user to login page 
        // (session is established so get information from redis store and checks if 
        // message key is here and if yes store it into request session and in view
        // write logic so if session has message display UI)
        //
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int n)
        {
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            if (isAuthenticated == "true")
            {
                // redirect to home
                return Redirect("/");
            }
            else
            {
                string email = HttpContext.Request.Form["email"];
                string password = HttpContext.Request.Form["password"];
                string username = HttpContext.Request.Form["username"];
                // validation rules
                // All fields have to be filled out
                // Email field has to be of email format
                // Password has to have at least one character, one number, and at least eight characters long
                List<string> errors = new List<string>();
                if (email.Length == 0)
                {
                    errors.Add("emaile");
                }
                if (password.Length == 0)
                {
                    errors.Add("passworde");
                }
                if (username.Length == 0)
                {
                    errors.Add("usernamee");
                }
                if (!Regex.IsMatch(email, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase))
                {
                    errors.Add("emailinvalid");
                }
                if (password.Length < 8)
                {
                    errors.Add("passwordshort");
                }
                if (errors.Count > 0)
                {
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                    IDatabase db = redis.GetDatabase();
                    var batch = db.CreateBatch();
                    //string errorconfirmed = "";
                    for (var i = 0; i < errors.Count; i++)
                    {
                        string error = errors[i];
                        /*switch (error)
                        {
                            case "emaile":
                                errorconfirmed = "emaile";
                                break;
                            case "passworde":
                                errorconfirmed = "passworde";
                                break;
                            case "usernamee":
                                errorconfirmed = "usernamee";
                                break;
                            case "emailinvalid":
                                errorconfirmed = "emailinvalid";
                                break;
                            default:
                                errorconfirmed = "passwordshort";
                                break;
                        }*/
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry(error, "Shown") });
                        //db.HashSet(HttpContext.Items["id"] as string, new HashEntry[] { new HashEntry(errors[i],"true") });
                        //db.HashSet(HttpContext.Items["id"] as string, new HashEntry[] { new HashEntry("email", email) });
                        //db.HashSet(HttpContext.Items["id"] as string, new HashEntry[] { new HashEntry("username", username) });
                    }
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("emailfield", email) });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("usernamefield", username) });
                    batch.Execute();
                    return Redirect("/users/create");
                }
                else
                {
                    // reset session[3,4,5,6,7,8,9], set session[10]/register alert to true, clean data, insert into database (hash password), redirect to login
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                    IDatabase db = redis.GetDatabase();
                    var batch = db.CreateBatch();
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("emaile", "Hidden") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("emailinvalid", "Hidden") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("passworde", "Hidden") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("passwordshort", "Hidden") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("usernamee", "Hidden") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("emailfield", "") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("usernamefield", "") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("registeralert", "Shown") });
                    batch.Execute();
                    email = email.Trim();
                    password = password.Trim();
                    username = username.Trim();
                    //Oraclehp ohp = new Oraclehp();
                    //int r = ohp.QueryCUD($"begin insert into users (email, password, username) values ({email},{password},{username});commit;end;");
                    Oraclehp ohp = new Oraclehp();
                    DataSet data = ohp.Query($"begin insert into users (email, password, username) values ('{email}','{password}','{username}');commit;end;");
                    return Redirect("/logins/create");
                }
            }
        }

        // GET: Users/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"select * from users where id = {id}");
            int count = data.Tables[0].Rows.Count;
            if (count == 0)
            {
                return NotFound();
            }
            else
            {
                ViewData["isAuthenticated"] = session[1];
                ViewData["idonly"] = session[2];
                ViewData["id"] = $"/users/details/{session[2]}";
                ViewData["username"] = session[4];
                ViewData["rid"] = id.ToString();
                ViewData["rusername"] = data.Tables[0].Rows[0]["username"];
                ViewData["imageurl"] = data.Tables[0].Rows[0]["imageurl"];
                ViewData["description"] = data.Tables[0].Rows[0]["description"];
                ViewData["created"] = data.Tables[0].Rows[0]["created"];
                ViewData["21"] = session[21];
                ViewData["29"] = session[29];
                ViewData["36"] = session[36];
                ViewData["37"] = session[37];
                Oraclehp ohp2 = new Oraclehp();
                DataSet posts = ohp2.Query($"select p.id,p.name,p.description,p.imageurl,p.created,p.userid,p.topicid,u.username,t.name as topicname from posts p inner join users u on p.userid = u.id inner join topics t on p.topicid = t.id where userid = '{id}' order by created desc");
                DataSet postscount = ohp2.Query($"select count(*) as count from posts where userid = '{id}'");
                DataSet topicscount = ohp2.Query($"select count(*) as count from topicfollowing where following = '{id}'");
                DataSet userfollowingcount = ohp2.Query($"select count(*) as count from userfollowing where following = '{id}'");
                DataSet userfollowedcount = ohp2.Query($"select count(*) as count from userfollowing where followed = '{id}'");
                DataSet likescount = ohp2.Query($"select count(*) as count from liks where lik = '{id}'");
                DataSet commentscount = ohp2.Query($"select count(*) as count from comments where userid = '{id}'");
                if (session[1] == "true" && session[2] != id.ToString())
                {
                    // get followed posts
                    Oraclehp ohp3 = new Oraclehp();
                    DataSet followedstatus = ohp3.Query($"select count(*) as count from userfollowing where following = '{session[2]}' and followed = '{id}'");
                    if (followedstatus.Tables[0].Rows[0]["count"].ToString() == "0")
                    {
                        ViewData["followedstatus"] = "Follow";
                    }
                    else
                    {
                        ViewData["followedstatus"] = "Unfollow";
                    }

                }
                ViewData["posts"] = posts.Tables[0].Rows;
                ViewData["postscount"] = postscount.Tables[0].Rows[0]["count"];
                ViewData["topicscount"] = topicscount.Tables[0].Rows[0]["count"];
                ViewData["userfollowingcount"] = userfollowingcount.Tables[0].Rows[0]["count"];
                ViewData["userfollowedcount"] = userfollowedcount.Tables[0].Rows[0]["count"];
                ViewData["likescount"] = likescount.Tables[0].Rows[0]["count"];
                ViewData["commentscount"] = commentscount.Tables[0].Rows[0]["count"];
                ViewResult vr = View();
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase db = redis.GetDatabase();
                var batch = db.CreateBatch();
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profileupdatedalert", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postcreatedalert", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postdeletedalert", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("commentdeletedalert", "Hidden") });
                batch.Execute();
                return vr;
            }
        }

        // GET: Users/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            ViewData["isAuthenticated"] = session[1];
            ViewData["id"] = $"/users/details/{session[2]}";
            ViewData["username"] = session[4];
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"select * from users where id = {id}");
            int count = data.Tables[0].Rows.Count;
            if (count == 0)
            {
                return NotFound();
            }
            else
            {
                if (isAuthenticated == "false")
                {
                    // redirect to home
                    return Redirect("/");
                }
                else
                {
                    if (session[2] != id.ToString())
                    {
                        // redirect to home
                        return Redirect("/");
                    }
                    else
                    {
                        ViewData["16"] = session[16];
                        ViewData["17"] = session[17];
                        ViewData["18"] = session[18];
                        ViewData["19"] = session[19];
                        ViewData["inputusername"] = data.Tables[0].Rows[0]["username"];
                        ViewData["description"] = data.Tables[0].Rows[0]["description"];
                        if (session[20] == "true")
                        {
                            ViewData["inputusername"] = session[18];
                            ViewData["description"] = session[19];
                        }
                        ViewResult vr = View();
                        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                        IDatabase db = redis.GetDatabase();
                        var batch = db.CreateBatch();
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profileusernamee", "Hidden") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profiledescriptione", "Hidden") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profileusernamefield", "") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profiledescriptionfield", "") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profilechanged", "false") });
                        batch.Execute();
                        return vr;
                    }
                }
            }
        }


        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, int? x)
        {
            if (id == null)
            {
                return NotFound();
            }
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            ViewData["isAuthenticated"] = session[1];
            ViewData["id"] = $"/users/details/{session[2]}";
            ViewData["username"] = session[4];
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"select * from users where id = {id}");
            int count = data.Tables[0].Rows.Count;
            if (count == 0)
            {
                return NotFound();
            }
            else
            {
                if (isAuthenticated == "false")
                {
                    // redirect to home
                    return Redirect("/");
                }
                else
                {
                    if (session[2] != id.ToString())
                    {
                        // redirect to home
                        return Redirect("/");
                    }
                    else
                    {
                        // validate data and if data is invalid open/close redis connection to toggle
                        // errors and update input fields with user entered values and redirect to 
                        // previous url 
                        string username = HttpContext.Request.Form["username"];
                        string description = HttpContext.Request.Form["description"];
                        // validation rules
                        // All fields have to be filled out
                        // Email field has to be of email format
                        // Password has to have at least one character, one number, and at least eight characters long
                        List<string> errors = new List<string>();
                        if (username.Length == 0)
                        {
                            errors.Add("profileusernamee");
                        }
                        if (description.Length == 0)
                        {
                            errors.Add("profiledescriptione");
                        }
                        if (errors.Count > 0)
                        {
                            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                            IDatabase db = redis.GetDatabase();
                            var batch = db.CreateBatch();
                            for (var i = 0; i < errors.Count; i++)
                            {
                                string error = errors[i];
                                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry(error, "Shown") });
                            }
                            batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profileusernamefield", username) });
                            batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profiledescriptionfield", description) });
                            batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profilechanged", "true") });
                            batch.Execute();
                            return Redirect($"/users/edit/{id}");
                        }
                        else
                        {
                            // save data into database, trigger updated profile alert, and redirect to profile page
                            Oraclehp ohp2 = new Oraclehp();
                            DataSet data2 = ohp2.Query($"begin update users set username = '{username}', description = '{description}' where id = '{id}';commit;end;");
                            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                            IDatabase db = redis.GetDatabase();
                            var batch = db.CreateBatch();
                            batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profileupdatedalert", "Shown") });
                            batch.Execute();
                            return Redirect($"/users/details/{session[2]}");
                        }
                    }
                }
            }
        }


        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            ViewData["isAuthenticated"] = session[1];
            ViewData["id"] = $"/users/details/{session[2]}";
            ViewData["username"] = session[4];
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"select * from users where id = '{id}'");
            int count = data.Tables[0].Rows.Count;
            if (count == 0)
            {
                return NotFound();
            }
            else
            {
                if (isAuthenticated == "false")
                {
                    // redirect to home
                    return Redirect("/");
                }
                else
                {
                    if (session[2] != id.ToString())
                    {
                        // redirect to home
                        return Redirect("/");
                    }
                    else
                    {
                        // delete profile, delete current session from redis store, return page with profile deleted alert and js embedded to delete client side cookie
                        Oraclehp ohp2 = new Oraclehp();
                        DataSet data2 = ohp2.Query($"begin delete from users where id = '{id}';commit;end;");
                        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                        IDatabase db = redis.GetDatabase();
                        db.KeyDelete(session[0]);
                        return View("Deleted");
                    }
                }
            }
        }
     }
}
