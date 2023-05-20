using ConsoleApp7.Components;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp7.Services
{
    public class FilesService
    {
        
        public string[] drives { get; set; } = new string[7];
        public string[] files { get; set; } = new string[7];

        public FilesService()
        {
          
        }

        public List<Row> DrivesData()
        {
            List<Row> result = this.ReadDrives();
            result.RemoveAt(0);

            return result;
        }

        public List<Row> Data(string path)
            {
            List<Row> result = this.Read(path);
                return result;
            }

        private List<Row> ReadDrives()
            {
                List<Row> result = new List<Row>();
                files[0] = "..";
                files[1] = "";
                files[2] = "";
                files[3] = "";
                files[4] = "";
                files[5] = "";
                files[6] = "";

                result.Add(new Row(files));

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                string sizeText = drive.TotalSize.ToString();
                string sizeGB = GetSize(sizeText);

                drives[0] = drive.Name;
                drives[1] = sizeGB;
                drives[2] = "";
                drives[3] = drive.RootDirectory.ToString();
                drives[4] = drive.Name.ToString();
                drives[5] = "";
                drives[6] = "drive";

                result.Add(new Row(drives));
            }

            return result;
        }

        private List<Row> Read(string path)
        {
            List<Row> result = new List<Row>();
            if (path == "")
            {
                ReadDrives();
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(path);

                files[0] = "..";
                files[1] = "";
                files[2] = "";
                files[3] = "";
                files[4] = "";
                files[5] = "";
                files[6] = "";
                result.Add(new Row(files));


                foreach (DirectoryInfo item in dir.GetDirectories())
                {
                    string date = item.LastWriteTime.ToString();
                    string name = item.Name.ToString();

                    if (name.Length > Console.WindowWidth / 4)
                    {
                        name = name.Remove(Console.WindowWidth / 4, name.Length - Console.WindowWidth / 4) + "~";
                    }

                    DirectoryInfo parent = item.Parent;
                    string preparent = "";
                    if (parent != null)
                    {
                        if (parent.Parent != null)
                        {
                            preparent = parent.Parent.ToString();
                        }
                        else
                        {
                            preparent = item.Root.ToString();
                        }
                    }

                    files[0] = name;
                    files[1] = "dir".PadLeft(8);
                    files[2] = date.Substring(0, 10);
                    files[3] = preparent;
                    files[4] = item.FullName;
                    files[5] = item.FullName;
                    files[6] = "dir";
                    result.Add(new Row(files));
                }



                foreach (FileInfo item in dir.GetFiles())
                {
                    string date = item.LastWriteTime.ToString();
                    string name = item.Name.ToString();

                    DirectoryInfo parent = item.Directory;
                    string preparent = "";

                    if (parent != null)
                    {
                        if (parent.Parent != null)
                        {
                            preparent = parent.Parent.ToString();
                        }
                        else
                        {
                            preparent = item.Directory.Root.ToString();
                        }
                    }

                    if (name.Length > Console.WindowWidth / 4)
                    {
                        name = name.Remove(Console.WindowWidth / 4, name.Length - Console.WindowWidth / 4) + "~";
                    }

                    string sizeText = item.Length.ToString();
                    string sizeMeasured = GetSize(sizeText);

                    files[0] = name;
                    files[1] = sizeMeasured;
                    files[2] = date.Substring(0, 10);
                    files[3] = preparent;
                    files[4] = item.Directory.ToString();
                    files[5] = item.FullName;
                    files[6] = "file";

                    result.Add(new Row(files));
                }
            }
                return result;
            
        }

        private string GetSize(string FileSize)
        {
            long size = long.Parse(FileSize);
            string sizeMeasured;

            if (size > 10000000000)
            {
                size = size / 1000000000;
                sizeMeasured = size.ToString() + " GB";
                sizeMeasured = sizeMeasured.PadLeft(8);
            }
            else if (size > 1000000000)
            {
                double dotSize = size;
                dotSize = dotSize / 1000000000;
                sizeMeasured = dotSize.ToString();
                if (sizeMeasured.Length > 3)
                {
                    sizeMeasured = sizeMeasured.Substring(0, sizeMeasured.LastIndexOf(".") + 4) + " GB";
                    sizeMeasured = sizeMeasured.PadLeft(8);
                }
                else
                {
                    sizeMeasured = sizeMeasured.Substring(0, sizeMeasured.LastIndexOf(".")) + " GB";
                    sizeMeasured = sizeMeasured.PadLeft(8);

                }
            }
            else if (size > 1000000)
            {
                size = size / 1000000;
                sizeMeasured = size.ToString() + " MB";
                sizeMeasured = sizeMeasured.PadLeft(8);
            }
            else if (size > 1000)
            {
                size = size / 1000;
                sizeMeasured = size.ToString() + " KB";
                sizeMeasured = sizeMeasured.PadLeft(8);
            }
            else
            {
                sizeMeasured = size.ToString() + "  B";
                sizeMeasured = sizeMeasured.PadLeft(8);
            }
            return sizeMeasured;
        }

        public void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exist
            if (!dir.Exists);
               
            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();
            
            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public void CopyFile(string sourceDir, string destinationDir, bool recursi)
        {
            File.Copy(sourceDir, destinationDir, true);
        }

        public void DeleteItem(string path)
        {
             bool isDir = Directory.Exists(path);
             if (isDir)
             {
                 Directory.Delete(path, true);
             }
             else
             {
                 File.Delete(path);
             }
        }
        
        public void MkDir(string path)
        {
            if (Directory.Exists(path))
            {
                int origLenght = path.Length;
                int i = 2;
                
                while (Directory.Exists(path))
                {
                    path = path.Substring(0, origLenght);
                    path = path + i.ToString();
                    i++;
                }
            }
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception)
            {
                
            }
            
        }

        public List<string> ReadFile(string path) 
        {

            List<string> lines = new List<string>();

            StreamReader reader = new StreamReader(path);

            while (!reader.EndOfStream)
            {
                lines.Add(reader.ReadLine());
            }
            reader.Close();
            return lines;
          
        }

        public void SaveFile(string path, List<string> lines)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (var item in lines)
                {
                    sw.WriteLine(item);
                }
                
            }
        }

        public void Createtxt(string path)
        {
            try
            {
                using (StreamWriter sw = File.CreateText(path)) ;
            }
            catch (Exception)
            {

            }
          
        }

    }
}
    

