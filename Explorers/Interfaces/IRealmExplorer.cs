using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net;
using WowDotNetAPI.Explorers.Models;

namespace WowDotNetAPI.Explorers.Interfaces
{
	public interface IRealmExplorer : IExplorer
	{
		Realm GetSingleRealm(string name);
		string GetSingleRealmAsJson(string name);

		IEnumerable<Realm> GetAllRealms();
		string GetAllRealmsAsJson();

		IEnumerable<Realm> GetRealmsByType(string type);
		string GetRealmsByTypeAsJson(string type);

		IEnumerable<Realm> GetRealmsByPopulation(string population);
		string GetRealmsByPopulationAsJson(string population);

		IEnumerable<Realm> GetRealmsByStatus(bool status);
		string GetRealmsByStatusAsJson(bool status);

		IEnumerable<Realm> GetRealmsByQueue(bool queue);
		string GetRealmsByQueueAsJson(bool queue);

		IEnumerable<Realm> GetMultipleRealms(params string[] names);
		string GetMultipleRealmsAsJson(params string[] names);

		IEnumerable<Realm> GetMultipleRealmsViaQuery(string query);
		string GetRealmsViaQueryAsJson(string query);
	}
}
