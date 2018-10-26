﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Helpers
{
    /// <summary>
    /// Property Helper
    /// </summary>
    public static class PropertyHelper
    {
        /// <summary>
        /// Get single property details by property name
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyDetails GetIndividualPropertyDetailsByPropertyName<TObject>(this TObject obj, string propertyName)
            where TObject : class, new()
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrWhiteSpace(propertyName) == true)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                if (propertyInfo.Name.ToLowerInvariant() == propertyName.Trim().ToLowerInvariant())
                {
                    var value = propertyInfo.GetValue(obj);
                    return new PropertyDetails
                    {
                        PropertyInfo = propertyInfo,
                        Value = value
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Get single property details from attibute
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static PropertyDetails<TAttribute> GetIndividualPropertyDetailsByAttribute<TObject, TAttribute>(this TObject obj)
            where TObject : class, new()
            where TAttribute : Attribute
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                TAttribute[] attributes = (TAttribute[])propertyInfo.GetCustomAttributes(typeof(TAttribute), true);

                if (attributes != null & attributes.Any())
                {
                    var value = propertyInfo.GetValue(obj);
                    return new PropertyDetails<TAttribute>
                    {
                        Attributes = attributes,
                        PropertyInfo = propertyInfo,
                        Value = value
                    };
                }
            }

            return null;
        }
    }
}
