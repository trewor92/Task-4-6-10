﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflection.Tasks
{
    public static class CommonTasks
    {

        /// <summary>
        /// Returns the lists of public and obsolete classes for specified assembly.
        /// Please take attention: classes (not interfaces, not structs)
        /// </summary>
        /// <param name="assemblyName">name of assembly</param>
        /// <returns>List of public but obsolete classes</returns>
        public static IEnumerable<string> GetPublicObsoleteClasses(string assemblyName) {
            Assembly assembly = Assembly.Load(assemblyName);
            return assembly.GetTypes().Where(x => x.IsClass && x.IsPublic && x.GetCustomAttribute<ObsoleteAttribute>() != null).Select(x => x.Name);
        }

        /// <summary>
        /// Returns the value for required property path
        /// </summary>
        /// <example>
        ///  1) 
        ///  string value = instance.GetPropertyValue("Property1")
        ///  The result should be equal to invoking statically
        ///  string value = instance.Property1;
        ///  2) 
        ///  string name = instance.GetPropertyValue("Property1.Property2.FirstName")
        ///  The result should be equal to invoking statically
        ///  string name = instance.Property1.Property2.FirstName;
        /// </example>
        /// <typeparam name="T">property type</typeparam>
        /// <param name="obj">source object to get property from</param>
        /// <param name="propertyPath">dot-separated property path</param>
        /// <returns>property value of obj for required propertyPath</returns>
        public static T GetPropertyValue<T>(this object obj, string propertyPath) {
            return (T)propertyPath.Split('.').Aggregate(obj, (y, x) => y= y.GetType().GetProperty(x).GetValue(y,null));
        }


        /// <summary>
        /// Assign the value to the required property path
        /// </summary>
        /// <example>
        ///  1)
        ///  instance.SetPropertyValue("Property1", value);
        ///  The result should be equal to invoking statically
        ///  instance.Property1 = value;
        ///  2)
        ///  instance.SetPropertyValue("Property1.Property2.FirstName", value);
        ///  The result should be equal to invoking statically
        ///  instance.Property1.Property2.FirstName = value;
        /// </example>
        /// <param name="obj">source object to set property to</param>
        /// <param name="propertyPath">dot-separated property path</param>
        /// <param name="value">assigned value</param>
        public static void SetPropertyValue(this object obj, string propertyPath, object value) {            
            IEnumerable<string> propertyArray = propertyPath.Split('.');
            string last = propertyArray.Last();
            object targetObj = propertyArray
                .Take(propertyArray.Count() - 1)?
                .Aggregate(obj, (y, x) => y = y.GetType()
                .GetProperty(x)
                .GetValue(y, null)) ?? obj;
            targetObj.GetType().GetProperty(last).DeclaringType.GetProperty(last).SetValue(targetObj, value);
        }
    }
}
