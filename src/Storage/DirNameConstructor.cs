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

        /// <summary>2018-06-11 - 16:59
        /// 2020-09-09T04:52:42
        /// Testing razne opcije za - Single file deployment and executable - 
        /// Za single file ovo radi ispranvo javi od exe path: 
        /// Console.WriteLine($"Process.GetCurrentProcess().MainModule.FileName = {Process.GetCurrentProcess().MainModule.FileName}");
        /// 
        /// Ovoo ostlao radi tamo gjde je ekrahiran single file 
        /// Console.WriteLine($"AppContext.BaseDirectory = {AppContext.BaseDirectory}");            
        /// Console.WriteLine($"Assembly.GetEntryAssembly().Location = {Assembly.GetEntryAssembly().Location}");
        /// Console.WriteLine($"Assembly.GetExecutingAssembly().Location = {Assembly.GetExecutingAssembly().Location}");
        /// Console.WriteLine($"Assembly.GetCallingAssembly().Location = {Assembly.GetCallingAssembly().Location}");
        ///                                                     
        /// --------------
        /// string appDirPath = DirHandler.GetAppDir();
        /// - u Windows vraca c:\users\alpha\documents\         
        /// - u Linux vraca c/users/alpha/documents/ 
        /// - bug: path na linuxu nije isti kao na win. Window using BACKSLASH = \ , Linux using SLASH = /	a URL using SLASH = / 
        ///- RIJESENJE: 
        ///	- koristi Path.Combine() - https://docs.microsoft.com/en-us/dotnet/api/system.io.path.combine?view=netcore-2.1		
        ///- DirectorySeparatorChar - https://docs.microsoft.com/en-us/dotnet/api/system.io.path.directoryseparatorchar?view=netcore-2.1
        ///	- vraca sluzbeni spearator direktoraij za taj OS 
        ///	- za windows je \
        ///	- za Linux je /
        ///- AltDirectorySeparatorChar - https://docs.microsoft.com/en-us/dotnet/api/system.io.path.altdirectoryseparatorchar?view=netcore-2.1
        ///	- vraca alternativni ISPRAVNI separator direktorija za taj OS koji se moze upotrebiti 
        ///	- za windows je /
        ///	- za Linux je / - mislim treba provjeriti 
        ///- REF:
        ///	https://superuser.com/questions/176388/why-does-windows-use-backslashes-for-paths-and-unix-forward-slashes
        ///	https://docs.microsoft.com/en-us/windows/desktop/FileIO/naming-a-file		
        ///	https://en.wikipedia.org/wiki/Path_(computing)
        /// Return dir of app
        /// https://stackoverflow.com/questions/837488/how-can-i-get-the-applications-path-in-a-net-console-application
        /// https://stackoverflow.com/questions/15653921/get-current-folder-path/15653950
        /// https://stackoverflow.com/questions/6246074/mono-c-sharp-get-application-path/18562036#18562036
        /// One correct and cross-platform solution would be
        /// Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
        /// Note that both Environment.CurrentDirectory and Assembly.GetExecutingAssembly().Location (more exactly, the directory thereof) are semantically wrong even though they are often - but not always - the same directory:        
        /// The current directory is the "working directory" and can be changed at any point in time, like the "cd" command does in a shell.
        /// The executing assembly is the assembly that contains the code which is currently running, and may or may not be in the same directory as the actual application.For instance if your application is "A.exe" which has a dependency "B.dll", and some code in B.dll calls Assembly.GetExecutingAssembly(), it would result in "/path/to/B.dll".
        /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly?view=netcore-2.1
        /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.location?view=netcore-2.1#System_Reflection_Assembly_Location
        /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.codebase?view=netcore-2.1#System_Reflection_Assembly_CodeBase
        /// </summary>
        /// <returns></returns>
        public static string GetAppDir()
        {
            //GetParent - 2018-10-06 - 8:41
            //https://stackoverflow.com/questions/14899422/how-to-navigate-a-few-folders-up
            //string directory = System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString());

            //2019-06-11 - 2:05 - problem width GetExecutingAssembly if we use this code as dll under some exe .. then this return DLL path not exe path 
            //2019-06-11 - 2:10 string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;//vraca: c:\users\alpha\documents\visual...
            //2019-06-11 - 2:06 LoggerApp.Information($"Assembly.GetExecutingAssembly().Location: {appPath} - {DateTime.UtcNow}");
                        
            //2019-06-11 - 2:05 - we need GetEntryAssembly because problem width GetExecutingAssembly if we use this code as dll under some exe .. then this return DLL path not exe path 
            string appPath = Assembly.GetEntryAssembly().Location;//vraca: c:\users\alpha\documents\visual...
            //2019-06-11 - 2:06 LoggerApp.Information($"Assembly.GetEntryAssembly().Location: {appPath} - {DateTime.UtcNow}");
            string appDir = Path.GetDirectoryName(appPath);
                
            //appDir = appDir.Trim('\\');//2018-07-17 - 14:39 necemo ovo trimati jer na Linuxo je / a ne \ - idem ostavlajti i dodavati gore sto ide vec                                               
            return appDir;           
        }

        /// <summary>
        /// 2019-01-22 - 14:44
        /// Return string with apsoluth path to application dir and for up lvele above that
        /// </summary>
        /// <param name="parentUpLevel"></param>
        /// <returns></returns>
        private static string GetAppDir(int parentUpLevel)
        {
            //GetParent - 2018-10-06 - 8:41
            //https://stackoverflow.com/questions/14899422/how-to-navigate-a-few-folders-up
            //string directory = System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString());
            //2019-06-11 - 2:08 string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;//vraca: c:\users\alpha\documents\visual...
            //2019-06-11 - 2:05 - we need GetEntryAssembly because problem width GetExecutingAssembly if we use this code as dll under some exe .. then this return DLL path not exe path 
            string appPath = Assembly.GetEntryAssembly().Location;//vraca: c:\users\alpha\documents\visual...
            string appDir = Path.GetDirectoryName(appPath);
            //appDir = appDir.Trim('\\');//2018-07-17 - 14:39 necemo ovo trimati jer na Linuxo je / a ne \ - idem ostavlajti i dodavati gore sto ide vec

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

        //PRIVATE: ----------------------------------------------------------
    }
}
