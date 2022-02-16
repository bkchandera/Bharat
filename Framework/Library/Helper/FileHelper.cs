using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace Framework.Library.Helper
{
    public static class FileHelper
    {

        public static string ReadFile(string FileName)
        {
            return File.ReadAllText(FileName);
        }
        public static string FileName(string name)
        {
            return Directory.GetCurrentDirectory() + name;
        }

        public static string FileName(string name, string ProjName)
        {
            return Directory.GetParent(Directory.GetCurrentDirectory()) + "\\" + ProjName + "\\" + name;
        }
        //public static string FileName(string path,string name)
        //{
        //    return path+"\\" +name;
        //}
        public static string NewFileName(string path, string NewName)
        {
            return path + "\\" + NewName.CurrentTimestamp() + ".xlsx";
        }
        public static string NewFileName(string path, string NewName, string extension = "xlsx")
        {
            return path + "\\" + NewName;
        }
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static string FileServerPath(string flag = "Upload")
        {
            return Directory.GetParent(Directory.GetCurrentDirectory()) + "\\FileServer\\Import\\" + flag;
        }
        public static string FrameworkPath(string flag)
        {
            return Directory.GetParent(Directory.GetCurrentDirectory()) + "\\Framework\\" + flag;
        }
        public static string ProjectPath()
        {
            return Directory.GetParent(Directory.GetCurrentDirectory()) + "\\";
        }
        public static string FullPath()
        {

            return Path.GetDirectoryName(new FileInfo(Assembly.GetExecutingAssembly().Location).FullName) + "\\";
        }
        public static string DirectoryPath(string dir)
        {
            return Directory.GetParent(Directory.GetCurrentDirectory()) + "\\FileServer\\" + dir + "\\";
        }
        public static string FileNameWithTimestamp(string name, string extension)
        {
            return name + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + extension;
        }

        public static byte[] ZipFile(Dictionary<string, string> FileInfo)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (KeyValuePair<string, string> file in FileInfo)
                    {
                        ziparchive.CreateEntryFromFile(file.Key, file.Value);
                    }
                }
                return memoryStream.ToArray();
                //return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
            }
        }

        /// <summary>
        /// this 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="file_server_path">file server path is FileServer</param>
        /// <returns></returns>
        public static string UploadFile(IFormFile file, string file_server_path)
        {
            string new_file_name = string.Empty;
            if (file != null)
            {
                string file_extension = Path.GetExtension(file.FileName);
                string[] extension_array = { ".jpg", ".jpeg", ".png", ".ico" };
                new_file_name = string.Concat(Guid.NewGuid().ToString().Replace("-","").ToUpper(), file_extension);
                if (extension_array.Contains(file_extension))
                {
                    string dir_path = FileHelper.FileServerPath(file_server_path);
                    FileHelper.CreateDirectory(dir_path);
                    new_file_name = FileHelper.NewFileName(dir_path, new_file_name, file_extension);

                    using (var fileStream = new FileStream(new_file_name, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
            }
            return new_file_name;
        }
    }
}


