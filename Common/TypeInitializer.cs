using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class TypeInitializer
    {
        private const string ConfigurationError = "Failed to initialize {0}, unable to load type '{1}'";

        #region GetTypeOrDefault

        /// <summary>
        /// Gets the type or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDefault">The type of the default.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="objName">Name of the obj.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static T GetTypeOrDefault<T, TDefault>(TypeElement type, string objName, params object[] args) where TDefault : T, new()
        {
            return type != null ? GetTypeOrDefault<T, TDefault>(type.ClassType, objName, args) : new TDefault();
        }

        /// <summary>
        /// Gets the type or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDefault">The type of the default.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="objName">Name of the obj.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static T GetTypeOrDefault<T, TDefault>(string type, string objName, params object[] args) where TDefault : T, new()
        {
            return string.IsNullOrEmpty(type) ? new TDefault() : GetType<T>(type, objName, args);
        }

        /// <summary>
        /// Gets the type or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDefault">The type of the default.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="generic">The generic.</param>
        /// <param name="objName">Name of the obj.</param>
        /// <returns></returns>
        public static T GetTypeOrDefault<T, TDefault>(TypeElement type, Type generic, string objName) where TDefault : T, new()
        {
            if (type == null || string.IsNullOrEmpty(type.ClassType))
                return new TDefault();
            return GetType<T>(type, generic, objName);
        }

        #endregion

        #region GetType

        /// <summary>
        /// Gets the type from.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="type">The type.</param>
        /// <param name="objName">Name of the obj.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static T GetTypeFrom<T>(string assemblyPath, string type, string objName, params object[] args)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentException("Parameter 'assemblyPath' could not be empty.'");

            if (string.IsNullOrEmpty(type))
                throw new ArgumentException("Parameter 'type' could not be empty.'");

            var ass = Assembly.LoadFrom(assemblyPath);
            object obj;
            try
            {
                obj = ass.CreateInstance(type, false, BindingFlags.CreateInstance, null, args, null, null);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, ConfigurationError, objName, type), ex);
            }

            if (obj == null)
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, ConfigurationError, objName, type));

            return (T)obj;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="objName">Name of the obj.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static T GetType<T>(TypeElement type, string objName, params object[] args)
        {
            if (type == null)
                throw new ArgumentException("Parameter 'type' could not be null.'");

            return GetType<T>(type.ClassType, type.Method, objName, args);
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="objName">Name of the obj.</param>
        /// <returns></returns>
        public static object GetType(string type, string objName)
        {
            return GetType<object>(type, objName);
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="objName">Name of the obj.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static T GetType<T>(string type, string objName, params object[] args)
        {
            return GetType<T>(type, null, objName, args);
        }


        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeInfo">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="objName">Name of the obj.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static T GetType<T>(string typeInfo, string methodName, string objName, params object[] args)
        {
            if (typeInfo == null)
                throw new ArgumentException("Parameter 'type' could not be null.'");

            Type type;

            var genericIndex = typeInfo.LastIndexOf("]]", StringComparison.Ordinal);
            if (genericIndex > 0)
            {
                int index = typeInfo.IndexOf(',', genericIndex);
                var assembly = Assembly.Load(typeInfo.Substring(index + 1));
                var typeName = typeInfo.Substring(0, index);

                type = assembly.GetType(typeName);
            }
            else
            {
                type = Type.GetType(typeInfo);
            }

            if (type == null)
            {
                throw new ArgumentException(String.Format("Unrecognized type: '{0}'", typeInfo));
            }

            object obj;
            try
            {
                obj = Activator.CreateInstance(type, BindingFlags.CreateInstance, null, string.IsNullOrEmpty(methodName) ? args : null, null, null);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(
                    string.Format(CultureInfo.InvariantCulture, ConfigurationError, objName, typeInfo), ex);
            }

            if (obj == null)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, ConfigurationError,
                                                                     objName, typeInfo));
            }

            try
            {
                if (!string.IsNullOrEmpty(methodName))
                    return (T)obj.GetType().InvokeMember(methodName,
                                                          BindingFlags.Instance | BindingFlags.Public |
                                                          BindingFlags.InvokeMethod, null, obj, args);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(
                    string.Format(CultureInfo.InvariantCulture, ConfigurationError, objName, typeInfo), ex);
            }


            return (T)obj;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="generic">The generic.</param>
        /// <param name="objName">Name of the obj.</param>
        /// <returns></returns>
        public static T GetType<T>(TypeElement type, Type generic, string objName)
        {
            if (type == null)
                throw new ArgumentException("Parameter 'type' could not be null.'");

            if (generic == null)
                throw new ArgumentException("Parameter 'genereic' could not be null.'");

            var genericIndex = type.ClassType.LastIndexOf("]]", StringComparison.Ordinal);
            if (genericIndex < 0)
                genericIndex = 0;

            var index = type.ClassType.IndexOf(',', genericIndex);
            var assembly = Assembly.Load(type.ClassType.Substring(index + 1));

            object obj;
            try
            {
                obj = assembly.CreateInstance(string.Format(CultureInfo.InvariantCulture, "{0}`1[[{1}]]",
                                                          type.ClassType.Substring(0, index),
                                                          generic.AssemblyQualifiedName));
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, ConfigurationError, objName, type), ex);
            }

            if (obj == null)
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, ConfigurationError, objName, type.ClassType));

            return (T)obj;
        }


        public static Type LoadType(string typeInfo, string objName)
        {
            if (typeInfo == null)
                throw new ArgumentException("Parameter 'type' could not be null.'");

            var genericIndex = typeInfo.LastIndexOf("]]", StringComparison.Ordinal);
            if (genericIndex < 0)
                genericIndex = 0;

            var index = typeInfo.IndexOf(',', genericIndex);

            var assembly = Assembly.Load(typeInfo.Substring(index + 1));

            var typeName = typeInfo.Substring(0, index);


            var type = assembly.GetType(typeName);

            if (type == null)
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, ConfigurationError, objName, typeName));


            return type;
        }

        public static Type LoadType(TypeElement type, string objName)
        {
            if (type == null)
                throw new ArgumentException("Parameter 'type' could not be null.'");

            return LoadType(type.ClassType, objName);
        }

        #endregion
    }
}
