using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Aleab.CefUnity.UnityEditor
{
    public class BuildPostProcessor
    {
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // Only Windows x86 and x86_64 atm.
            if (target != BuildTarget.StandaloneWindows && target != BuildTarget.StandaloneWindows64)
                return;

            ////////////////////////////////////////////////////////////////
            // COPY EVERY FILE IN THE PLUGIN DIRECTORY TO THE BUILD PATH. //
            ////////////////////////////////////////////////////////////////

            // Get the source directory (Assets/Plugins or Assets/Plugins/x86 or Assets/Plugins/x86_64).
            string srcPluginsFolder = string.Format("{0}/{1}", Application.dataPath, "Plugins");
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                    if (Directory.Exists(srcPluginsFolder + "/x86"))
                        srcPluginsFolder += "/x86";
                    break;

                case BuildTarget.StandaloneWindows64:
                    if (Directory.Exists(srcPluginsFolder + "/x86_64"))
                        srcPluginsFolder += "/x86_64";
                    break;

                default:
                    break;
            }

            // Get the destination directory (<BUILT_EXE_PATH>/<EXE_NAME>_Data/Plugins).
            int splitIndex = pathToBuiltProject.LastIndexOf('/');
            string buildPath = pathToBuiltProject.Substring(0, splitIndex);
            string executableName = pathToBuiltProject.Substring(splitIndex + 1);
            string buildPluginsPath = string.Format("{0}/{1}_Data/Plugins", buildPath, Path.GetFileNameWithoutExtension(executableName));

            DirectoryInfo srcPluginsFolderInfo = new DirectoryInfo(srcPluginsFolder);
            DirectoryInfo buildPluginsPathInfo = new DirectoryInfo(buildPluginsPath);

            // Exclude some files (.dlls should already be there!)
            var srcPluginsFolderFiles = srcPluginsFolderInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly)
                                                            .Where(fi => !fi.Name.EndsWith(".meta") && !fi.Name.EndsWith(".dll") &&
                                                                         !fi.Name.EndsWith(".pdb") && !fi.Name.EndsWith(".lib") &&
                                                                         !fi.Name.EndsWith(".mdb") && !fi.Name.EndsWith(".xml"));
            var srcPluginsFolderDirectories = srcPluginsFolderInfo.GetDirectories();

            // Copy selected files and sub-directories.
            foreach (var dir in srcPluginsFolderDirectories)
                DirectoryCopy(dir.FullName, string.Format("{0}/{1}", buildPluginsPathInfo.FullName, dir.Name), true);
            foreach (var file in srcPluginsFolderFiles)
                File.Copy(file.FullName, string.Format("{0}/{1}", buildPluginsPathInfo.FullName, file.Name), false);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}