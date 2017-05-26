using System.Linq;
using EnvDTE;

namespace Marazt.ConfigTransformation.Extensions
{
    /// <summary>
    /// Extension class for ProjectItem
    /// </summary>
    internal static class ProjectItemExt
    {
        #region Constants

        /// <summary>
        /// The full path property
        /// </summary>
        public const string FullPathProperty = "FullPath";

        /// <summary>
        /// The item type property
        /// </summary>
        public const string ItemTypeProperty = "ItemType";

        #endregion Constants


        #region Methods

        /// <summary>
        /// Tries the get property value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>[True] if proerty was found, otherwise [False]</returns>
        public static bool TryGetPropertyValue<TResult>(this ProjectItem source, string propertyName, out TResult propertyValue)
        {
            propertyValue = default(TResult);

            var property = source.Properties?.Cast<Property>().SingleOrDefault(e => e.Name.Equals(propertyName));

            if (property == null)
            {
                return false;
            }

            propertyValue = (TResult)property.Value;
            return true;
        }

        /// <summary>
        /// Tries the set property value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>[True] if proerty was found, otherwise [False]</returns>
        public static bool TrySetPropertyValue<TResult>(this ProjectItem source, string propertyName, TResult propertyValue)
        {
            if (source.Properties == null)
            {
                return false;
            }

            var property = source.Properties.Cast<Property>().SingleOrDefault(e => e.Name.Equals(propertyName));

            if (property == null)
            {
                return false;
            }

            property.Value = propertyValue;

            return true;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Full path of the item</returns>
        public static string GetFullPath(this ProjectItem source)
        {
            return source.GetPropertyValue<string>(FullPathProperty);
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
            return (TResult)source.Properties.Cast<Property>().First(e => e.Name.Equals(propertyName)).Value;
        }

        #endregion Methods
    }
}
