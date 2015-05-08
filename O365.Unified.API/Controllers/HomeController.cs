using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using O365.Unified.API.Helpers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace O365.Unified.API.Controllers
{
    public class HomeController : Controller
    {
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
                    //Enable signon and read users' profile
                    using (var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/beta/me"))
                    {
                        request.Headers.Add("Authorization", "Bearer " + token);
                        request.Headers.Add("Accept", "application/json;odata.metadata=minimal");

                        using (var response = client.SendAsync(request).Result)
                        {

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var json = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                                ViewBag.CurrentUserDisplayName = json["displayName"].ToString();

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

        public ActionResult SignIn()
        {
            if (string.IsNullOrEmpty(SettingsHelper.ClientId) || string.IsNullOrEmpty(SettingsHelper.ClientSecret))
            {
                ViewBag.Message = "Please set your ClientId and ClientSecret in the web.config";

                return View("Error");
            }

            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority);

            // The url in our app that Azure should redirect to after successful signin
            string redirectUri = Url.Action("Authorize", "Home", null, Request.Url.Scheme);

            // Generate the parameterized URL for Azure signin
            Uri authUri = authContext.GetAuthorizationRequestURL(SettingsHelper.O365UnifiedResource, SettingsHelper.ClientId,
                new Uri(redirectUri), UserIdentifier.AnyUser, null);

            // Redirect the browser to the Azure signin page
            return Redirect(authUri.ToString());
        }

        public ActionResult SignOut()
        {
            // Save the token in the session
            Session["access_token"] = null;

            return Redirect(Url.Action("Index", "Home", null, Request.Url.Scheme));

        }

        public async Task<ActionResult> Authorize()
        {
            // Get the 'code' parameter from the Azure redirect
            string authCode = Request.Params["code"];

            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority);

            // The same url we specified in the auth code request
            string redirectUri = Url.Action("Authorize", "Home", null, Request.Url.Scheme);

            // Use client ID and secret to establish app identity
            ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);

            try
            {
                // Get the token
                var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
                    authCode, new Uri(redirectUri), credential, SettingsHelper.O365UnifiedResource);

                // Save the token in the session
                Session["access_token"] = authResult.AccessToken;

                
                return Redirect(Url.Action("Index", "Home", null, Request.Url.Scheme));
            }
            catch (AdalException ex)
            {
                return Content(string.Format("ERROR retrieving token: {0}", ex.Message));
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}