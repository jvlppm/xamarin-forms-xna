namespace Sample.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Xamarin.Forms;

    public class Setter
    {
        public string Property { get; set; }
        public object Value { get; set; }
    }

    [ContentProperty("Children")]
    public class Style
    {
        const BindingFlags BindablePropertyFlags = BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Static;

        public Style()
        {
            Children = new List<Setter>();
        }

        public IList<Setter> Children { get; private set; }

        public static readonly BindableProperty StyleProperty =
            BindableProperty.CreateAttached<Style, Style>(bindable => GetStyle(bindable), default(Style),
                propertyChanged: (bindable, oldvalue, newvalue) =>
                {
                    foreach (var setter in newvalue.Children)
                        SetValue(bindable, setter);
                });

        static void SetValue(BindableObject bindable, Setter setter)
        {
            var binding = setter.Value as Binding;
            if (binding != null)
            {
                var prop = (BindableProperty)bindable.GetType()
                    .GetField(setter.Property + "Property", BindablePropertyFlags)
                    .GetValue(null);
                bindable.SetBinding(prop, new Binding(binding.Path, binding.Mode, binding.Converter, binding.ConverterParameter, binding.StringFormat));
                return;
            }

            var pInfo = bindable.GetType().GetTypeInfo().GetRuntimeProperty(setter.Property);
            pInfo.SetMethod.Invoke(bindable, new[] { ConvertValue(pInfo, (string)setter.Value) });
        }

        static object ConvertValue(PropertyInfo pInfo, string value)
        {
            var converterInfo = pInfo.PropertyType.GetCustomAttribute<TypeConverterAttribute>();
            if (converterInfo != null)
            {
                var converterType = Type.GetType(converterInfo.ConverterTypeName);
                var conv = (TypeConverter)Activator.CreateInstance(converterType);
                return conv.ConvertFrom(value);
            }

            if (pInfo.PropertyType.IsEnum)
                return Enum.Parse(pInfo.PropertyType, value);

            return Convert.ChangeType(value, pInfo.PropertyType.GetTypeInfo());
        }

        public static Style GetStyle(BindableObject bindable)
        {
            return (Style)bindable.GetValue(StyleProperty);
        }

        public static void SetStyle(BindableObject bindable, Style value)
        {
            bindable.SetValue(StyleProperty, value);
        }
    }
}