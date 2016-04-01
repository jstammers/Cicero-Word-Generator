using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStructures
{
    public class SerializableExpandableObjectConverter:ExpandableObjectConverter
    {
        //This class inherits from the default ExpandableObjectConverter but prevents serialization of objects to a string. Json.NET cannot convert from a string to other data types, so this should prevent that from happening
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
                return false;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(String))
                return false;
            else
                return base.CanConvertFrom(context, sourceType);
        }
    }

    public class SerializableExpandableStructConverter:ExpandableStructConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
                return false;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(String))
                return false;
            else
                return base.CanConvertFrom(context, sourceType);
        }
    }
}
