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
    public class TopicfollowingController : Controller
    { 
// GET: Topicfollowing/Details/5
public string Details(int? id)
    {
        if (id == null)
        {
            return "404";
        }
        string sessionstring = HttpContext.Request.Scheme;
        string[] session = sessionstring.Split(",");
        Oraclehp ohp = new Oraclehp();
            //DataSet data = ohp.Query($"select * from users where id in (select following from topicfollowing where followed = '{id}' order by created desc)");
            DataSet data = ohp.Query($"select * from users u inner join topicfollowing tf on u.id = tf.following where tf.followed = '{id}' order by tf.created desc");
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

        public string Details2(int? id)
        {
            if (id == null)
            {
                return "404";
            }
            string sessionstring = HttpContext.Request.Scheme;
            string[] session = sessionstring.Split(",");
            Oraclehp ohp = new Oraclehp();
            //DataSet data = ohp.Query($"select * from topics where id in (select followed from topicfollowing where following = {id})");
            DataSet data = ohp.Query($"select * from topics t inner join topicfollowing tf on t.id = tf.followed where tf.following = '{id}' order by tf.created desc");
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

        // POST: Topicfollowing/Create
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
                // insert post data to topicfollowing and return something
                string userid = HttpContext.Request.Form["userid"];
                string topicid = HttpContext.Request.Form["topicid"];
                Oraclehp ohp = new Oraclehp();
                DataSet data = ohp.Query($"begin insert into topicfollowing (following, followed) values ('{userid}','{topicid}');commit;end;");
                string json = JsonConvert.SerializeObject(data);
                return json;
            }
        }



        // POST: Topicfollowing/Delete/5
        [HttpPost, ActionName("Delete")]
        public string DeleteConfirmed(int? id)
        {
            string userid = HttpContext.Request.Form["userid"];
            string topicid = HttpContext.Request.Form["topicid"];
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"begin delete from topicfollowing where following = '{userid}' and followed = '{topicid}';commit;end;");
            string json = JsonConvert.SerializeObject(data);
            return json;
        }
    }
}