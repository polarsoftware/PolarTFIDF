using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Polar.System
{
    public static class DirHandler
    {
        public static string CreateDirInRootApp(string dir, int parentUpLevel = 0)
        {
            string path = DirNameConstructor.GetAbsoluteDirPath(dir, parentUpLevel);
            Directory.CreateDirectory(path);
            return path;
        }

        public static string CreateDirInRootApp(string dir, string subdir, int parentUpLevel = 0)
        {
            string path = DirNameConstructor.GetAbsoluteDirPath(dir, subdir);
            Directory.CreateDirectory(path);
            return path;
        }

        public static string CreateDirInRootApp(string dir, string subdir1, string subdir2, int parentUpLevel = 0)
        {
            string path = DirNameConstructor.GetAbsoluteDirPath(dir, subdir1, subdir2);            
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// 2019-01-22 - 3:30
        /// From string which contain relative dirs path (dir1/dir2/dir3) or (dir1\dir2\dir3) create dir and subdir in root of appicaliton 
        /// (System.Reflection.Assembly.GetExecutingAssembly().Location;) 
        /// We need to construct like this because:
        /// - on Windows return  c:\users\alpha\documents\         
        /// - on Linux return c/users/alpha/documents/ 
        /// </summary>
        /// <param name="dirsPath">string1/string2/string3...</param>
        /// <returns></returns>
        public static string CreateDirInRootAppFromPath(string dirsPath, int parentUpLevel = 0)
        {            
            if (string.IsNullOrWhiteSpace(dirsPath) == true)
            {
                throw new ArgumentException("if (string.IsNullOrWhiteSpace(dirsPath) == true)");
            }

            dirsPath = dirsPath.Trim();
            var cs = new char[] { '/', '\\' };
            dirsPath = dirsPath.Trim(cs);
            string[] dirs = dirsPath.Split(cs);
            return CreateDirInRootAppFromList(dirs.ToList(), parentUpLevel);            
        }
                
        /// <summary>
        /// 2019-01-22 - 3:23
        /// Create dir and subdir on disk from list of string in root of application (System.Reflection.Assembly.GetExecutingAssembly().Location;) 
        /// We need to construct like this because: 
        /// - on Windows return  c:\users\alpha\documents\         
        /// - on Linux return c/users/alpha/documents/ 
        /// </summary>
        /// <param name="inDirName"></param>
        /// <returns>full apsolut apth to new dir. null if something wronw  </returns>
        public static string CreateDirInRootAppFromList(List<string> dirs, int parentUpLevel = 0)
        {
            try
            {
                if (dirs == null || dirs.Count == 0)
                {
                    throw new ArgumentException("if (dirs == null || dirs.Count == 0)");                    
                }

                foreach (var dir in dirs)
                {
                    if (string.IsNullOrWhiteSpace(dir) == true)
                    {
                        throw new ArgumentException("if (string.IsNullOrWhiteSpace(dir) == true)");
                    }
                }

                string firstDir = dirs.First();
                string pathCombination = GetCreateDirInRootApp(firstDir, parentUpLevel);
                if (string.IsNullOrWhiteSpace(pathCombination) == true)
                {
                    throw new ArgumentException("if (string.IsNullOrWhiteSpace(pathCombination) == true)");
                }

                if (dirs.Count == 1)
                {
                    return pathCombination;
                }

                bool skip = true;//skip first time because we created it
                foreach (var dir in dirs)
                {
                    if (skip == true)
                    {
                        skip = false;
                        continue;
                    }

                    if (DirNameConstructor.IsDirNameValid(dir) == false)
                    {
                        throw new ArgumentException("DirNameConstructor.IsDirNameValid(dir) == false");                        
                    }
                    pathCombination = Path.Combine(pathCombination, dir);
                    if (pathCombination == null)
                    {
                        throw new ArgumentException("if (pathCombination == null)");
                        //return null;
                    }
                    if (Directory.Exists(pathCombination) == false)
                    {
                        Directory.CreateDirectory(pathCombination);
                    }
                }
                return pathCombination;
            }
            catch (Exception e)
            {
                throw e;
                //LoggerApp.Error($"ERROR: DirHandler.GetCreateDirInRootApp(List<string> dirs): {e.ToString()} - {DateTime.UtcNow}");
                //return null;
            }
        }


        /// <summary>2018-07-17 - 18:29
        /// TODO1 !!!!!: maknuti ovo gdje se god zbraja ime file na ovo jer je to za Linux pogresno - 2018-07-24 - 15:46
        /// (valjda se misli na to kad se dobije string odavde da se rucno ide na ovo dodat npr. "path" + "\db22" a valjda moze sa Path.Combine() ) + 2019-06-13 - 9:53
        /// Create dir in root of appicaliton (System.Reflection.Assembly.GetExecutingAssembly().Location;) 
        /// </summary>
        /// <param name="inDirName"></param>
        /// <returns>full apsolut apth to new dir. null if something wronw  </returns>
        public static string GetCreateDirInRootApp(string inDirName, int parentUpLevel = 0)
        {
            try
            {
                /*2019-01-22 - 16:49
                if (DirNameConstructor.IsDirNameValid(inDirName) == false)
                {
                    return null;
                }
                string appDirPath = DirNameConstructor.GetAppDir();//vraca c:\users\alpha\documents ili sl patk.. ugalvnom na kraju nema "\\"
                if (string.IsNullOrWhiteSpace(appDirPath) == true)
                {
                    LoggerApp.Error($"ERROR: GetAppDir(string path) if (string.IsNullOrWhiteSpace(appDirPath) == true) - {DateTime.UtcNow}");
                    return null;
                }

                string pathCombination = Path.Combine(appDirPath, inDirName);
                */
                string pathCombination = DirNameConstructor.GetAbsoluteDirPath(inDirName, parentUpLevel);
                if (Directory.Exists(pathCombination) == false)
                {
                    Directory.CreateDirectory(pathCombination);
                }
                return pathCombination;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}

/*2018-07-18 - 17:17
           string dir = DirHandler.GetAbsoluteFilePath(inDirName);
           if (!Directory.Exists(dir))
           {
               Directory.CreateDirectory(dir);
           }*/
/*inDirName.Trim('\\');
string dir = DirHandler.GetAppDir() + "\\" + inDirName + "\\";
if (!Directory.Exists(dir))
{
    Directory.CreateDirectory(dir);
}*/

/*
    /// <summary>
    /// </summary>
    /// <param name="inDirName"></param>
    /// <returns></returns>
    public static string GetDirNameAndCreate(string inDirName)
    {
        string dir = DirHandler.GetAbsoluteFilePath(inDirName);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        return dir;

        2018-07-17 - 18:33
        inDirName.Trim('\\');
        string dir = DirHandler.GetAppDir() + "\\" + inDirName + "\\";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        //return dir;
    }*/
