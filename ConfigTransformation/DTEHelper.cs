using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Marazt.ConfigTransformation
{
    /// <summary>
    /// DTE Helper class
    /// </summary>
    public static class DteHelper
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>DTE instance</returns>
        public static DTE2 GetInstance()
        {
            return (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        /// <summary>
        /// Gets the service istance of interface.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <returns>Instance of service. Can return null</returns>
        public static TInstance GetServiceIstanceOfInterface<TInterface, TInstance>()
            where TInstance : class
        {
            return Package.GetGlobalService(typeof(TInterface)) as TInstance;
        }



        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="optionPageName">Name of the option page.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// Value of the options property
        /// </returns>
        public static T GetPropertyValue<T>(string applicationName, string optionPageName, string propertyName)
        {
            var dte = GetInstance();
            var props = dte.Properties[applicationName, optionPageName];
            var val = props.Item(propertyName).Value;
            if (val == null)
            {
                return default(T);
            }

            return (T)val;
        }

    }
}
