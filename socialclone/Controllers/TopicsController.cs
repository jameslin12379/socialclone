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
using StackExchange.Redis;


namespace socialclone.Controllers
{
    public class TopicsController : Controller
    {
        // if user is logged in or not, get all topics and list them 
        public IActionResult Index()
        {
            //Dictionary<string, string> session = HttpContext.Items["session"] as Dictionary<string, string>;
            //string isAuthenticated = session["isAuthenticated"];
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            string isAuthenticated = session[1];
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"select * from topics order by name");
            DataRowCollection rows = data.Tables[0].Rows;
            ViewData["rows"] = rows;
            ViewData["isAuthenticated"] = isAuthenticated;
            ViewData["id"] = $"/users/details/{session[2]}";
            ViewData["username"] = session[4];
            return View();
        }

        // GET: Topics/Details/5
        // if user is logged out get topic, topic posts, posts count, following count, ajax list of followers
        // if user is logged in get same information, include follow/unfollow section, check if user has followed topic already
        // and if yes set value of followed else follow, include js script to follow/unfollow with ajax to make changes
        // to database and when done update UI
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"select * from topics where id = '{id}'");
            int count = data.Tables[0].Rows.Count;
            if (count == 0)
            {
                return NotFound();
            }
            else
            {
                // pass topic data into view, get posts from topic into view, and topicfollowing count and hidden modal into view
                
                // write js script so if user clicks on topicfollowing count ajax request 
                // is sent to get users who have followed this topic and insert them into hidden modal and toggle modal
                // if close button is clicked modal is closed
                Oraclehp ohp2 = new Oraclehp();
                DataSet posts = ohp2.Query($"select p.id,p.name,p.description,p.imageurl,p.created,p.userid,p.topicid,u.username,t.name as topicname from posts p inner join users u on p.userid = u.id inner join topics t on p.topicid = t.id where topicid = '{id}' order by created desc");
                DataSet postscount = ohp2.Query($"select count(*) as count from posts where topicid = '{id}'");
                DataSet followingcount = ohp2.Query($"select count(*) as count from topicfollowing where followed = '{id}'");
                if (session[1] == "true")
                {
                    // get followed posts
                    Oraclehp ohp3 = new Oraclehp();
                    DataSet followedstatus = ohp3.Query($"select count(*) as count from topicfollowing where following = '{session[2]}' and followed = '{id}'");
                    if (followedstatus.Tables[0].Rows[0]["count"].ToString()  == "0")
                    {
                        ViewData["followedstatus"] = "Follow";
                    } else
                    {
                        ViewData["followedstatus"] = "Unfollow";
                    }

                }
                ViewData["isAuthenticated"] = session[1];
                ViewData["idonly"] = session[2];
                ViewData["id"] = $"/users/details/{session[2]}";
                ViewData["username"] = session[4];
                ViewData["rid"] = id.ToString();
                ViewData["name"] = data.Tables[0].Rows[0]["name"];
                ViewData["imageurl"] = data.Tables[0].Rows[0]["imageurl"];
                ViewData["description"] = data.Tables[0].Rows[0]["description"];
                ViewData["posts"] = posts.Tables[0].Rows;
                ViewData["postscount"] = postscount.Tables[0].Rows[0]["count"];
                ViewData["followingcount"] = followingcount.Tables[0].Rows[0]["count"];
                ViewResult vr = View();
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                IDatabase db = redis.GetDatabase();
                var batch = db.CreateBatch();
                batch.HashSetAsync(session[0], new HashEntry[] { new HashEntry("profileupdatedalert", "Hidden") });
                batch.Execute();
                return vr;
            }
        }
    }


}
