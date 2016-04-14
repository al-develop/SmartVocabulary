using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmartVocabulary.Common
{
    public class EnumDescriptionCoverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Enum myEnum = (Enum)value;
            string description = GetEnumDescription(myEnum);
            return description;
        }
                
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }        
    }
}