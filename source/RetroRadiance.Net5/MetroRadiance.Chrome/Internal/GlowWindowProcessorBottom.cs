﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MetroRadiance.Core.Win32;
#pragma warning disable 612

namespace MetroRadiance.Chrome.Internal
{
	internal class GlowWindowProcessorBottom : GlowWindowProcessor
	{
		public override Orientation Orientation => Orientation.Horizontal;

		public override HorizontalAlignment HorizontalAlignment => HorizontalAlignment.Stretch;

		public override VerticalAlignment VerticalAlignment => VerticalAlignment.Top;

		public override double GetLeft(double ownerLeft, double ownerWidth)
		{
			return ownerLeft;
		}

		public override double GetTop(double ownerTop, double ownerHeight)
		{
			return ownerTop + ownerHeight;
		}

		public override double GetWidth(double ownerLeft, double ownerWidth)
		{
			return ownerWidth;
		}

		public override double GetHeight(double ownerTop, double ownerHeight)
		{
			return GlowSize;
		}

		public override HitTestValues GetHitTestValue(Point point, double actualWidht, double actualHeight)
		{
			var bottomLeft = new Rect(0, 0, EdgeSize - GlowSize, actualHeight);
			var bottomRight = new Rect(actualWidht - EdgeSize + GlowSize, 0, EdgeSize - GlowSize, actualHeight);

			return bottomLeft.Contains(point)
				? HitTestValues.HTBOTTOMLEFT
				: bottomRight.Contains(point) ? HitTestValues.HTBOTTOMRIGHT : HitTestValues.HTBOTTOM;
		}

		public override Cursor GetCursor(Point point, double actualWidht, double actualHeight)
		{
			var bottomLeft = new Rect(0, 0, EdgeSize - GlowSize, actualHeight);
			var bottomRight = new Rect(actualWidht - EdgeSize + GlowSize, 0, EdgeSize - GlowSize, actualHeight);

			return bottomLeft.Contains(point)
				? Cursors.SizeNESW
				: bottomRight.Contains(point) ? Cursors.SizeNWSE : Cursors.SizeNS;
		}
	}
}
