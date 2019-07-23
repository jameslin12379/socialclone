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
    public class LoginsController : Controller
    {
        // GET: logins/create
        // if user is logged in redirect to / since users can't log in when logged in else
        // return login form
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
                ViewData["12"] = session[12];
                ViewData["13"] = session[13];
                ViewData["14"] = session[14];
                ViewData["15"] = session[15];
                ViewData["isAuthenticated"] = isAuthenticated;
                ViewResult vr = View();
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase db = redis.GetDatabase();
                var batch = db.CreateBatch();
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("registeralert", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("emailnotfound", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("passwordwrong", "Hidden") });
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("loginemailfield", "") });
                batch.Execute();
                return vr;
            }
        }


        // POST: logins/create
        // if user is logged in redirect to / else extract email and password from request body 
        // and check if email is found in database and if not open/close redis to update emailnotfound field
        // to Shown and loginemailfield to email from request body and redirect to /login else check 
        // if password from sql result matches password from request body and if not open/close redis to update passwordwrong field
        // to Shown and loginemailfield to email from request body and redirect to /login else (authentication done)
        // open/close redis to update isAuthenticated field to true, id to id, email to email, username to username
        // and redirect to /.

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
                // if 
                string email = HttpContext.Request.Form["email"];
                string password = HttpContext.Request.Form["password"];
                Oraclehp ohp = new Oraclehp();
                DataSet data = ohp.Query($"select id, email, password, username from users where email = '{email}'");
                int count = data.Tables[0].Rows.Count;
                if (count == 0)
                {
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                    IDatabase db = redis.GetDatabase();
                    var batch = db.CreateBatch();
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("emailnotfound", "Shown") });
                    batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("loginemailfield", email) });
                    batch.Execute();
                    return Redirect("/logins/create");
                }
                else
                {
                    if (data.Tables[0].Rows[0]["password"] as string != password)
                    {
                        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                        IDatabase db = redis.GetDatabase();
                        var batch = db.CreateBatch();
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("passwordwrong", "Shown") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("loginemailfield", email) });
                        batch.Execute();
                        return Redirect("/logins/create");
                    }
                    else
                    {
                        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                        IDatabase db = redis.GetDatabase();
                        var batch = db.CreateBatch();
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("isAuthenticated", "true") });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("id", data.Tables[0].Rows[0]["id"].ToString()) });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("email", data.Tables[0].Rows[0]["email"].ToString()) });
                        batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("username", data.Tables[0].Rows[0]["username"].ToString()) });
                        batch.Execute();
                        return Redirect("/");
                    }
                }
            }
        }


    }
}

/* ViewData["emaile"] = HttpContext.Items["emaile"] as string;
               ViewData["emailinvalid"] = HttpContext.Items["emailinvalid"] as string;
               ViewData["passworde"] = HttpContext.Items["passworde"] as string;
               ViewData["passwordshort"] = HttpContext.Items["passwordshort"] as string;
               ViewData["usernamee"] = HttpContext.Items["usernamee"] as string;
               ViewData["emailfield"] = HttpContext.Items["emailfield"] as string;
               ViewData["usernamefield"] = HttpContext.Items["usernamefield"] as string;*/
