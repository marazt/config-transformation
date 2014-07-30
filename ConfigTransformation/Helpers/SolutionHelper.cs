using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Marazt.ConfigTransformation.Transformation;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;


namespace Marazt.ConfigTransformation.Helpers
{
    /// <summary>
    /// Solution Helepr class
    /// </summary>
    public static class SolutionHelper
    {

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <returns>Project instance of set hierarchy</returns>
        public static Project GetProjectFromHierarchy(IVsHierarchy hierarchy)
        {
            object project;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT,(int)__VSHPROPID.VSHPROPID_ExtObject,out project));
            return (project as Project);

        }


        /// <summary>
        /// Finds the name of the solution item by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns>SolutionItem instance of set name or null if not found</returns>
        public static ProjectItem FindSolutionItemByName(string name, bool recursive)
        {
            ProjectItem projectItem = null;
            foreach (Project project in DTEHelper.GetInstance().Solution.Projects)
            {
                projectItem = FindProjectItemInProject(project, name, recursive);

                if (projectItem != null)
                {
                    break;
                }
            }
            return projectItem;
        }

        /// <summary>
        /// Finds the project item in project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="name">The name.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns>ProjectItem instance of set name or null if not found</returns>
        public static ProjectItem FindProjectItemInProject(Project project, string name, bool recursive)
        {
            ProjectItem projectItem = null;

            if (project.Kind != EnvDTE.Constants.vsProjectKindSolutionItems)
            {
                if (project.ProjectItems != null && project.ProjectItems.Count > 0)
                {
                    projectItem = DteHelper.FindItemByName(project.ProjectItems, name, recursive);
                }
            }
            else
            {
                // if solution folder, one of its ProjectItems might be a real project
                foreach (ProjectItem item in project.ProjectItems)
                {
                    var realProject = item.Object as Project;

                    if (realProject != null)
                    {
                        projectItem = FindProjectItemInProject(realProject, name, recursive);

                        if (projectItem != null)
                        {
                            break;
                        }
                    }
                }
            }

            return projectItem;
        }



        /// <summary>
        /// Determines whether [is single project item selection] [the specified hierarchy].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="itemID">The itemID.</param>
        /// <returns>[True] if single project item is selected, otherwise [False]</returns>
        public static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemID)
        {
            hierarchy = null;
            itemID = VSConstants.VSITEMID_NIL;

            // ReSharper disable once TooWideLocalVariableScope
            // ReSharper disable once RedundantAssignment
            var hierarchySelection = VSConstants.S_OK;

            var monitorSelection = DTEHelper.GetServiceIstanceOfInterface<SVsShellMonitorSelection, IVsMonitorSelection>();
            var solution = DTEHelper.GetServiceIstanceOfInterface<SVsSolution, IVsSolution>();
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            var hierarchyPtr = IntPtr.Zero;
            var selectionContainerPtr = IntPtr.Zero;

            try
            {
                IVsMultiItemSelect multiItemSelect;
                hierarchySelection = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemID, out multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hierarchySelection) || hierarchyPtr == IntPtr.Zero || itemID == VSConstants.VSITEMID_NIL)
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

                if (itemID == VSConstants.VSITEMID_ROOT)
                {
                    return false;
                }

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null)
                {
                    return false;
                }

                // ReSharper disable once RedundantAssignment
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

        /// <summary>
        /// Projects the supports transforms.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// [True] if project suppors transformation, otherwise [False]
        /// </returns>
        public static bool ProjectSupportsTransforms(IVsProject project)
        {
            string projectFullPath;
            if (ErrorHandler.Failed(project.GetMkDocument(VSConstants.VSITEMID_ROOT, out projectFullPath)))
            {
                return false;
            }

            return TransformationProvider.IsProjectSupported(projectFullPath);
        }

        /// <summary>
        /// Gets the file name from item.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="itemid">The itemID.</param>
        /// <returns>Full name of the file of the item</returns>
        public static string GetFileNameFromItem(IVsProject project, uint itemid)
        {
            string itemFullPath;

            if (ErrorHandler.Failed(project.GetMkDocument(itemid, out itemFullPath)))
            {
                return null;
            }

            return itemFullPath;

        }


        /// <summary>
        /// Determines whether [is correct item for transformation operations selected] [the specified item full path].
        /// </summary>
        /// <param name="itemFullPath">The item full path.</param>
        /// <returns>[True] if correct file for transformation is selected, otherwise [False]</returns>
        public static bool IsCorrectItemForTransformationOperationsSelected(out string itemFullPath)
        {
            itemFullPath = null;
            IVsHierarchy hierarchy;
            // ReSharper disable once RedundantAssignment
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

            string projectFullPath;
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


    }
}
