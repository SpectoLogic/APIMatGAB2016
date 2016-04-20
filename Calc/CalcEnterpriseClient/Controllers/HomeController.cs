using CalcEnterpriseClient.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace CalcEnterpriseClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // Using API Managment
        public const string C_BASE_SERVICE_URI = "https://<yourAPIMInstanceName>.azure-api.net/calcenterprise";
        public const string C_SERVICE_PATH = "https://<yourAPIMInstanceName>.azure-api.net/calcenterprise/api/MathClass/";
        // Accessing API's directly
        //public const string C_BASE_SERVICE_URI = "<yourcalcEnterpriseAPIwebsitesUrl>";
        //public const string C_SERVICE_PATH = "/api/MathClass/";

        public const string C_CALC_API_SUBSCRIPTION_KEY = "<your API MGMT API key>";

        /// <summary>
        /// WEBAPP and WEBAPI are in a different AAD APP
        /// Use this method if this app is protected by ADD-Application: CalcEnterpriseClient
        /// Authenticate to the API by using a service principal aka AAD App (with Delegate Permissions to
        /// CalcEnterpriseAPI)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string _ClientID = "<CalcEnterpriseClient AAD CLientID>";              // CalcEnterpriseClient - Client ID
            string _ClientSecret = "<CalcEnterpriseClient AAD App Secret/Key>";  // CalcEnterpriseClient - Client Secret
            string _Ressource = "<CalcEnterpriseAPI AAD CLientID>";              // CalcEnterpriseAPI - Client ID to be used if we call from middle tier!
            /*
                https://azure.microsoft.com/en-us/documentation/articles/app-service-api-dotnet-service-principal-auth/
                ida:ClientId and ida:Resource are different values for this tutorial because you're using separate Azure AD 
                applications for the middle tier and data tier. If you were using a single Azure AD application for the calling 
                API app and the protected API app, you would use the same value in both ida:ClientId and ida:Resource
            */
            string _Authority = "https://login.microsoftonline.com/<yourTenantName>.onmicrosoft.com";
            SessionTalk[] contacts = null;

            var clientCredential = new ClientCredential(_ClientID, _ClientSecret);
            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext context =
                new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(_Authority, false);

            AuthenticationResult authenticationResult = context.AcquireToken(
                    _Ressource,
                     clientCredential);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new
                    Uri(C_BASE_SERVICE_URI);

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", C_CALC_API_SUBSCRIPTION_KEY);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authenticationResult.AccessTokenType, authenticationResult.AccessToken);
                string path = C_SERVICE_PATH;
                try
                {
                    var json = client.GetStringAsync(path).Result;
                    contacts = JsonConvert.DeserializeObject<SessionTalk[]>(json);
                }
                catch (Exception ed)
                {
                    contacts = new SessionTalk[] { new SessionTalk() { Title = ed.ToString() } };
                }
            }

            return View(contacts);
        }

        /// <summary>
        /// WEBAPP and WEBAPI are in a different AAD APP
        /// Use this method if this app is protected by ADD-Application: CalcEnterpriseClient
        /// Authenticate by requesting a bearer token for the user and the backend service
        /// CalcEnterpriseAPI app => (IMPERSONATION - Webapi will have Caller Details in Claims not just Service Principal)!
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Index_OtherAAD(string code)
        {
            SessionTalk[] contacts = null;
            string _ClientID = "<CalcEnterpriseClient AAD CLientID>";              // CalcEnterpriseClient - Client ID
            string _ClientSecret = "<CalcEnterpriseClient AAD App Secret/Key>";  // CalcEnterpriseClient - Client Secret
            string _Ressource = "<CalcEnterpriseAPI AAD CLientID>";              // CalcEnterpriseAPI - Client ID to be used if we call from middle tier!

            if (String.IsNullOrEmpty(code))
            {
                string authorizationUrl = string.Format(
                    "https://login.windows.net/{0}/oauth2/authorize?" +
                    "api-version=1.0&" +
                    "response_type=code&" +
                    "client_id={1}&" +
                    "resource={2}&" +
                    "redirect_uri={3}",
                    ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value,
                    _ClientID,
                    _Ressource,
                    Request.Url.ToString()
                    );
                return new RedirectResult(authorizationUrl);
            }
            else
            {
                Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext ac =
                      new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(string.Format("https://login.windows.net/{0}",
                                  ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value));

                ClientCredential clcred =
                 new ClientCredential(_ClientID, _ClientSecret);

                var ar = ac.AcquireTokenByAuthorizationCode(code,
                    new Uri(Request.Url.ToString().Replace(Request.Url.Query, "")), clcred);

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new
                        Uri(C_BASE_SERVICE_URI);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", C_CALC_API_SUBSCRIPTION_KEY);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ar.AccessTokenType, ar.AccessToken);
                    var json = client.GetStringAsync(C_SERVICE_PATH).Result;
                    contacts = JsonConvert.DeserializeObject<SessionTalk[]>(json);
                }
                return View(contacts);
            }
        }

        /// <summary>
        /// WEBAPP/API IN THE SAME AAD APP
        /// Use this method if this app and the API is protected by AAD-Application: CalcEnterpriseAPI 
        /// Try to authenticate with the user by passing the Authorization Token
        /// Since we are in the same AAD App we should be able to pass on the bearer token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult Index_SameAAD()
        {
            SessionTalk[] contacts = null;
            string bearerToken = Request.Headers["X-MS-TOKEN-AAD-ID-TOKEN"];
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new
                    Uri(C_BASE_SERVICE_URI);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", C_CALC_API_SUBSCRIPTION_KEY);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                var json = client.GetStringAsync(C_SERVICE_PATH).Result;
                contacts = JsonConvert.DeserializeObject<SessionTalk[]>(json);
            }
            return View(contacts);
        }
    }
}