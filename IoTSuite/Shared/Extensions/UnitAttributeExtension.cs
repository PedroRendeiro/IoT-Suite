using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IoTSuite.Shared.Extensions
{
    /// <summary>
    /// This attribute is used to represent a unit
    /// for a enum.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class UnitAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Holds the Unit for a value in an enum.
        /// </summary>
        public string Unit { get; protected set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor used to init a Unit Attribute
        /// </summary>
        /// <param name="value"></param>
        public UnitAttribute(string unit)
        {
            Unit = unit;
        }
        #endregion
    }

    public static class UnitExtension
    {
        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the Unit attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetUnit(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            UnitAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(UnitAttribute), false) as UnitAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].Unit : value.ToString();
        }

        /// <summary>
        /// Will get all the string values for a given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetUnits(this Type type)
        {
            if (type == null) throw new NullReferenceException();

            Dictionary<string, string> result = new Dictionary<string, string>();

            FieldInfo[] fields = type.GetFields();
            // Get the stringvalue attributes
            fields.ToList().ForEach(field =>
            {
                // Get the stringvalue attributes
                UnitAttribute[] attribs = field.GetCustomAttributes(
                    typeof(UnitAttribute), false) as UnitAttribute[];

                // Return the first if there was a match.
                if (attribs.Length > 0)
                {
                    result.Add(field.Name ,attribs[0].Unit);
                }
            });
            
            PropertyInfo[] properties = type.GetProperties();
            // Get the stringvalue attributes
            properties.ToList().ForEach(property =>
            {
                // Get the stringvalue attributes
                UnitAttribute[] attribs = property.GetCustomAttributes(
                    typeof(UnitAttribute), false) as UnitAttribute[];

                // Return the first if there was a match.
                if (attribs.Length > 0)
                {
                    result.Add(property.Name, attribs[0].Unit);
                }
            });

            return result;
        }
    }
}
