﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MandarinLearner.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetVisibility(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static object GetVisibility(object value)
        {
            if (!(value is bool))
                return Visibility.Collapsed;
            var objValue = (bool) value;
            if (objValue)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
    }
}