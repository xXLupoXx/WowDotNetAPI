using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace WowDotNetAPI
{
    public class JsonSource : IJsonSource
    {
        public string GetJson(string url)
        {
            var request = WebRequest.Create(url);

            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }
    }
}
