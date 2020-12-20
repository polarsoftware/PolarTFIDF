using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Polar.ML.TfIdf
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
            }
        }

        /// <summary>2018-07-17 - 18:29
        /// Create dir in root of appicaliton (System.Reflection.Assembly.GetExecutingAssembly().Location;) 
        /// </summary>
        /// <param name="inDirName"></param>
        /// <returns>full apsolut apth to new dir. null if something wronw  </returns>
        public static string GetCreateDirInRootApp(string inDirName, int parentUpLevel = 0)
        {
            try
            {               
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
