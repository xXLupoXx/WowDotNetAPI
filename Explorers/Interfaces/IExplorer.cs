﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;

namespace WowDotNetAPI.Explorers.Interfaces
{
	public interface IExplorer
	{
		string Region { get; set; }
		JavaScriptSerializer Serializer { get; set; }
		WebRequest Request { get; set; }
		string ProxyURL { get; set; }
		string ProxyUser { get; set; }
		string ProxyPassword { get; set; }
		bool HasProxy { get; set; }
	}
}
