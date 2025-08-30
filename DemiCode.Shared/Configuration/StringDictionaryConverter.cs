using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Xml;

namespace DemiCode.Configuration
{
    /// <summary>
    /// Converter class for <see cref="Dictionary{TKey,TValue}"/> with key and value of type <see cref="String"/>.
    /// </summary>
    public class StringDictionaryConverter : ConfigurationConverterBase
    {
        private static bool ValidateType(object value, Type expected)
        {
            if ((value != null) && (value.GetType() != expected))
                return false;

            return true;
        }

        ///<summary>
        ///Determines whether the conversion is allowed.
        ///</summary>
        ///
        ///<returns>
        ///true if the conversion is allowed; otherwise, false. 
        ///</returns>
        ///
        ///<param name="type">The type to convert to.</param>
        ///<param name="ctx">The <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> object used for type conversion.</param>
        public override bool CanConvertTo(ITypeDescriptorContext ctx, Type type)
        {
            return (type == typeof(string));
        }

        ///<summary>
        ///Determines whether the conversion is allowed.
        ///</summary>
        ///
        ///<returns>
        ///true if the conversion is allowed; otherwise, false.
        ///</returns>
        ///
        ///<param name="type">The <see cref="T:System.Type"></see> to convert from.</param>
        ///<param name="ctx">The <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> object used for type conversions.</param>
        public override bool CanConvertFrom(ITypeDescriptorContext ctx, Type type)
        {
            return (type == typeof(string));
        }

        ///<summary>
        ///Converts the given value object to the specified type, using the specified context and culture information.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Object"></see> that represents the converted value.
        ///</returns>
        ///
        ///<param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed. </param>
        ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
        ///<param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to. </param>
        ///<param name="value">The <see cref="T:System.Object"></see> to convert. </param>
        ///<exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        ///<exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return null;

            if (!ValidateType(value, typeof(Dictionary<string, string>)))
                throw GetConvertToException(value, typeof(Dictionary<string, string>));

            var data = (Dictionary<string, string>)value;
            var doc = new XmlDocument();
            var top = doc.CreateElement("top");
            doc.AppendChild(top);

            foreach (var pair in data)
            {
                var dt = doc.CreateElement("dt");
                dt.SetAttribute("k", pair.Key);
                dt.SetAttribute("v", pair.Value);
                top.AppendChild(dt);
            }

            return doc.OuterXml;
        }

        ///<summary>
        ///Converts the given object to the type of this converter, using the specified context and culture information.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Object"></see> that represents the converted value.
        ///</returns>
        ///
        ///<param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture. </param>
        ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
        ///<param name="value">The <see cref="T:System.Object"></see> to convert. </param>
        ///<exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var xml = (string)value;

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            if (doc.DocumentElement == null)
                throw GetConvertFromException(value);

            var dict = new Dictionary<string, string>(doc.DocumentElement.ChildNodes.Count);
            foreach (var childNode in doc.DocumentElement.ChildNodes)
            {
                var dt = (XmlElement)childNode;
                dict.Add(dt.GetAttribute("k"), dt.GetAttribute("v"));
            }
            return dict;
        }


    }
}
