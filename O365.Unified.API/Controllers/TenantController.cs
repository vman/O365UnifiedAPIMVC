using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace O365.Unified.API.Controllers
{
    public class TenantController : Controller
    {
        // GET: Tenant
        public ActionResult Index()
        {
            string token = (string)Session["access_token"];
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Message = "Please Login";
                // If there's no token in the session, redirect to Home
                return View();
            }

            try
            {

                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/beta/myOrganization/tenantDetails"))
                    {
                        request.Headers.Add("Authorization", "Bearer " + token);
                        request.Headers.Add("Accept", "application/json;odata.metadata=minimal");

                        using (var response = client.SendAsync(request).Result)
                        {

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var json = JObject.Parse(response.Content.ReadAsStringAsync().Result)["value"][0];

                                ViewBag.TenantName = json["displayName"].ToString();

                                ViewBag.VerifiedDomain = json["verifiedDomains"][0]["name"].ToString();

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