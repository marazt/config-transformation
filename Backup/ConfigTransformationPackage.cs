using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Marazt.ConfigTransformation.Commands;
using Marazt.ConfigTransformation.FileNesting;
using Marazt.ConfigTransformation.Helpers;
using Marazt.ConfigTransformation.Logging;
using Marazt.ConfigTransformation.Options;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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
    [InstalledProductRegistration("#110", "#112", "1.3.3.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidConfigTransformationPkgString)]
    //This is needed cause auto-load project to enable correct visibility check
    //http://www.codingodyssey.com/2008/03/22/dynamic-menu-commands-in-visual-studio-packages-part-2/
    //https://github.com/oncheckin/oncheckin-transformer/blob/master/OnCheckinTransformer.VisualStudio/OnCheckinTransforms.VisualStudioPackage.cs
    // add these 2 Annotations to execute Initialize() immediately when a project is loaded
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    //http://msdn.microsoft.com/en-us/library/bb166195.aspx
    //http://msdn.microsoft.com/en-us/library/bb166553.aspx
    //http://code.msdn.microsoft.com/VSSDK-IDE-Sample-Options-f152f574
    //http://stackoverflow.com/questions/13749903/visual-studio-extension-wait-for-all-projects-to-complete-loading-with-ivssolut
    [ProvideOptionPageAttribute(typeof(OptionsPage), AppConstants.ConfigTransformation, AppConstants.General, 0, 0, true)]
    public sealed class ConfigTransformationPackage : Package, IVsSolutionEvents
    {




        #region Properties

        /// <summary>
        /// The solution
        /// </summary>
        private IVsSolution solution;

        /// <summary>
        /// The handle cookie
        /// </summary>
        private uint handleCookie = uint.MaxValue;


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
            var mcs = this.GetServiceIstanceOfInterface<IMenuCommandService, OleMenuCommandService>();
            if (null != mcs)
            {
                //Register commands
                CommandHelper.RegisterCommands(mcs);
            }

            this.AdviseSolutionEvents();

        }


        #endregion Package Members

        /// <summary>
        /// Gets the service istance of interface.
        /// It is created only because DTEHelper.GetServiceIstanceOfInterface return true
        /// while calling in Initialize method
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TInstance">The type of the instance.</typeparam>
        /// <returns>Instance of service. Can return null</returns>
        private TInstance GetServiceIstanceOfInterface<TInterface, TInstance>()
         where TInstance : class
        {
            return this.GetService(typeof(TInterface)) as TInstance;
        }

        /// <summary>
        /// Releases the resources used by the <see cref="T:Microsoft.VisualStudio.Shell.Package" /> object.
        /// </summary>
        /// <param name="disposing">true if the object is being disposed, false if it is being finalized.</param>
        protected override void Dispose(bool disposing)
        {
            UnadviseSolutionEvents();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Advises the solution events.
        /// </summary>
        private void AdviseSolutionEvents()
        {
            UnadviseSolutionEvents();


            this.solution = this.GetServiceIstanceOfInterface<SVsSolution, IVsSolution>();

            if (this.solution != null)
            {
                solution.AdviseSolutionEvents(this, out this.handleCookie);
            }
        }

        /// <summary>
        /// Unadvises the solution events.
        /// </summary>
        private void UnadviseSolutionEvents()
        {
            if (this.solution != null)
            {
                if (this.handleCookie != uint.MaxValue)
                {
                    this.solution.UnadviseSolutionEvents(this.handleCookie);
                    this.handleCookie = uint.MaxValue;
                }

                this.solution = null;
            }
        }

        #endregion Methods


        #region IVsSolutionEvents Members

        /// <summary>
        /// Notifies listening clients that the project has been opened.
        /// </summary>
        /// <param name="pHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project being loaded.</param>
        /// <param name="fAdded">[in] true if the project is added to the solution after the solution is opened. false if the project is added to the solution while the solution is being opened.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            if (Options.Options.NestTransformationFiles)
            {
                Logger.LogInfo(Resources.NestingFiles);
                var project = SolutionHelper.GetProjectFromHierarchy(pHierarchy);
                ConfigFileNester.NestConfigurationFilesInProject(project);
                ConfigFileNester.NestConfigurationFilesInSolution();
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Queries listening clients as to whether the project can be closed.
        /// </summary>
        /// <param name="pHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project to be closed.</param>
        /// <param name="fRemoving">[in] true if the project is being removed from the solution before the solution is closed. false if the project is being removed from the solution while the solution is being closed.</param>
        /// <param name="pfCancel">[out] true if the client vetoed the closing of the project. false if the client approved the closing of the project.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies listening clients that the project is about to be closed.
        /// </summary>
        /// <param name="pHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project being closed.</param>
        /// <param name="fRemoved">[in] true if the project was removed from the solution before the solution was closed. false if the project was removed from the solution while the solution was being closed.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies listening clients that the project has been loaded.
        /// </summary>
        /// <param name="pStubHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the placeholder hierarchy for the unloaded project.</param>
        /// <param name="pRealHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project that was loaded.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Queries listening clients as to whether the project can be unloaded.
        /// </summary>
        /// <param name="pRealHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project to be unloaded.</param>
        /// <param name="pfCancel">[out] true if the client vetoed unloading the project. false if the client approved unloading the project.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies listening clients that the project is about to be unloaded.
        /// </summary>
        /// <param name="pRealHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project that will be unloaded.</param>
        /// <param name="pStubHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the placeholder hierarchy for the project being unloaded.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies listening clients that the solution has been opened.
        /// </summary>
        /// <param name="pUnkReserved">[in] Reserved for future use.</param>
        /// <param name="fNewSolution">[in] true if the solution is being created. false if the solution was created previously or is being loaded.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Queries listening clients as to whether the solution can be closed.
        /// </summary>
        /// <param name="pUnkReserved">[in] Reserved for future use.</param>
        /// <param name="pfCancel">[out] true if the client vetoed closing the solution. false if the client approved closing the solution.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies listening clients that the solution is about to be closed.
        /// </summary>
        /// <param name="pUnkReserved">[in] Reserved for future use.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Notifies listening clients that a solution has been closed.
        /// </summary>
        /// <param name="pUnkReserved">[in] Reserved for future use.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        #endregion IVsSolutionEvents Members

    }
}
