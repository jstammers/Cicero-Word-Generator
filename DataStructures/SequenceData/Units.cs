using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;

namespace DataStructures
{
    /// <summary>
    /// This class represents the units applied to a number. It keeps track of both the dimension (ie unity, s, volts, Hz) and a
    /// multiplier (ie M, u, k, etc).
    /// </summary>
    [Serializable, TypeConverter(typeof(ExpandableStructConverter)),JsonObject]
	public struct Units
	{
        public static bool Equivalent(Units a, Units b)
        {
            if (a.dimension != b.dimension)
                return false;
            if (a.multiplier != b.multiplier)
                return false;

            return true;
        }

        [Serializable, TypeConverter(typeof (Dimension.DimensionTypeConverter)),JsonObject]
		public struct Dimension {

            public static bool operator!=(Dimension a, Dimension b) {
                if (a == b)
                    return false;
                return true;
            }

            public static bool operator ==(Dimension a, Dimension b)
            {
                if (a.myDimensionID == b.myDimensionID)
                    return true;
                return false;
            }
            
            public class DimensionTypeConverter : EnumWrapperTypeConverter
            {
                public DimensionTypeConverter() : base(Dimension.allDimensions) 
                {
                }
            }

            private enum DimensionID { unity, s, Vo, Hz, A, Degree };
            [JsonProperty]
			private DimensionID myDimensionID;

   
			private static readonly string[] DimensionString = {
				"", "s", "V", "Hz", "A", "deg"};
    
            private static readonly string[] DimensionFullName = {
                "dimensionless", "seconds", "volts", "hertz", "amps", "degrees"};

            public static readonly Dimension unit = new Dimension(DimensionID.unity);

            public static readonly Dimension sec = new Dimension(DimensionID.s);

            public static readonly Dimension Volt = new Dimension(DimensionID.Vo);

            public static readonly Dimension Hertz = new Dimension(DimensionID.Hz);

            public static readonly Dimension Amp = new Dimension(DimensionID.A);
      
            public static readonly Dimension Deg = new Dimension(DimensionID.Degree);

            /// <summary>
            /// This property gives a list of commonly used multipliers for this dimension type.
            /// This should be used in the UI to give the user a limited number of multipliers to select from
            /// in a combo box, rather than presenting the user with a long list of impractical multipliers for
            /// a particular dimension type (ie Gs, or nHz)
            /// </summary>
            ///
            public Multiplier[] commonlyUsedMultipliers { 
                get 
                {
                    return commonlyUsedMultipliersArray[(int)myDimensionID];
                } 
            }


            /// <summary>
            /// This is a two dimensional array of the multipliers which are commonly or practically useful
            /// for given dimension types. The first array index is the dimension type. For instance, 
            /// commonlyUsedMultipliers[Dimension.v] is an array of the multipliers commonly applied to Volts.
            /// The array is used by the property above.
            /// </summary>

            private static readonly Multiplier[][] commonlyUsedMultipliersArray =
            {   Multiplier.allMultipliers, // unity
                new Multiplier[] {Multiplier.u, Multiplier.m, Multiplier.unity}, // s
                new Multiplier[] {Multiplier.u, Multiplier.m, Multiplier.unity, Multiplier.k}, // V
                new Multiplier[] {Multiplier.unity, Multiplier.k, Multiplier.M, Multiplier.G}, //Hz
                new Multiplier [] {Multiplier.u, Multiplier.m, Multiplier.unity, Multiplier.k}, // A
                new Multiplier [] {Multiplier.unity} // deg
            };

            public static readonly Dimension[] allDimensions = {unit, sec, Volt, Hertz, Amp, Deg};

	        public override string  ToString()
            {
	            return DimensionString[(int)myDimensionID];
            }

            public string toLongString()
            {
                return DimensionFullName[(int)myDimensionID];
            }

			private Dimension(DimensionID dimID) 
			{
				myDimensionID = dimID;
			}
		}

        [Serializable, TypeConverter(typeof (Multiplier.MultiplierTypeConverter)),JsonObject]
        public struct Multiplier
        {

            public class MultiplierTypeConverter : EnumWrapperTypeConverter
            {
                public MultiplierTypeConverter()
                    : base(Multiplier.allMultipliers)
                {
                }
            }

            private enum MultiplierID { na, um, mi, unit, ki, Me, Gi };
     
            private static readonly double[] MultiplierValue = { .000000001, .000001, .001, 1, 1000, 1000000, 1000000000 };
     
            private static readonly string[] MultiplierString = { "n", "u", "m", "", "k", "M", "G" };

            private static readonly string[] MultiplierFullName = {"nano", "micro", "milli", "", "kilo", "mega", "giga"};

   
            public static readonly Multiplier n = new Multiplier(MultiplierID.na);
      
            public static readonly Multiplier u = new Multiplier(MultiplierID.um);
 
            public static readonly Multiplier m = new Multiplier(MultiplierID.mi);
      
            public static readonly Multiplier unity = new Multiplier(MultiplierID.unit);

            public static readonly Multiplier k = new Multiplier(MultiplierID.ki);

            public static readonly Multiplier M = new Multiplier(MultiplierID.Me);

            public static readonly Multiplier G = new Multiplier(MultiplierID.Gi);


            public static readonly Multiplier[] allMultipliers = {n, u, m, unity, k, M, G};
            [JsonProperty]
            private MultiplierID myMultiplierID;

            private Multiplier(MultiplierID multID) {
                this.myMultiplierID = multID;
            }


            public override string  ToString()
            {
 	            return MultiplierString[(int)myMultiplierID];
            }

            public string toLongString()
            {
                return MultiplierFullName[(int)myMultiplierID];
            }

            public double getMultiplierFactor() 
            {
                return MultiplierValue[(int)myMultiplierID];
            }

            /// <summary>
            /// This is an implicit typecast from Multiplier to double, allowing for expressions like
            /// double a = number * Multiplier.M
            /// </summary>
            /// <param name="mul"></param>
            /// <returns></returns>
            static public implicit operator double(Multiplier mul)
            {
                return mul.getMultiplierFactor();
            }

        }


        private Dimension myDimension;
        [JsonProperty]
        public Dimension dimension
        {
            get { return myDimension; }
            set { myDimension = value; }
        }
        private Multiplier myMultiplier;
        [JsonProperty]
        public Multiplier multiplier
        {
            get { return myMultiplier; }
            set { myMultiplier = value; }
        }

        public override string  ToString()
        {
 	        return myMultiplier.ToString() + myDimension.ToString();
        }

        public string toLongString()
        {
            return myMultiplier.toLongString() + myDimension.toLongString();
        }

        /// <summary>
        /// This constructor attempts to construct a Unit object with the same long or short name as the given string.
        /// (ie it will parse strings like "mHz", "microvolts", "gigaunity", "M", "ks"). The constructor is case sensitive.
        /// If no matching unit type is found, the constructor throws an exception.
        /// </summary>
        /// <param name="unitString"></param>
        public Units(string unitString)
        {
            Units testUnit = new Units();
            foreach (Dimension dim in Dimension.allDimensions)
                foreach (Multiplier mul in Multiplier.allMultipliers)
                {
                    testUnit.myDimension = dim;
                    testUnit.myMultiplier = mul;
                    if ((unitString == testUnit.ToString()) || (unitString == testUnit.toLongString()))
                    {
                        myDimension = dim;
                        myMultiplier = mul;

                        return;
                    }
                }

            throw new Exception("Unrecognized Unit type " + unitString + " passed to Units(string unitString) constructor.");
        }
        [JsonConstructor]
        public Units(Dimension dimension, Multiplier multiplier) : this()
        {
            this.dimension = dimension;
            this.multiplier = multiplier;
        }

        [JsonIgnore]
        public static readonly Units V = new Units(Dimension.Volt, Multiplier.unity);
        [JsonIgnore]
        public static readonly Units Hz = new Units(Dimension.Hertz, Multiplier.unity);
        [JsonIgnore]
        public static readonly Units s = new Units(Dimension.sec, Multiplier.unity);
        [JsonIgnore]
        public static readonly Units A = new Units(Dimension.Amp, Multiplier.unity);
        [JsonIgnore]
        public static readonly Units deg = new Units(Dimension.Deg, Multiplier.unity);

	}
}
