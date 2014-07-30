using System.Linq;
using EnvDTE;

namespace Marazt.ConfigTransformation.Extensions
{
    /// <summary>
    /// Extension class for ProjectItem
    /// </summary>
    internal static class ProjectItemExt
    {
        #region Methods

        /// <summary>
        /// Tries the get property value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns></returns>
        public static TResult TryGetPropertyValue<TResult>(this ProjectItem source, string propertyName, out TResult propertyValue)
        {
            propertyValue = default(TResult);

            var property = source.Properties.Cast<Property>().SingleOrDefault(e => e.Name.Equals(propertyName));

            if (property == null)
            {
                return propertyValue;
            }
            propertyValue = (TResult)property.Value;
            return propertyValue;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Full path of the item</returns>
        public static string GetFullPath(this ProjectItem source)
        {
            return source.GetPropertyValue<string>("FullPath");
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private static TResult GetPropertyValue<TResult>(this ProjectItem source, string propertyName)
        {
            return (TResult)source.Properties.Cast<Property>().First<Property>(e => e.Name.Equals(propertyName)).Value;
        }

        #endregion Methods
    }
}
