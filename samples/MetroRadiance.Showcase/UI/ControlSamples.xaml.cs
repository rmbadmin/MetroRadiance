﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MetroRadiance.Showcase.UI
{
	partial class ControlSamples
	{
		public ControlSamples()
		{
			this.InitializeComponent();
			this.DataContext = new SampleValues();
		}

		private void HandleBlurWindowButtonClicked(object sender, RoutedEventArgs e)
		{
			new BlurWindowSample().Show();
		}

		private void HandleAcrylicBlurWindowButtonClicked(object sender, RoutedEventArgs e)
		{
			new AcrylicBlurWindowSample().Show();
		}
	}

	public class SampleValues
	{
		public int Int32 { get; set; } = 32;
	}
}
