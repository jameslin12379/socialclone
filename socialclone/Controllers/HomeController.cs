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

namespace socialclone.Controllers
{
    public class HomeController : Controller
    {
        // if user is logged in get posts from followed topics/people
        // else get all posts either way make sure
        // to open/close a database connection to get needed data
        public IActionResult Index()
        {
            //Dictionary<string, string> session = HttpContext.Items["session"] as Dictionary<string, string>;
            //string isAuthenticated = session["isAuthenticated"];
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            if (isAuthenticated == "true")
            {
                // get followed posts
                Oraclehp ohp = new Oraclehp();
                DataSet data = ohp.Query($"select p.id,p.name,p.description,p.imageurl,p.created,p.userid,p.topicid,u.username,t.name as topicname from posts p inner join users u on p.userid = u.id inner join topics t on p.topicid = t.id where topicid in (select followed from topicfollowing where following = '{session[2]}') order by created desc");
                
                DataRowCollection rows = data.Tables[0].Rows;
                ViewData["rowsc"] = rows.Count;
                ViewData["rows"] = rows;
                ViewData["isAuthenticated"] = isAuthenticated;
                ViewData["id"] = $"/users/details/{session[2]}";
                ViewData["username"] = session[4];
                return View();
            }
            else
            {
                Oraclehp ohp = new Oraclehp();
                DataSet data = ohp.Query("select p.id,p.name,p.description,p.imageurl,p.created,p.userid,p.topicid,u.username,t.name as topicname from posts p inner join users u on p.userid = u.id inner join topics t on p.topicid = t.id order by p.created desc");
                DataRowCollection rows = data.Tables[0].Rows;
                ViewData["rows"] = rows;
                ViewData["isAuthenticated"] = isAuthenticated;
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
