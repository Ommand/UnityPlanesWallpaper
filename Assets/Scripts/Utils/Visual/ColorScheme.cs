using System.Collections.Generic;
using UnityEngine;

namespace Utils.Visual
{
	public static class ColorScheme
	{
		private static readonly List<(Color, Color)> Colors = new List<(Color, Color)>()
		{
			(FromHex(0x59C742), FromHex(0xEE9A4F)),
			(FromHex(0x59C742), FromHex(0xAB3995)),
			(FromHex(0x9DDA49), FromHex(0xEE784F)),
			(FromHex(0x9DDA49), FromHex(0x7D3A9F)),
			(FromHex(0xE1EB4E), FromHex(0xCF4578)),
			(FromHex(0xE1EB4E), FromHex(0x5943A4)),
			(FromHex(0xEEDD4F), FromHex(0xA53798)),
			(FromHex(0xEEDD4F), FromHex(0x4356A2)),
			(FromHex(0xEEC44F), FromHex(0x6C3EA1)),
			(FromHex(0xEEC44F), FromHex(0x357F94)),
			(FromHex(0xEEA14F), FromHex(0x474CA5)),
			(FromHex(0xEEA14F), FromHex(0xAB3995)),
			(FromHex(0xEE734F), FromHex(0x3B6D9A)),
			(FromHex(0xEE734F), FromHex(0xA1DB49)),
			(FromHex(0xD64770), FromHex(0x349B81)),
			(FromHex(0xD64770), FromHex(0xD6E84D)),
			(FromHex(0xBA3E8C), FromHex(0x3DB652)),
			(FromHex(0xBA3E8C), FromHex(0xEEE54F)),
			(FromHex(0x8B379D), FromHex(0x87D447)),
			(FromHex(0x8B379D), FromHex(0xEED54F)),
			(FromHex(0x5E42A3), FromHex(0xD6E84D)),
			(FromHex(0x5E42A3), FromHex(0xEEB74F)),
			(FromHex(0x4453A3), FromHex(0xEEDF4F)),
			(FromHex(0x4453A3), FromHex(0xEE9A4F)),
			(FromHex(0x397398), FromHex(0xEEC94F)),
			(FromHex(0x397398), FromHex(0xEE684F)),
			(FromHex(0x329587), FromHex(0xEEBB4F)),
			(FromHex(0x329587), FromHex(0xDC4969)),
			(FromHex(0x39AA6A), FromHex(0xEEAB4F)),
			(FromHex(0x39AA6A), FromHex(0xC54282)),
		};

		private static Color FromHex(int hexRgb)
		{
			var r = (byte) (hexRgb >> 16);
			var g = (byte) (hexRgb >> 8);
			var b = (byte) hexRgb;

			return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
		}

		public static (Color, Color) GetRandomPair()
		{
			return Colors.PickRandom();
		}

		public static Color RandomColor => new Color(Random.Range(0f, 1), Random.Range(0f, 1), Random.Range(0f, 1));
	}
}