using System.Collections.Generic;

namespace AirportData
{
	public interface IAirportDataProvider
	{
		List<AirportInfo> Airports { get; }
	}
}