﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MetroRadiance.Controls
{
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class TabView : ListBox
	{
		static TabView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TabView), new FrameworkPropertyMetadata(typeof(TabView)));
		}

		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);

			foreach (var item in e.RemovedItems.OfType<ITabItem>())
			{
				item.IsSelected = false;
			}
			foreach (var item in e.AddedItems.OfType<ITabItem>())
			{
				item.IsSelected = true;
			}
		}
	}
}
