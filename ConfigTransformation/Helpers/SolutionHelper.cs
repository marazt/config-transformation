using System;
using System.Runtime.InteropServices;
using Marazt.ConfigTransformation.Transformation;
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
        /// Determines whether [is single project item selection] [the specified hierarchy].
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="itemid">The itemid.</param>
        /// <returns>[True] if single project item is selected, otherwise [False]</returns>
        public static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;

            // ReSharper disable once TooWideLocalVariableScope
            // ReSharper disable once RedundantAssignment
            var hierarchySelection = VSConstants.S_OK;

            var monitorSelection = DteHelper.GetServiceIstanceOfInterface<SVsShellMonitorSelection, IVsMonitorSelection>();
            var solution = DteHelper.GetServiceIstanceOfInterface<SVsSolution, IVsSolution>();
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            var hierarchyPtr = IntPtr.Zero;
            var selectionContainerPtr = IntPtr.Zero;

            try
            {
                IVsMultiItemSelect multiItemSelect;
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
        /// <param name="itemid">The itemid.</param>
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
