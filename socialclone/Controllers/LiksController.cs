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
    public class LiksController : Controller
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
            //DataSet data = ohp.Query($"select * from posts where id in (select likd from liks where lik = {id})");
            DataSet data = ohp.Query($"select * from posts p inner join liks l on p.id = l.likd where l.lik = '{id}' order by l.created desc");
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
            //DataSet data = ohp.Query($"select * from users where id in (select lik from liks where likd = {id})");
            DataSet data = ohp.Query($"select * from users u inner join liks l on u.id = l.lik where l.likd = '{id}' order by l.created desc");

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
                string rid = HttpContext.Request.Form["rid"];
                Oraclehp ohp = new Oraclehp();
                DataSet data = ohp.Query($"begin insert into liks (lik, likd) values ('{userid}','{rid}');commit;end;");
                string json = JsonConvert.SerializeObject(data);
                return json;
            }
        }


        
        // POST: Topicfollowing/Delete/5
        [HttpPost, ActionName("Delete")]
        public string DeleteConfirmed(int? id)
        {
            string userid = HttpContext.Request.Form["userid"];
            string rid = HttpContext.Request.Form["rid"];
            Oraclehp ohp = new Oraclehp();
            DataSet data = ohp.Query($"begin delete from liks where lik = '{userid}' and likd = '{rid}';commit;end;");
            string json = JsonConvert.SerializeObject(data);
            return json;
        }
        
        
    }
}