using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace O365.Unified.API.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            string token = (string)Session["access_token"];
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Message = "Please Login";

                return View();
            }

            try
            {

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/beta/myOrganization/users"))
                    {
                        request.Headers.Add("Authorization", "Bearer " + token);
                        request.Headers.Add("Accept", "application/json;odata.metadata=minimal");

                        using (var response = client.SendAsync(request).Result)
                        {

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var users = JObject.Parse(response.Content.ReadAsStringAsync().Result)["value"];
                                List<string> userList = new List<string>();

                                foreach (var user in users)
                                {
                                    string displayName = user["displayName"].ToString();
                                    userList.Add(displayName);
                                }

                                ViewBag.Users = userList;

                            }

                        }
                    }
                }

                return View();
            }
            catch (AdalException ex)
            {
                return Content(string.Format("ERROR retrieving messages: {0}", ex.Message));
            }
        }
    }
}