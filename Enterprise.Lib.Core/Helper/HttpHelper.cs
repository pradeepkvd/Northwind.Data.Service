using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;

namespace Enterprise.Lib.Core.Helper
{

    public static class HttpHelper
    {
        public static IHttpContextAccessor _httpContextAccessor { get; private set; }
        public static string GetRequestIp()
        {
            var context = _httpContextAccessor?.HttpContext;

            if (context == null)
                return string.Empty;

            var ip = string.Empty;

            try
            {
                ip = context.Connection?.RemoteIpAddress?.ToString();
            }
            catch (Exception)
            {
                //ignore exception
                //TODO: handle gracefully
                ip = "unknown ip";
            }

            return ip.Trim() == "::1" ? "127.0.0.1" : ip.Trim();
        }

        public static string GetHostname(string ipAddress = null)
        {
            var request = _httpContextAccessor?.HttpContext?.Request;

            if (request == null)
                return string.Empty;
            
            var hostName = string.Empty;
            
            ipAddress = string.IsNullOrEmpty(ipAddress) ? GetRequestIp() : ipAddress;

            //if request is still null then unable to get request from current context
            if (string.IsNullOrEmpty(ipAddress))
                return string.Empty;

            if (!string.IsNullOrEmpty(ipAddress))
            {
                try
                {
                    var hostEntry = Dns.GetHostEntry(ipAddress);
                    hostName = hostEntry?.HostName ?? string.Empty;
                }
                catch (Exception)
                {
                    //ignore exception
                    //TODO: handle gracefully
                    hostName = "unknown host";
                }
            }

            return hostName?.Trim();

        }

        public static string GetLoggedInUserName()
        {
            return _httpContextAccessor?.HttpContext?.User?.Identity?.Name??"Data Service";
        }

        public static IDictionary<string,StringValues> GetQueryStringDictionary(string queryString)
        {
           return QueryHelpers.ParseQuery(queryString);
        }

        public static dynamic GetDynamicObjectFromQueryString(IDictionary<string, StringValues> queryDictionary)
        {
            dynamic dynamicQuery = new ExpandoObject();

            IDictionary<string, object> dictionary = (IDictionary<string, object>)dynamicQuery;
            foreach (var key in queryDictionary.Keys)
            {
                dictionary.Add(key, queryDictionary.GetStringValue(key));
            }

            return dynamicQuery;
        }

        public static string GetStringValue(this IDictionary<string,StringValues> obj, string key)
        {
            return obj.ContainsKey(key) ? obj.Where(q => q.Key.ToLower() == key.ToLower())?.Select(s => s.Value)?.First() : null;
        }

        /// <summary>
        /// Call this in startup.cs to set the IHttpContextAccessor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public static void LoadHttpCopntextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
