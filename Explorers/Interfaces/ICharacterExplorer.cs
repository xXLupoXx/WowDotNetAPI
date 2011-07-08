using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WowDotNetAPI.Explorers.Models;

namespace WowDotNetAPI.Explorers.Interfaces
{
	public interface ICharacterExplorer : IExplorer
	{
		Character GetSingleCharacter(string name, string realm, params string[] optionalFields);
	}
}
