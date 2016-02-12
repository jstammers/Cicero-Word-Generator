using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;
namespace DataStructures
{
    [Serializable, TypeConverter(typeof(ExpandableObjectConverter)),JsonObject]
    public class FpgaRunReport
    {
        public FpgaRunReport() { }

        public UInt32 retriggerWaitedSamples;
        public UInt16 retriggerTimeoutCount;
    }
}
