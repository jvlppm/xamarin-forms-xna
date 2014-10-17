namespace Sample.Converters
{
    using Sample.Controls;
    using System;
    using Xamarin.Forms;

    public class ButtonImageConverter : IValueConverter
    {
        public string ColorName { get; set; }
        public bool Wide { get; set; }

        public ButtonImageConverter()
        {
            ColorName = "grey";
            Wide = true;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string state = (ImageButtonState)value == ImageButtonState.Normal ? "_normal" : "_over";
            string size = Wide ? "_wide" : "_narrow";

            return "button_" + ColorName + size + state;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
