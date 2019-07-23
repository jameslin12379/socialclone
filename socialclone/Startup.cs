using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace socialclone
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            // define a middleware that takes a request and checks if it contains a cookie and if yes does cookie contains session id
            // if not generate random session id and other default information and open/close redis connection to enter this information
            // setup information on request.session and return id as cookie to client and proceed to controllers
            // if yes use id to query redis and setup information on request.session and proceed to controllers
            
            // client sends request to server either with session id or not
            // if not build new session in redis and return session id as cookie to client
            // and now all requests from client to server would include cookie (session id)
            // when client clicks logout, session in redis is destroyed manually and 
            // client cookie is destroyed and return a page to client that embeds JavaScript
            // to clear client cookie in order to completely destroy session on both sides and
            // indicate to user how to navigate to website again

            app.Use((context, next) =>
            {
                string cookie = context.Request.Cookies["sid"];
                if (cookie == null)
                {
                    string sid = Guid.NewGuid().ToString();
                    string isAuthenticated = "false";
                    string id = "false";
                    string email = "false";
                    string username = "false";
                    string emaile = "Hidden";
                    string emailinvalid = "Hidden";
                    string passworde = "Hidden";
                    string passwordshort = "Hidden";
                    string usernamee = "Hidden";
                    string emailfield = "";
                    string usernamefield = "";
                    string registeralert = "Hidden";
                    string emailnotfound = "Hidden";
                    string passwordwrong = "Hidden";
                    string loginemailfield = "";
                    string profileusernamee = "Hidden";
                    string profiledescriptione = "Hidden";
                    string profileusernamefield = "";
                    string profiledescriptionfield = "";
                    string profilechanged = "false";
                    string profileupdatedalert = "Hidden";
                    string profiledeletedalert = "Hidden";
                    string postnamee = "Hidden";
                    string postdescriptione = "Hidden";
                    string posttopicide = "Hidden";
                    string postnamefield = "";
                    string postdescriptionfield = "";
                    string posttopicidfield = "";
                    string postcreatedalert = "Hidden";
                    string updatedpostnamee = "Hidden";
                    string updatedpostdescriptione = "Hidden";
                    string updatedpostnamefield = "";
                    string updatedpostdescriptionfield = "";
                    string postchanged = "false";
                    string postupdatedalert = "Hidden";
                    string postdeletedalert = "Hidden";
                    string commentdeletedalert = "Hidden";
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                    IDatabase db = redis.GetDatabase();
                    var batch = db.CreateBatch();
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("isAuthenticated", isAuthenticated) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("id", id) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("email", email) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("username", username) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("emaile", emaile) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("emailinvalid", emailinvalid) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("passworde", passworde) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("passwordshort", passwordshort) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("usernamee", usernamee) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("emailfield", emailfield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("usernamefield", usernamefield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("registeralert", registeralert) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("emailnotfound", emailnotfound) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("passwordwrong", passwordwrong) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("loginemailfield", loginemailfield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("profileusernamee", profileusernamee) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("profiledescriptione", profiledescriptione) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("profileusernamefield", profileusernamefield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("profiledescriptionfield", profiledescriptionfield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("profilechanged", profilechanged) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("profileupdatedalert", profileupdatedalert) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("profiledeletedalert", profiledeletedalert) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("postnamee", postnamee) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("postdescriptione", postdescriptione) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("posttopicide", posttopicide) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("postnamefield", postnamefield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("postdescriptionfield", postdescriptionfield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("posttopicidfield", posttopicidfield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("postcreatedalert", postcreatedalert) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("updatedpostnamee", updatedpostnamee) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("updatedpostdescriptione", updatedpostdescriptione) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("updatedpostnamefield", updatedpostnamefield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("updatedpostdescriptionfield", updatedpostdescriptionfield) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("postchanged", postchanged) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("postupdatedalert", postupdatedalert) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("postdeletedalert", postdeletedalert) });
                    batch.HashSetAsync(sid, new HashEntry[] { new HashEntry("commentdeletedalert", commentdeletedalert) });
                    batch.Execute();
                    context.Response.Cookies.Append("sid", sid);
                    context.Request.Scheme =
                    $"{sid},{isAuthenticated},{id},{email},{username},{emaile},{emailinvalid},{passworde},{passwordshort},{usernamee},{emailfield},{usernamefield},{registeralert}," +
                    $"{emailnotfound},{passwordwrong},{loginemailfield},{profileusernamee}, {profiledescriptione}, {profileusernamefield}, {profiledescriptionfield},{profilechanged}," +
                    $"{profileupdatedalert},{profiledeletedalert},{postnamee},{postdescriptione},{posttopicide},{postnamefield},{postdescriptionfield},{posttopicidfield},{postcreatedalert}," +
                    $"{updatedpostnamee},{updatedpostdescriptione},{updatedpostnamefield},{updatedpostdescriptionfield},{postchanged},{postupdatedalert},{postdeletedalert},{commentdeletedalert}";
                }
                else
                {
                    // use session id from cookie to query information from redis (open/close connection) and store them into req.session and proceed to controllers
                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
                    IDatabase db = redis.GetDatabase();
                    
                    string sid = cookie;
                    string isAuthenticated = db.HashGet(cookie, "isAuthenticated").ToString();
                    string id = db.HashGet(cookie, "id").ToString();
                    string email = db.HashGet(cookie, "email").ToString();
                    string username = db.HashGet(cookie, "username").ToString();
                    string emaile = db.HashGet(cookie, "emaile").ToString();
                    string emailinvalid = db.HashGet(cookie, "emailinvalid").ToString();
                    string passworde = db.HashGet(cookie, "passworde").ToString();
                    string passwordshort = db.HashGet(cookie, "passwordshort").ToString();
                    string usernamee = db.HashGet(cookie, "usernamee").ToString();
                    string emailfield = db.HashGet(cookie, "emailfield").ToString();
                    string usernamefield = db.HashGet(cookie, "usernamefield").ToString();
                    string registeralert = db.HashGet(cookie, "registeralert").ToString();
                    string emailnotfound = db.HashGet(cookie, "emailnotfound").ToString();
                    string passwordwrong = db.HashGet(cookie, "passwordwrong").ToString();
                    string loginemailfield = db.HashGet(cookie, "loginemailfield").ToString();
                    string profileusernamee = db.HashGet(cookie, "profileusernamee").ToString();
                    string profiledescriptione = db.HashGet(cookie, "profiledescriptione").ToString();
                    string profileusernamefield = db.HashGet(cookie, "profileusernamefield").ToString();
                    string profiledescriptionfield = db.HashGet(cookie, "profiledescriptionfield").ToString();
                    string profilechanged = db.HashGet(cookie, "profilechanged").ToString();
                    string profileupdatedalert = db.HashGet(cookie, "profileupdatedalert").ToString();
                    string profiledeletedalert = db.HashGet(cookie, "profiledeletedalert").ToString();
                    string postnamee = db.HashGet(cookie, "postnamee").ToString();
                    string postdescriptione = db.HashGet(cookie, "postdescriptione").ToString();
                    string posttopicide = db.HashGet(cookie, "posttopicide").ToString();
                    string postnamefield = db.HashGet(cookie, "postnamefield").ToString();
                    string postdescriptionfield = db.HashGet(cookie, "postdescriptionfield").ToString();
                    string posttopicidfield = db.HashGet(cookie, "posttopicidfield").ToString();
                    string postcreatedalert = db.HashGet(cookie, "postcreatedalert").ToString();
                    string updatedpostnamee = db.HashGet(cookie, "updatedpostnamee").ToString();
                    string updatedpostdescriptione = db.HashGet(cookie, "updatedpostdescriptione").ToString();
                    string updatedpostnamefield = db.HashGet(cookie, "updatedpostnamefield").ToString();
                    string updatedpostdescriptionfield = db.HashGet(cookie, "updatedpostdescriptionfield").ToString();
                    string postchanged = db.HashGet(cookie, "postchanged").ToString();
                    string postupdatedalert = db.HashGet(cookie, "postupdatedalert").ToString();
                    string postdeletedalert = db.HashGet(cookie, "postdeletedalert").ToString();
                    string commentdeletedalert = db.HashGet(cookie, "commentdeletedalert").ToString();
                    context.Request.Scheme =
                   $"{sid},{isAuthenticated},{id},{email},{username},{emaile},{emailinvalid},{passworde},{passwordshort},{usernamee},{emailfield},{usernamefield},{registeralert}," +
                   $"{emailnotfound},{passwordwrong},{loginemailfield},{profileusernamee}, {profiledescriptione}, {profileusernamefield}, {profiledescriptionfield},{profilechanged}," +
                   $"{profileupdatedalert},{profiledeletedalert},{postnamee},{postdescriptione},{posttopicide},{postnamefield},{postdescriptionfield},{posttopicidfield},{postcreatedalert}," +
                   $"{updatedpostnamee},{updatedpostdescriptione},{updatedpostnamefield},{updatedpostdescriptionfield},{postchanged},{postupdatedalert},{postdeletedalert},{commentdeletedalert}";
                }
            

                // Call the next delegate/middleware in the pipeline
                // if current request is not /login and register alert is still open
                // close register alert session state
                return next();
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

/*db.HashSet(id, "isAuthenticated", isAuthenticated);
                   db.HashSet(id, "user", user);
                   db.HashSet(id, "emaile", emaile);
                   db.HashSet(id, "emailinvalid", emailinvalid);
                   db.HashSet(id, "passworde", passworde);
                   db.HashSet(id, "passwordshort", passwordshort);
                   db.HashSet(id, "usernamee", usernamee);
                   db.HashSet(id, "emailfield", emailfield);
                   db.HashSet(id, "usernamefield", usernamefield);
                   db.HashSet(id, "registeralert", registeralert);*/
//var batch = db.CreateBatch();
/*
 *     /*Dictionary<string, string> session = new Dictionary<string, string>();
                    session.Add("id", id);
                    session.Add("isAuthenticated", isAuthenticated);
                    session.Add("user", user);
                    context.Items["session"] = session;*/


/*
context.Items["id"] = id;
context.Items["isAuthenticated"] = isAuthenticated;
context.Items["user"] = user;
context.Items["emaile"] = emaile;
context.Items["emailinvalid"] = emailinvalid;
context.Items["passworde"] = passworde;
context.Items["passwordshort"] = passwordshort;
context.Items["usernamee"] = usernamee;
context.Items["emailfield"] = emailfield;
context.Items["usernamefield"] = usernamefield;
context.Items["registeralert"] = registeralert;

context.Items["id"] = cookie;
context.Items["isAuthenticated"] = batch.HashGetAsync(cookie, "isAuthenticated");
context.Items["user"] = batch.HashGetAsync(cookie, "user");
context.Items["emaile"] = batch.HashGetAsync(cookie, "emaile");
context.Items["emailinvalid"] = batch.HashGetAsync(cookie, "emailinvalid");
context.Items["passsworde"] = batch.HashGetAsync(cookie, "passworde");
context.Items["passswordshort"] = batch.HashGetAsync(cookie, "passwordshort");
context.Items["usernamee"] = batch.HashGetAsync(cookie, "usernamee");
context.Items["emailfield"] = batch.HashGetAsync(cookie, "emailfield");
context.Items["usernamefield"] = batch.HashGetAsync(cookie, "usernamefield");
context.Items["registeralert"] = batch.HashGetAsync(cookie, "registeralert");
    */


                    //context.Items.Add("session", session);

                    //context.Session.SetString("id", id);
                    //context.Session.SetString("isAuthenticated", isAuthenticated);
                    //context.Session.SetString("user", user);
                    //context.Response.Cookies.Append("isAuthenticated", context.Session.GetString("isAuthenticated"));
                    //context.Response.Cookies.Append("user", context.Session.GetString("user"));
                    
/*string isAuthenticated = batch.HashGetAsync(cookie, "isAuthenticated");
                   string user = batch.HashGetAsync(cookie, "user");
                   string emaile = batch.HashGetAsync(cookie, "emaile");
                   string emailinvalid = batch.HashGetAsync(cookie, "emailinvalid");
                   string passworde = batch.HashGetAsync(cookie, "passworde");
                   string passwordshort = batch.HashGetAsync(cookie, "passwordshort");
                   string usernamee = batch.HashGetAsync(cookie, "usernamee");
                   string emailfield = batch.HashGetAsync(cookie, "emailfield");
                   string usernamefield = batch.HashGetAsync(cookie, "usernamefield");
                   string registeralert = batch.HashGetAsync(cookie, "registeralert");*/
//batch.Execute();

/*if (context.Request.Path.Value != "/login" && registeralert == "registeralertshown")
{
    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
    IDatabase db = redis.GetDatabase();
    var batch = db.CreateBatch();
    batch.HashSetAsync(id, new HashEntry[] { new HashEntry("registeralert", registeralert) });
    batch.Execute();
}*/
