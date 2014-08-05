using System.IO;
using EnvDTE;
using Marazt.ConfigTransformation.Extensions;
using Marazt.ConfigTransformation.Helpers;
using Marazt.ConfigTransformation.Transformation;
using Microsoft.Practices.RecipeFramework.Library;

//using Microsoft.Practices.RecipeFramework.Library; it contains DteHelper class

namespace Marazt.ConfigTransformation.FileNesting
{
    /// <summary>
    /// Class for configuration file nesting
    /// </summary>
    internal static class ConfigFileNester
    {
        #region Properties

        #endregion Properties

        #region Methods



        /// <summary>
        /// Nests the configuration files in solution.
        /// </summary>
        public static void NestConfigurationFilesInSolution()
        {

            foreach (Project project in DTEHelper.GetInstance().Solution.Projects)
            {
                NestConfigurationFilesInProject(project);
            }
        }

        /// <summary>
        /// Nests the configuration files in project.
        /// </summary>
        /// <param name="project">The project.</param>
        public static void NestConfigurationFilesInProject(Project project)
        {
            if (project.Kind != Constants.vsProjectKindSolutionItems)
            {
                if (project.ProjectItems != null && project.ProjectItems.Count > 0)
                {
                    SearchFolConfigFilesAndNestThem(project.ProjectItems);
                }
            }
            else
            {
                // if solution folder, one of its ProjectItems might be a real project
                if (project.ProjectItems != null)
                {
                    foreach (ProjectItem item in project.ProjectItems)
                    {
                        var realProject = item.Object as Project;

                        if (realProject != null)
                        {
                            SearchFolConfigFilesAndNestThem(project.ProjectItems);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Searchs the fol configuration files and nest them.
        ///From Radim: http://stackoverflow.com/questions/19427333/how-to-find-a-projectitem-by-the-file-name
        /// </summary>
        /// <param name="projectItems">The project items.</param>
        private static void SearchFolConfigFilesAndNestThem(ProjectItems projectItems)
        {
            foreach (ProjectItem projectItem in projectItems)
            {
                string transFileName;

                if (projectItem.TryGetPropertyValue(ProjectItemExt.FullPathProperty, out transFileName))
                {
                    transFileName = Path.GetFileName(transFileName);
                    string configFileName;
                    if (TransformationProvider.CheckTransformationFileAndGetSourceFileFromIt(transFileName,
                  Options.Options.TransfomationFileNameRegexp, Options.Options.SourceFileRegexpMatchIndex,
                    out configFileName))
                    {
                        var configItem = DteHelper.FindItemByName(projectItems, configFileName, true);
                        var itemToBeNested = DteHelper.FindItemByName(projectItems, transFileName, true);

                        if (configItem == null || itemToBeNested == null)
                        {
                            continue;
                        }

                        // ReSharper disable once UnusedVariable
                        var pitn = configItem.ProjectItems.AddFromFile(itemToBeNested.GetFullPath());
                        string itemType;
                        if (itemToBeNested.TryGetPropertyValue(ProjectItemExt.ItemTypeProperty, out itemType))
                        {
                            pitn.TrySetPropertyValue(ProjectItemExt.ItemTypeProperty, itemType);
                        }

                    }
                }
            }
        }

        #endregion Methods
    }
}
