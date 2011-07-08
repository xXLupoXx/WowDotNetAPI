using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowDotNetAPI
{
    public interface IJsonSource
    {
        string GetJson(string url);
    }
}
