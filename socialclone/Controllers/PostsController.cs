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
    public class PostsController : Controller
    {
        // GET: Users/Create
        // if user is logged in redirect to / since users can't register when logged in else
        // return registration form
        public IActionResult Create()
        {
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            if (isAuthenticated == "false")
            {
                // redirect to home
                return Redirect("/");
            }
            else
            {
                ViewData["23"] = session[23];
                ViewData["24"] = session[24];
                ViewData["25"] = session[25];
                ViewData["26"] = session[26];
                ViewData["27"] = session[27];
                ViewData["28"] = session[28];
                ViewData["isAuthenticated"] = isAuthenticated;
                ViewData["idonly"] = session[2];
                ViewData["id"] = $"/users/details/{session[2]}";
                ViewData["username"] = session[4];
                ViewResult vr = View();
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase db = redis.GetDatabase();
                var batch = db.CreateBatch();
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postnamee", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postdescriptione", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("posttopicide", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postnamefield", "") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postdescriptionfield", "") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("posttopicidfield", "") });
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
            if (isAuthenticated == "false")
            {
                // redirect to home
                return Redirect("/");
            }
            else
            {
                string name = HttpContext.Request.Form["name"];
                string description = HttpContext.Request.Form["description"];
                // validation rules
                // All fields have to be filled out
                // Email field has to be of email format
                // Password has to have at least one character, one number, and at least eight characters long
                List<string> errors = new List<string>();
                if (name.Length == 0)
                {
                    errors.Add("postnamee");
                }
                if (description.Length == 0)
                {
                    errors.Add("postdescriptione");
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
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postnamefield", name) });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postdescriptionfield", description) });
                    batch.Execute();
                    return Redirect("/posts/create");
                }
                else
                {
                    // reset session[3,4,5,6,7,8,9], set session[10]/register alert to true, clean data, insert into database (hash password), redirect to login
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                    IDatabase db = redis.GetDatabase();
                    var batch = db.CreateBatch();
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postnamee", "Hidden") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postdescriptione", "Hidden") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("posttopicide", "Hidden") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postnamefield", "") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postdescriptionfield", "") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("posttopicidfield", "") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postcreatedalert", "Shown") });
                    batch.Execute();
                    name = name.Trim();
                    description = description.Trim();
                    //Oraclehp ohp = new Oraclehp();
                    //int r = ohp.QueryCUD($"begin insert into users (email, password, username) values ({email},{password},{username});commit;end;");
                    Oraclehp ohp = new Oraclehp();
                    DataSet data = ohp.Query($"begin insert into posts (name, description, userid) values ('{name}','{description}', '{session[2]}');commit;end;");
                    return Redirect($"/users/details/{session[2]}");
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
            DataSet data = ohp.Query($"select p.id,p.name,p.description,p.imageurl,p.created,p.userid,p.topicid,u.username,t.name as topicname from posts p inner join users u on p.userid = u.id inner join topics t on p.topicid = t.id where p.id = {id}");
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
                ViewData["name"] = data.Tables[0].Rows[0]["name"];
                ViewData["imageurl"] = data.Tables[0].Rows[0]["imageurl"];
                ViewData["description"] = data.Tables[0].Rows[0]["description"];
                ViewData["created"] = data.Tables[0].Rows[0]["created"];
                ViewData["rusername"] = data.Tables[0].Rows[0]["username"];
                ViewData["topicname"] = data.Tables[0].Rows[0]["topicname"];
                ViewData["userid"] = data.Tables[0].Rows[0]["userid"].ToString();
                ViewData["topicid"] = data.Tables[0].Rows[0]["topicid"];
                ViewData["21"] = session[21];
                ViewData["35"] = session[35];
                Oraclehp ohp2 = new Oraclehp();
                DataSet likescount = ohp2.Query($"select count(*) as count from liks where likd = '{id}'");
                DataSet commentscount = ohp2.Query($"select count(*) as count from comments where postid = '{id}'");
                if (session[1] == "true")
                {
                // get followed posts
                Oraclehp ohp3 = new Oraclehp();
                    DataSet likedstatus = ohp3.Query($"select count(*) as count from liks where lik = '{session[2]}' and likd = '{id}'");
                    if (likedstatus.Tables[0].Rows[0]["count"].ToString() == "0")
                    {
                        ViewData["likedstatus"] = "Like";
                    }
                    else
                    {
                        ViewData["likedstatus"] = "Unlike";
                    }
                }

                ViewData["likescount"] = likescount.Tables[0].Rows[0]["count"];
                ViewData["commentscount"] = commentscount.Tables[0].Rows[0]["count"];
                ViewResult vr = View();
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase db = redis.GetDatabase();
                var batch = db.CreateBatch();
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profileupdatedalert", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postupdatedalert", "Hidden") });
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
            DataSet data = ohp.Query($"select * from posts where id = {id}");
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
                    if (session[2] != data.Tables[0].Rows[0]["userid"].ToString())
                    {
                        // redirect to home
                        return Redirect("/");
                    }
                    else
                    {
                        ViewData["30"] = session[30];
                        ViewData["31"] = session[31];
                        ViewData["32"] = session[32];
                        ViewData["33"] = session[33];
                        ViewData["name"] = data.Tables[0].Rows[0]["name"];
                        ViewData["description"] = data.Tables[0].Rows[0]["description"];
                        if (session[34] == "true")
                        {
                            ViewData["name"] = session[32];
                            ViewData["description"] = session[33];
                        }
                        ViewResult vr = View();
                        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                        IDatabase db = redis.GetDatabase();
                        var batch = db.CreateBatch();
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("updatedpostnamee", "Hidden") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("updatedpostdescriptione", "Hidden") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("updatedpostnamefield", "") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("updatedpostdescriptionfield", "") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postchanged", "false") });
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
            DataSet data = ohp.Query($"select * from posts where id = {id}");
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
                    if (session[2] != data.Tables[0].Rows[0]["userid"].ToString())
                    {
                        // redirect to home
                        return Redirect("/");
                    }
                    else
                    {
                        // validate data and if data is invalid open/close redis connection to toggle
                        // errors and update input fields with user entered values and redirect to 
                        // previous url 
                        string name = HttpContext.Request.Form["name"];
                        string description = HttpContext.Request.Form["description"];
                        // validation rules
                        // All fields have to be filled out
                        // Email field has to be of email format
                        // Password has to have at least one character, one number, and at least eight characters long
                        List<string> errors = new List<string>();
                        if (name.Length == 0)
                        {
                            errors.Add("updatedpostnamee");
                        }
                        if (description.Length == 0)
                        {
                            errors.Add("updatedpostdescriptione");
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
                            batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("updatedpostnamefield", name) });
                            batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("updatedpostdescriptionfield", description) });
                            batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postchanged", "true") });
                            batch.Execute();
                            return Redirect(HttpContext.Request.Path);
                        }
                        else
                        {
                            // save data into database, trigger updated profile alert, and redirect to profile page
                            Oraclehp ohp2 = new Oraclehp();
                            DataSet data2 = ohp2.Query($"begin update posts set name = '{name}', description = '{description}' where id = '{id}';commit;end;");
                            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                            IDatabase db = redis.GetDatabase();
                            var batch = db.CreateBatch();
                            batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postupdatedalert", "Shown") });
                            batch.Execute();
                            return Redirect($"/posts/details/{id}");
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
            DataSet data = ohp.Query($"select * from posts where id = '{id}'");
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
                    if (session[2] != data.Tables[0].Rows[0]["userid"].ToString())
                    {
                        // redirect to home
                        return Redirect("/");
                    }
                    else
                    {
                        // delete profile, delete current session from redis store, return page with profile deleted alert and js embedded to delete client side cookie
                        Oraclehp ohp2 = new Oraclehp();
                        DataSet data2 = ohp2.Query($"begin delete from posts where id = '{id}';commit;end;");
                        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                        IDatabase db = redis.GetDatabase();
                        var batch = db.CreateBatch();
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postdeletedalert", "Shown") });
                        batch.Execute();
                        return Redirect($"/users/details/{session[2]}");
                    }
                }
            }
        }
    }
}
