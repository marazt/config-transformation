using System;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
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
    [ProvideOptionPageAttribute(typeof(OptionsPage), ConfigTransformation, General, 0, 0, true)]
    public sealed class ConfigTransformationPackage : Package
    {

        #region Constants

        /// <summary>
        /// The configuration transformation
        /// </summary>
        private const string ConfigTransformation = "Config Transformation";

        /// <summary>
        /// The general
        /// </summary>
        private const string General = "General";

        #endregion Constants


        #region Properties

        /// <summary>
        /// The transformation provider
        /// </summary>
        private readonly TransformationProvider transformationProvider;

        private IVsDifferenceService diffService;

        #endregion Properties


        #region Constructors

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require
        /// any Visual Studio service because at this point the package object is created but
        /// not sited yet inside Visual Studio environment. The place to do all the other
        /// initialization is the Initialize method.
        /// </summary>
        public ConfigTransformationPackage()
        {
            this.transformationProvider = new TransformationProvider();
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
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for transfomration
                var transformMenuCommandID = new CommandID(GuidList.guidConfigTransformationCmdSet, (int)PkgCmdIDList.cmdidCtxMenuTransformItem);
                var transformMenuItem = new OleMenuCommand(TransformMenuItemCallback, transformMenuCommandID);
                transformMenuItem.BeforeQueryStatus += MenuTransformationCommandsBeforeQueryStatus;
                mcs.AddCommand(transformMenuItem);


                // Create the command for comparison
                var comparisonMenuCommandID = new CommandID(GuidList.guidConfigTransformationCmdSet, (int)PkgCmdIDList.cmdidCtxMenuComparisonItem);
                var comparisonMenuItem = new OleMenuCommand(CompareFilesMenuItemCallback, comparisonMenuCommandID);
                comparisonMenuItem.BeforeQueryStatus += MenuTransformationCommandsBeforeQueryStatus;
                mcs.AddCommand(comparisonMenuItem);

            }

            this.diffService = GetService(typeof(SVsDifferenceService)) as IVsDifferenceService;
        }

        /// <summary>
        /// Handles the BeforeQueryStatus event of the menuTransformationCommands control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MenuTransformationCommandsBeforeQueryStatus(object sender, EventArgs e)
        {


            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {

                // start by assuming that the menu will not be shown
                menuCommand.Visible = false;
                menuCommand.Enabled = false;

                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;

                if (!IsSingleProjectItemSelection(out hierarchy, out itemid))
                {
                    return;
                }


                var vsProject = (IVsProject)hierarchy;
                if (!ProjectSupportsTransforms(vsProject))
                {
                    return;
                }

                var fileName = GetFileNameFromItem(vsProject, itemid);
                if (fileName == null || !IsTransformationFile(fileName))
                {
                    return;
                }

                menuCommand.Visible = true;
                menuCommand.Enabled = true;
            }
        }


        /// <summary>
        /// Gets the file name from item.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="itemid">The itemid.</param>
        /// <returns>Full name of the file of the item</returns>
        private static string GetFileNameFromItem(IVsProject project, uint itemid)
        {
            string itemFullPath = null;

            if (ErrorHandler.Failed(project.GetMkDocument(itemid, out itemFullPath)))
            {
                return null;
            }

            return itemFullPath;

        }
        private bool IsTransformationFile(string fileName)
        {
            return transformationProvider.IsTransformationFile(fileName,
                GetPropertyValue<string>(OptionsPage.TransfomationFileNameRegexpPropertyName),
                GetPropertyValue<int>(OptionsPage.SourceFileRegexpMatchIndexPropertyName));
        }

        private bool ProjectSupportsTransforms(IVsProject project)
        {
            string projectFullPath = null;
            if (ErrorHandler.Failed(project.GetMkDocument(VSConstants.VSITEMID_ROOT, out projectFullPath)))
            {
                return false;
            }

            return transformationProvider.IsProjectSupported(projectFullPath);

        }

        /// <summary>
        /// Determines whether [is single project item selection] [the specified hierarchy].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="itemid">The itemid.</param>
        /// <returns></returns>
        private static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;
            int hierarchySelection = VSConstants.S_OK;

            var monitorSelection = GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            IVsMultiItemSelect multiItemSelect = null;
            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainerPtr = IntPtr.Zero;

            try
            {
                hierarchySelection = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hierarchySelection) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                // multiple items are selected
                if (multiItemSelect != null)
                {
                    return false;
                }

                // there is a hierarchy root node selected, thus it is not a single item inside a project

                if (itemid == VSConstants.VSITEMID_ROOT)
                {
                    return false;
                }

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null)
                {
                    return false;
                }

                var guidProjectID = Guid.Empty;

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out guidProjectID)))
                {
                    return false; // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
                }

                // if we got this far then there is a single project item selected
                return true;
            }
            finally
            {
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }

        #endregion


        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void TransformMenuItemCallback(object sender, EventArgs e)
        {
            string fileName;
            var result = IsCorrectItemForTransformationOperationsSelected(out fileName);
            if (!result)
            {
                return;
            }

            transformationProvider.Transform(fileName, GetPropertyValue<string>(OptionsPage.TransfomationFileNameRegexpPropertyName),
             GetPropertyValue<int>(OptionsPage.SourceFileRegexpMatchIndexPropertyName));

        }

        /// <summary>
        /// Compares the files menu item callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CompareFilesMenuItemCallback(object sender, EventArgs e)
        {
            string fileName;
            var result = IsCorrectItemForTransformationOperationsSelected(out fileName);
            if (!result)
            {
                return;
            }


            var files = transformationProvider.TransformToTemporaryFile(fileName, GetPropertyValue<string>(OptionsPage.TransfomationFileNameRegexpPropertyName),
                   GetPropertyValue<int>(OptionsPage.SourceFileRegexpMatchIndexPropertyName));

            if (files == null)
            {
                return;
            }

            this.CompareFiles(files.Item1, files.Item2);

        }

        /// <summary>
        /// Determines whether [is correct item for transformation operations selected] [the specified item full path].
        /// </summary>
        /// <param name="itemFullPath">The item full path.</param>
        /// <returns>[True] if correct file for transformation is selected, otherwise [False]</returns>
        private bool IsCorrectItemForTransformationOperationsSelected(out string itemFullPath)
        {
            itemFullPath = null;
            IVsHierarchy hierarchy = null;
            uint itemid = VSConstants.VSITEMID_NIL;

            //TODO: It is needed?
            if (!IsSingleProjectItemSelection(out hierarchy, out itemid))
            {
                return false;
            }

            var vsProject = (IVsProject)hierarchy;
            if (!ProjectSupportsTransforms(vsProject))
            {
                return false;
            }

            string projectFullPath = null;
            if (ErrorHandler.Failed(vsProject.GetMkDocument(VSConstants.VSITEMID_ROOT, out projectFullPath)))
            {
                return false;
            }

            //var buildPropertyStorage = vsProject as IVsBuildPropertyStorage;
            //if (buildPropertyStorage == null)
            //{
            //    return false;
            //}


            if (ErrorHandler.Failed(vsProject.GetMkDocument(itemid, out itemFullPath)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Value of the options property</returns>
        private static T GetPropertyValue<T>(string propertyName)
        {
            var dte = (DTE)GetGlobalService(typeof(DTE));
            var props = dte.Properties[ConfigTransformation, General];
            var val = props.Item(propertyName).Value;
            if (val == null)
            {
                return default(T);
            }

            return (T)val;
        }



        /// <summary>
        /// Compares the files.
        /// </summary>
        /// <param name="leftFileName">Name of the left file.</param>
        /// <param name="rightFileName">Name of the right file.</param>
        private void CompareFiles(string leftFileName, string rightFileName)
        {
            if (this.diffService == null)
            {
                return;
            }

            diffService.OpenComparisonWindow(leftFileName, rightFileName);
        }

        #endregion Methods

    }
}
