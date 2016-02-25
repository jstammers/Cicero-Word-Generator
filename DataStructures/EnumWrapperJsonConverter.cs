using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace DataStructures
{
    public class EnumWrapperJsonConverter: JsonConverter
    {
        private List<Object> objects;
        public EnumWrapperJsonConverter()
        {
            objects = new List<object>();
        }

        public EnumWrapperJsonConverter(List<object> selectableValues)
        {
            this.objects = selectableValues;
        }

        public EnumWrapperJsonConverter(System.Array selectableValues)
        {
            this.objects = new List<object>();
            foreach (object obj in selectableValues)
                this.objects.Add(obj);
        }

        public EnumWrapperJsonConverter(Object[] selectableValues)
        {
            this.objects = new List<object>(selectableValues);
        }

        public TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new TypeConverter.StandardValuesCollection(objects);
        }
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
        public bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            t.WriteTo(writer);
        }
    }
}

