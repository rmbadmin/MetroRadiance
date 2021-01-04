﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MetroRadiance.Media
{
	public static class ColorHelper
	{
		/// <summary>
		/// 現在の RGB 色空間による色から HSV 色空間の <see cref="HsvColor"/> 構造体を作成します。
		/// </summary>
		public static HsvColor ToHsv(this Color c)
		{
			return HsvColor.FromRgb(c);
		}

		public static uint GetColorAsUInt32(Color color)
		{
			return ((uint)color.A << 24) | ((uint)color.B << 16) | ((uint)color.G << 8) | color.R;
		}

		public static Color GetColorFromUInt32(uint color)
		{
			return Color.FromArgb((byte)(color >> 24), (byte)(color >> 16), (byte)(color >> 8), (byte)color);
		}
	}
}
