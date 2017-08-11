﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace FastBuilder.Support
{
	internal class BoolToVisibilityConverter : MarkupExtension, IValueConverter
	{
		public Visibility TrueValue { get; set; } = Visibility.Visible;
		public Visibility FalseValue { get; set; } = Visibility.Collapsed;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? this.FalseValue : (bool)value ? this.TrueValue : this.FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null && (Visibility)value == this.TrueValue;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}
