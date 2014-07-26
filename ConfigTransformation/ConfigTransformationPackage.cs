using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Marazt.ConfigTransformation.Commands;
using Marazt.ConfigTransformation.Helpers;
using Marazt.ConfigTransformation.Logging;
using Microsoft.VisualStudio.Shell;

namespace Marazt.ConfigTransformation
{


    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidConfigTransformationPkgString)]
    //This is needed cause auto-load project to enable correct visibility check
    //http://www.codingodyssey.com/2008/03/22/dynamic-menu-commands-in-visual-studio-packages-part-2/
    //https://github.com/oncheckin/oncheckin-transformer/blob/master/OnCheckinTransformer.VisualStudio/OnCheckinTransforms.VisualStudioPackage.cs
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    //http://msdn.microsoft.com/en-us/library/bb166195.aspx
    //http://msdn.microsoft.com/en-us/library/bb166553.aspx
    //http://code.msdn.microsoft.com/VSSDK-IDE-Sample-Options-f152f574
    [ProvideOptionPageAttribute(typeof(OptionsPage), AppConstants.ConfigTransformation, AppConstants.General, 0, 0, true)]
    public sealed class ConfigTransformationPackage : Package
    {




        #region Properties




        #endregion Properties


        #region Constructors

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require
        /// any Visual Studio service because at this point the package object is created but
        /// not sited yet inside Visual Studio environment. The place to do all the other
        /// initialization is the Initialize method.
        /// </summary>
        // ReSharper disable once EmptyConstructor
        public ConfigTransformationPackage()
        {

        }

        #endregion Constructors


        #region Methods

        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Logger.LogInfo(string.Format(Resources.InitializeOf, Resources.ApplicationCaption));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = DTEHelper.GetServiceIstanceOfInterface<IMenuCommandService, OleMenuCommandService>();
            if (null != mcs)
            {
                //Register commands
                CommandHelper.RegisterCommands(mcs);
            }
        }


        #endregion Package Members


        #endregion Methods

    }
}
