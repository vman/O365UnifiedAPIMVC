using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace O365.Unified.API.Helpers
{
    public class SettingsHelper
    {
        public static string ClientId
        {
            get { return ConfigurationManager.AppSettings["ida:ClientID"]; }
        }

        public static string ClientSecret
        {
            get { return ConfigurationManager.AppSettings["ida:ClientSecret"]; }
        }

        public static string AzureADAuthority
        {
            get { return "https://login.microsoftonline.com/common"; }
        }

        public static string O365UnifiedResource
        {
            get { return "https://graph.microsoft.com/"; }
        }
    }
}