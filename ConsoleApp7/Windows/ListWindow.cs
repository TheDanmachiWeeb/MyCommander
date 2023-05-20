using ConsoleApp7.Components;
using ConsoleApp7.Services;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp7.Windows
{
    public class ListWindow : Window
    {
        private List<string> UpperRow = new List<string> { "Left", "File", "Command", "Options", "Right" };
        private List<string> DownRow = new List<string> { "------", "------", "------", "------", "Copy", "------", "Mkdir", "Delete", "------", "Move" };

        public event Action MyCopyEvent;
        public event Action MyDeleteEvent;
        public event Action MyMkDirEvent;
        public event Action ReadFileEvent;

        public event Action Open;

        private Table TableRight { get; set; }
        private Table TableLeft { get; set; }

        public List<Table> tables = new List<Table>();

     
        public ListWindow ()
        {
            TableRight = new Table(true, 0);
            TableLeft = new Table(false, Console.WindowWidth/2);
            tables.Add(TableLeft);
            tables.Add(TableRight);
            foreach (var Table in tables)
            {
                FilesService service = new FilesService();

                List<Row> rows = service.DrivesData();//(Config.DefaultPath);
                {
                    Table.Add(rows);        
                }
                Table.ToDrives += Table_ToDrives;
                Table.Navigate += Table_Navigate;

                // Table.MyCopyEvent += Table_MyCopy;
                //Table.MyDeleteEvent += Table_MyDelete;
                //Table.MyMkDirEvent += Table_MyMkDir;
                Table.TryOpeningFile += Table_OpenFile;
            }
           
        }
        public void Table_Navigate(string path)
        {
            
            FilesService service = new FilesService();
            List<Row> rows = service.Data(path); 

            foreach (var Table in tables)
            {
               if (Table.Active == true)
               {
                    Table.Add(rows);
                }
            }
        }
      

        private void Table_ToDrives ()
        {
            FilesService service = new FilesService();
            List<Row> rows = service.DrivesData();

            foreach (var Table in tables)
            {
                if (Table.Active == true)
                {
                    Table.Add(rows);
                }
            }
        }

        public override void Draw()
        {
            this.TableLeft.Draw(0,0);
            this.TableRight.Draw(0,0);
            DownElement(DownRow);
            UpperElement(UpperRow);
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Tab) 
            {
                TableLeft.Active = !TableLeft.Active;
                TableRight.Active = !TableRight.Active;
            }
            else if (info.Key == ConsoleKey.F5) //copy
            {
                    try
                    {
                        this.MyCopyEvent();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        NoAccess();
                    } 
            }
            else if (info.Key == ConsoleKey.F8) //delete
            {
                    try
                    {
                        this.MyDeleteEvent();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        NoAccess();
                    }
            }
            else if (info.Key == ConsoleKey.F7) //mkdir
            {
                    try
                    {

                        this.MyMkDirEvent();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        NoAccess();
                    }
            }
            else if (info.Key == ConsoleKey.F10) //move
            {
                    try
                    {
                        this.MyDeleteEvent();
                        this.MyCopyEvent();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        NoAccess();
                    }
            }

            TableLeft.HandleKey(info);
            TableRight.HandleKey(info);
        }


        private void DownElement(List<string> data)
        {
            int RealLenght = 0;
            int x = 0;
            int y = Console.CursorTop;
            foreach (var item in data)
            {
                RealLenght = RealLenght + 11;
            }
            x = (Console.WindowWidth - RealLenght) / 2;

            Console.SetCursorPosition(x, y);
            for (int i = 0; i < data.Count; i++)
            {
                Console.ResetColor();
                int index = i + 1;
                string number = index.ToString();
                Console.Write(number.PadLeft(2).PadRight(3));
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(data[i].PadRight(7).PadLeft(8));
                Console.ResetColor();
            }
          //  Console.SetCursorPosition(x, y + 1);
        }
        private void UpperElement(List<string> data)
        {
            int x = 0;
            int y = 0;
            int lenght = 0;
            Console.SetCursorPosition(x, y);
            for (int i = 0; i < data.Count; i++)
            {
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("  ");
                Console.Write(data[i]);
                Console.Write("   ");
                lenght = lenght + data[i].Length + 5;
            }
            string other = "".PadRight(Console.WindowWidth - lenght);
            Console.WriteLine(other);
            Console.ResetColor();
        }

        private void NoAccess()
        {
                int x = 0;
                int y = 0;
                string message = "  No Access to this directory";
                int lenght = message.Length;
                string other = "".PadRight(Console.WindowWidth - lenght);

                Console.SetCursorPosition(x, y);

                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Magenta;

                Console.Write(message.ToUpper());
                Console.WriteLine(other);
                
                Thread.Sleep(1500);
                Console.ResetColor();
        }

        public string GetPaths(int argument)
        {
             string DestinationPath = "";
             string OriginPath = "";
             string CopyPath = "";
             string name = "";
             string samepath = "";
             string mkdirpath = "";

        FilesService service = new FilesService();
            if (TableLeft.Active)
            {

                OriginPath = TableLeft.GetCurrentPath(true, true);
                name = TableLeft.GetDirName();
                samepath = TableLeft.GetCurrentPath(true, false);
                mkdirpath = TableLeft.GetCurrentPath(true, true); //mkdirpath

                DestinationPath = TableRight.GetCurrentPath(false, true); //not same as mkdirpath
                CopyPath = DestinationPath;
                DestinationPath = DestinationPath + "\\" + name;
            }
            else if (TableRight.Active)
            {
                OriginPath = TableRight.GetCurrentPath(true, true); //same as delpath
                name = TableRight.GetDirName();
                samepath = TableRight.GetCurrentPath(true, false);
                mkdirpath = TableRight.GetCurrentPath(true, true);

                DestinationPath = TableLeft.GetCurrentPath(false, true);
                CopyPath = DestinationPath;
                DestinationPath = DestinationPath + "\\" + name;
            }
            if (argument == 0)
            {
                return OriginPath;
            }
            else if (argument == 1)
            {
                return name;
            }
            else if (argument == 2)
            {
                return CopyPath;
            }
            else if (argument == 3)
            {
                return DestinationPath;
            }
            else if (argument == 4)
            {
                return samepath;
            }
            else if (argument == 5)
            {
                return mkdirpath;
            }
        
            else return "ur fucked";
        }

        private void Table_OpenFile()
        {
            this.ReadFileEvent();
        }
    }
}

