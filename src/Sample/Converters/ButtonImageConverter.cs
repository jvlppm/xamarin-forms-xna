namespace Sample.Converters
{
    using Sample.Controls;
    using System;
    using Xamarin.Forms;

    public class ButtonImageConverter : IValueConverter
    {
        public string ColorName { get; set; }

        public ButtonImageConverter()
        {
            ColorName = "blue";
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string state = GetStateSuffix((ImageButtonState)value);
            return "button_" + ColorName + state + ".9";
        }

        string GetStateSuffix(ImageButtonState value)
        {
            switch (value)
            {
                case ImageButtonState.Over: return "_over";
                case ImageButtonState.Pressed:
                case ImageButtonState.Pressing: return "_pressed";
                default: return "_normal";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
