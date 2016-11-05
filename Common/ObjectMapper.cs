using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectMapper
    {
        /// <summary>
        /// Maps the specified s.
        /// </summary>
        /// <typeparam name="TS">The type of the S.</typeparam>
        /// <typeparam name="TD">The type of the D.</typeparam>
        /// <param name="s">The s.</param>
        /// <param name="d">The d.</param>
        /// <param name="changes">The changes.</param>
        public static void Map<TS, TD>(TS s, TD d, out PropertyChange[] changes)
        {
            ObjectMapperT<TS, TD>.Apply(s, d, out changes);
        }

        /// <summary>
        /// Maps the specified s.
        /// </summary>
        /// <typeparam name="TS">The type of the S.</typeparam>
        /// <typeparam name="TD">The type of the D.</typeparam>
        /// <param name="s">The s.</param>
        /// <param name="d">The d.</param>
        public static void Map<TS, TD>(TS s, TD d)
        {
            PropertyChange[] pcs;
            ObjectMapperT<TS, TD>.Apply(s, d, out pcs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        internal class ObjectMapperT<TSource, TDestination>
        {
            private static readonly IList<PropertyMatch> PropertyMatches = new List<PropertyMatch>();

            /// <summary>
            /// Fills the property matches.
            /// </summary>
            public static void FillPropertyMatches()
            {
                lock (PropertyMatches)
                {
                    PropertyMatches.Clear();
                    var sourceProps = typeof(TSource).GetProperties();
                    var destinationProps = typeof(TDestination).GetProperties();

                    foreach (var sourceProp in sourceProps)
                    {
                        var sourceType = sourceProp.PropertyType;
                        var sourceTypeIsEnum = (sourceProp.PropertyType.IsGenericType
                                                && sourceProp.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                                && sourceProp.PropertyType.GetGenericArguments()[0].IsEnum) || sourceProp.PropertyType.IsEnum;



                        foreach (var destinationProp in destinationProps)
                        {
                            var destinationType = destinationProp.PropertyType;
                            var destinationTypeIsEnum = (destinationProp.PropertyType.IsGenericType
                                                && destinationProp.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                                && destinationProp.PropertyType.GetGenericArguments()[0].IsEnum) || destinationProp.PropertyType.IsEnum;

                            if (sourceProp.Name.Equals(destinationProp.Name, StringComparison.InvariantCultureIgnoreCase)
                                && (sourceType.Equals(destinationType)
                                || sourceTypeIsEnum && destinationTypeIsEnum)
                                && sourceProp.GetCustomAttributes(typeof(ReadOnlyAttribute), true).Length == 0
                                /*and writeonly*/ //|| CheckNullableEnumType(sourceProp.PropertyType, destinationProp.PropertyType).HasValue)
                            )
                                PropertyMatches.Add(new PropertyMatch(sourceProp, destinationProp));
                        }
                    }
                }
            }

            static ObjectMapperT()
            {
                FillPropertyMatches();
            }

            /// <summary>
            /// Applies the specified source.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="destination">The destination.</param>
            /// <param name="changes">The changes.</param>
            public static void Apply(TSource source, TDestination destination, out PropertyChange[] changes)
            {

                if (source == null)
                    throw new ArgumentNullException("source", "source cannot be null");

                var pcs = new List<PropertyChange>();

                foreach (var propertyMatch in PropertyMatches)
                {
                    var sourceVal = propertyMatch.SourceProperty.GetValue(source, null);
                    var destVal = propertyMatch.DestinationProperty.GetValue(destination, null);

                    if (!object.Equals(sourceVal, destVal))
                    {
                        var destinationTypeIsEnum = (propertyMatch.DestinationProperty.PropertyType.IsGenericType
                                                && propertyMatch.DestinationProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                                && propertyMatch.DestinationProperty.PropertyType.GetGenericArguments()[0].IsEnum) || propertyMatch.DestinationProperty.PropertyType.IsEnum;
                        var destinationTypeIsNullable = propertyMatch.DestinationProperty.PropertyType.IsGenericType && propertyMatch.DestinationProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

                        var sourceTypeIsEnum = (propertyMatch.SourceProperty.PropertyType.IsGenericType
                                                && propertyMatch.SourceProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                                                && propertyMatch.SourceProperty.PropertyType.GetGenericArguments()[0].IsEnum) || propertyMatch.SourceProperty.PropertyType.IsEnum;
                        var sourceTypeIsNullable = propertyMatch.SourceProperty.PropertyType.IsGenericType && propertyMatch.SourceProperty.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

                        if (destinationTypeIsEnum && sourceTypeIsEnum)
                        {
                            if (sourceTypeIsNullable && !destinationTypeIsNullable)
                            {
                                if ((sourceVal == null || (int)sourceVal == 0))
                                    propertyMatch.DestinationProperty.SetValue(destination, 0, null);
                                else
                                    propertyMatch.DestinationProperty.SetValue(destination, sourceVal, null);
                            }
                            else if (/*!sourceTypeIsNullable &&*/ destinationTypeIsNullable)
                            {
                                if ((sourceVal == null || (int)sourceVal == 0))
                                    propertyMatch.DestinationProperty.SetValue(destination, null, null);
                                else
                                {
                                    var value = Enum.ToObject(System.Nullable.GetUnderlyingType(propertyMatch.DestinationProperty.PropertyType), (int)sourceVal);
                                    var value1 = System.Activator.CreateInstance(propertyMatch.DestinationProperty.PropertyType, value);
                                    propertyMatch.DestinationProperty.SetValue(destination, value1, null);
                                }
                            }
                            else
                                propertyMatch.DestinationProperty.SetValue(destination, sourceVal, null);
                        }
                        else
                            propertyMatch.DestinationProperty.SetValue(destination, sourceVal, null);

                        if (!(destinationTypeIsEnum && sourceTypeIsEnum && Convert.ToInt32(sourceVal) == Convert.ToInt32(destVal)))
                            pcs.Add(new PropertyChange() { Property = propertyMatch.DestinationProperty, PreviousValue = destVal, NewValue = sourceVal });
                    }
                }
                changes = pcs.ToArray();

            }

            public struct PropertyMatch
            {
                /// <summary>
                /// 
                /// </summary>
                public PropertyInfo SourceProperty;
                /// <summary>
                /// 
                /// </summary>
                public PropertyInfo DestinationProperty;

                /// <summary>
                /// Initializes a new instance of the <see cref="ObjectMapperT&lt;TSource, TDestination&gt;.PropertyMatch"/> struct.
                /// </summary>
                /// <param name="sourceProp">The source prop.</param>
                /// <param name="destinationProp">The destination prop.</param>
                public PropertyMatch(PropertyInfo sourceProp, PropertyInfo destinationProp)
                {
                    SourceProperty = sourceProp;
                    DestinationProperty = destinationProp;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct PropertyChange
    {
        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public PropertyInfo Property { get; set; }
        /// <summary>
        /// Gets or sets the previous value.
        /// </summary>
        /// <value>
        /// The previous value.
        /// </value>
        public object PreviousValue { get; set; }
        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>
        /// The new value.
        /// </value>
        public object NewValue { get; set; }
    }
}
