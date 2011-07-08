using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace WowDotNetAPI
{
    public class ProxyJsonSource : IJsonSource
    {
        private readonly string proxyPassword;
        private readonly string proxyUrl;
        private readonly string proxyUser;

        public ProxyJsonSource(string proxyUser, string proxyPassword, string proxyUrl)
        {
            this.proxyPassword = proxyPassword;
            this.proxyUrl = proxyUrl;
            this.proxyUser = proxyUser;
        }

        public string GetJson(string url)
        {
            var request = WebRequest.Create(url);

            WebProxy proxy = new WebProxy(this.proxyUrl);
            proxy.Credentials = new NetworkCredential(this.proxyUser, this.proxyPassword);
            request.Proxy = proxy;

            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }
    }
}
