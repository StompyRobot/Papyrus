using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Papyrus.DataTypes;

namespace Papyrus
{
	public static class StarSystemExtensions
	{

		/// <summary>
		/// True if a link between the two systems exists
		/// </summary>
		/// <param name="system"></param>
		/// <param name="otherSystem"></param>
		/// <returns></returns>
		public static bool LinkExists(this StarSystem system, StarSystem otherSystem)
		{

			return (system.Links.Select(p => p.Value).Contains(otherSystem) || otherSystem.Links.Select(p => p.Value).Contains(system));

		}

		/// <summary>
		/// Returns all the links to this star system in the database.
		/// </summary>
		/// <param name="system"></param>
		/// <returns></returns>
		public static List<StarSystem> GetLinks(this StarSystem system)
		{

			var systems = system.Database.GetRecordsOfType<StarSystem>();
			return systems.Where(p => p.LinkExists(system)).ToList();

		} 

	}
}
