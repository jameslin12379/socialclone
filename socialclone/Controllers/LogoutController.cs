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
    public class LogoutController : Controller
    {
        // GET: logins/create
        // if user is logged in redirect to / since users can't log in when logged in else
        // return login form
        public IActionResult Index()
        {
            //Dictionary<string, string> session = HttpContext.Items["session"] as Dictionary<string, string>;
            //string isAuthenticated = session["isAuthenticated"];
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            if (isAuthenticated == "true")
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase db = redis.GetDatabase();
                db.KeyDelete(session[0]);
                return View("Loggedout");
            }
            else
            {
                return Redirect("/");
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
