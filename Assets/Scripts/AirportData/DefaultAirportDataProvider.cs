using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AirportData
{
	public class DefaultAirportDataProvider : IAirportDataProvider
	{
		private List<AirportInfo> _airports;
		private const string AirportResourcePath = "airports";

		public List<AirportInfo> Airports
		{
			get
			{
				if (_airports == null)
					LoadData();
				return _airports;
			}
		}

		public void LoadData()
		{
			var resource = Resources.Load<TextAsset>(AirportResourcePath);

			if (!resource) throw new FileNotFoundException($"No resource found: '{AirportResourcePath}'");

			var stringPattern = @"(?:\""([^\""]+)\""|\\N),";
			var floatPattern = @"([\d.-]+),";

			string lastTry = "";
			try
			{
				_airports = resource.text.Split('\n').Select(airportString =>
				{
					lastTry = airportString;
					var stringMatches = Regex.Matches(airportString, stringPattern);
					var floatMatches = Regex.Matches(airportString, floatPattern);

					return new AirportInfo(
						stringMatches[0].Groups[1].Value,
						stringMatches[1].Groups[1].Value,
						stringMatches[2].Groups[1].Value,
						stringMatches[3].Groups[1].Value,
						stringMatches[4].Groups[1].Value,
						float.Parse(floatMatches[1].Groups[1].Value, CultureInfo.InvariantCulture),
						float.Parse(floatMatches[2].Groups[1].Value, CultureInfo.InvariantCulture));
				}).ToList();
			}
			catch (Exception e)
			{
				Debug.Log(lastTry);
				Debug.LogException(e);
				throw;
			}
		}
	}
}