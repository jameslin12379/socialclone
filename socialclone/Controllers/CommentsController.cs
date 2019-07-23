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
using Newtonsoft.Json;

namespace socialclone.Controllers
{
    public class CommentsController : Controller
    {

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
            DataSet data = ohp.Query($"select c.id,c.description,c.created,c.userid,c.postid,u.username from comments c inner join users u on c.userid = u.id inner join posts p on c.postid = p.id where c.id = {id}");
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
                ViewData["description"] = data.Tables[0].Rows[0]["description"];
                ViewData["created"] = data.Tables[0].Rows[0]["created"];
                ViewData["rusername"] = data.Tables[0].Rows[0]["username"];
                ViewData["userid"] = data.Tables[0].Rows[0]["userid"].ToString();
                ViewData["postid"] = data.Tables[0].Rows[0]["postid"];
                ViewData["21"] = session[21];
                ViewData["29"] = session[29];
                ViewData["36"] = session[36];
                ViewResult vr = View();
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase db = redis.GetDatabase();
                var batch = db.CreateBatch();
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profileupdatedalert", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postcreatedalert", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("postdeletedalert", "Hidden") });
                batch.Execute();
                return vr;
            }
        }




        // GET: Topicfollowing/Details/5
        public string Details2(int? id)
        {
            if (id == null)
            {
                return "404";
            }
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"select * from comments where userid = '{id}' order by created desc");
            int count = data.Tables[0].Rows.Count;
            if (count == 0)
            {
                return "404";
            }
            else
            {
                // return json data
                string json = JsonConvert.SerializeObject(data);
                return json;
            }
        }

        public string Details3(int? id)
        {
            if (id == null)
            {
                return "404";
            }
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"select * from comments where postid = '{id}' order by created desc");
            int count = data.Tables[0].Rows.Count;
            if (count == 0)
            {
                return "404";
            }
            else
            {
                // return json data
                string json = JsonConvert.SerializeObject(data);
                return json;
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
        public string Create(int n)
        {
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            if (isAuthenticated == "false")
            {
                // redirect to home
                return "404";
            }
            else
            {
                string description = HttpContext.Request.Form["description"];
                string userid = HttpContext.Request.Form["userid"];
                string postid = HttpContext.Request.Form["postid"];
                description = description.Trim();
                userid = userid.Trim();
                postid = postid.Trim();
                    //Oraclehp ohp = new Oraclehp();
                    //int r = ohp.QueryCUD($"begin insert into users (email, password, username) values ({email},{password},{username});commit;end;");
                    Oraclehp ohp = new Oraclehp();
                    DataSet data = ohp.Query($"begin insert into comments (description, userid, postid) values ('{description}','{userid}', '{postid}');commit;end;");
                    string json = JsonConvert.SerializeObject(data);
                    return json;
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
            DataSet data = ohp.Query($"select * from comments where id = '{id}'");
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
                        DataSet data2 = ohp2.Query($"begin delete from comments where id = '{id}';commit;end;");
                        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                        IDatabase db = redis.GetDatabase();
                        var batch = db.CreateBatch();
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("commentdeletedalert", "Shown") });
                        batch.Execute();
                        return Redirect($"/users/details/{session[2]}");
                    }
                }
            }
        }
    }
}