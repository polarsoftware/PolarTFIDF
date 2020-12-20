using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// 2019-01-22 - 15:05
    /// Construct string dir name and path with various combination 
    /// This class NEVER create file or directory on disk, just construct string of path.
    /// </summary>
    public static class DirNameConstructor
    {
        /// <summary>
        /// Construct string of Absolute path and dirs
        /// This method NEVER create file or directory on disk, just construct string of path.
        /// </summary>
        /// <param name="dirName1"></param>
        /// <param name="dirName2"></param>
        /// <param name="parentUpLevel"></param>
        /// <returns></returns>
        public static string GetAbsoluteDirPath(string dirName1, string dirName2, int parentUpLevel = 0)
        {
            string path = GetAbsoluteDirPath(dirName1, parentUpLevel);
            return Path.Combine(path, dirName2);
        }

        /// <summary>
        /// Construct string of Absolute path and dirs
        /// This method NEVER create file or directory on disk, just construct string of path.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="subdir1"></param>
        /// <param name="subdir2"></param>
        /// <param name="parentUpLevel"></param>
        /// <returns></returns>
        public static string GetAbsoluteDirPath(string dir, string subdir1, string subdir2, int parentUpLevel = 0)
        {
            string path = GetAbsoluteDirPath(dir, parentUpLevel);            
            path  = Path.Combine(path, subdir1);
            path = Path.Combine(path, subdir2);            
            return path;
        }

        public static string GetAbsoluteDirPath(List<string> dirs, int parentUpLevel = 0)
        {
            string path = GetAppDir(parentUpLevel);
            foreach (var item in dirs)
            {
                path = Path.Combine(path, item);
            }
            return path;
        }

        /// <summary>
        /// Construct string of Absolute path and dir
        /// This method NEVER create file or directory on disk, just construct string of path.        
        /// </summary>
        /// <param name="dirPathNames">Input is "dir1/dir2/dir3"</param>
        /// <param name="parentUpLevel"></param>
        /// <returns></returns>
        public static string GetAbsoluteDirPathFromNames(string dirPathNames, int parentUpLevel = 0)
        {            
            if (IsDirPathNamesValid(dirPathNames) == false)
            {
                throw new ArgumentNullException(dirPathNames);
            }

            string appDirPath;

            if (parentUpLevel <= 0)
            {
                appDirPath = GetAppDir();//vraca c:\users\alpha\documents ili sl patk.. ugalvnom na kraju nema "\\"
            }
            else
            {
                appDirPath = GetAppDir(parentUpLevel);
            }

            if (string.IsNullOrWhiteSpace(appDirPath) == true)
            {
                throw new ArgumentNullException(dirPathNames);                    
            }

            string pathCombination = Path.Combine(appDirPath, dirPathNames);
            return pathCombination;            
        }
       
        /// <summary>
        /// 2019-01-22 - 14:51        
        /// Construct and return string of absolute path in application folder on sent name which can be file name or direcotry name
        /// This method NOT CREATING dir or file name on this apsolute path 
        /// TODO: podijeliti na dvije class metode koje kreiraju dir i metode kojie vracaju apsolute path i konstrukcije oko njih 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAbsoluteDirPath(string dirName, int parentUpLevel = 0)
        {            
            if (IsDirNameValid(dirName) == false)
            {
                throw new ArgumentNullException(dirName);             
            }

            string appDirPath;
            if (parentUpLevel <= 0)
            {
                appDirPath = GetAppDir();//c:\users\alpha\documents ili sl patk.. ugalvnom na kraju nema "\\"
            }
            else
            {
                appDirPath = GetAppDir(parentUpLevel);
            }
                
            if (string.IsNullOrWhiteSpace(appDirPath) == true)
            {                
                throw new ArgumentNullException(dirName);
            }            
            return Path.Combine(appDirPath, dirName);
        }

        /// <returns></returns>
        public static string GetAppDir()
        {
            //GetParent 
            //https://stackoverflow.com/questions/14899422/how-to-navigate-a-few-folders-up
            //string directory = System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString());
                                    
            //- we need GetEntryAssembly because problem width GetExecutingAssembly if we use this code as dll under some exe .. then this return DLL path not exe path 
            string appPath = Assembly.GetEntryAssembly().Location;            
            string appDir = Path.GetDirectoryName(appPath);                
            
            return appDir;           
        }

        /// <summary>        
        /// Return string with apsoluth path to application dir and for up level above that
        /// </summary>
        /// <param name="parentUpLevel"></param>
        /// <returns></returns>
        private static string GetAppDir(int parentUpLevel)
        {
            //GetParent - 2018-10-06 - 8:41
            //https://stackoverflow.com/questions/14899422/how-to-navigate-a-few-folders-up
            
            string appPath = Assembly.GetEntryAssembly().Location;
            string appDir = Path.GetDirectoryName(appPath);            

            if (parentUpLevel > 0)
            {
                string appParentDir = appDir;
                for (int i = 0; i < parentUpLevel; i++)
                {
                    appParentDir = Directory.GetParent(appParentDir).ToString();
                }
                return appParentDir;
            }
            return appDir;            
        }
        
        /// <summary>
        /// Valid if do not coinatins any of: Path.AltDirectorySeparatorChar or Path.DirectorySeparatorChar  or GetInvalidPathChars 
        /// </summary>
        /// <param name="inDirName"></param>
        /// <returns></returns>
        public static bool IsDirNameValid(string inDirName)
        {
            if (string.IsNullOrWhiteSpace(inDirName) == true)
            {                
                return false;
            }

            if (inDirName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                return false;
            }
            if (inDirName.IndexOfAny(new char[] { Path.AltDirectorySeparatorChar }) != -1)
            {
                return false;
            }
            if (inDirName.IndexOfAny(new char[] { Path.DirectorySeparatorChar }) != -1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Input is "dir1/dir2/dir3"
        /// </summary>
        /// <param name="inDirName"></param>
        /// <returns></returns>
        public static bool IsDirPathNamesValid(string inDirName)
        {            
            if (string.IsNullOrWhiteSpace(inDirName) == true)
            {
                return false;
            }

            if (inDirName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                return false;
            }            
            return true;
        }       
    }
}
