using ConsoleApp7.Windows;
using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using ConsoleApp7.Services;

namespace ConsoleApp7.Components
{
    public class Table : IComponent
    { 
        public bool Active { get; set; }
        public int StartingWidth { get; set; }

        public event Action TryOpeningFile;

        public event Action ToDrives;
        public event Action<string> Navigate;
        private string CurrentPath = Config.DefaultPath;
        private List<string> headers = new List<string>();
        private List<Row> rows = new List<Row>();
        FilesService service = new FilesService();
      
      

        private int offset = 0;
        public int Selected { get; set; } = 0;
        public int Count { get; set; } = Console.WindowHeight - 8;

        public Table(bool Active, int StartingWidth)
        {
            CurrentPath = "";
            this.headers.Add("Name");
            this.headers.Add("Size".PadLeft(8));
            this.headers.Add("Date");
            this.Active = Active;
            this.StartingWidth = StartingWidth; 
        }

        public void Add(List<Row> data)
        {
            rows = data;
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.UpArrow)
            {
                if (Active)
                {
                    if (Selected <= 0)
                        return;

                    Selected--;

                    if (Selected == offset - 1)
                        offset--;
                }
            }
            else if (info.Key == ConsoleKey.DownArrow)
            {
                if (Active)
                {
                    if (Selected >= rows.Count - 1)
                        return;

                    Selected++;

                    if (Selected == offset + Math.Min(Count, this.rows.Count))
                        offset++;
                }
            }
            else if (info.Key == ConsoleKey.Enter && (rows[Selected].Data[4] != "")) //going down (click on not ..)
            {
                if (Active)
                {
                    bool failedEntry = false;
                    int oldOffset = offset;
                    int oldSelected = Selected;
                 
                    //CurrentPath = "";
                    
                   
                    string path = this.rows[Selected].Data[4].ToString();

                    DirectoryInfo dir = new DirectoryInfo(path);
                    try
                    {
                        if (this.rows[Selected].Data[6].ToString() == "file") 
                        {
                            this.TryOpeningFile();
                        }
                        else { dir.GetDirectories(); }
                        
                    }
                    catch (UnauthorizedAccessException)
                    {
                        NoAccess();
                        failedEntry = true;
                        string oldPath = path.Substring(0, path.LastIndexOf("\\") + 1);
                        path = oldPath;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        string oldPath = path.Substring(0, path.LastIndexOf("\\") + 1);
                        path = oldPath;
                        dir = new DirectoryInfo(path);
                        dir.GetDirectories();
                    }
                   
                    if (failedEntry)
                    {
                        Selected = oldSelected;
                        offset = oldOffset;
                    }
                    else
                    {
                        Selected = 0;
                        offset = 0;
                    }
                    this.Navigate(path);
                    CurrentPath = path;
                }
            } 
            else if (info.Key == ConsoleKey.Enter && (rows[Selected].Data[4] == ""))    //going up (click on ..)
            {
                int oldSelected = Selected;
                bool NoPath = false;
                if (Active)
                {
                    string path = "";
                
                    if (this.rows.Count > 1)
                    {
                        path = this.rows[Selected + 1].Data[3].ToString();
                        this.Navigate(path);
                    
                    }
                    else
                    {
                        path = CurrentPath.Substring(0, CurrentPath.LastIndexOf("\\") + 1);
                        this.Navigate(path);
                    }
                    if (CurrentPath == path)
                    {
                        this.ToDrives();
                        NoPath = true;
                    }
                    
                    CurrentPath = path;

                    if (NoPath)
                    {
                        CurrentPath = "";
                        NoPath = false;
                    }
                }
            }
            else if (info.Key == ConsoleKey.F10) //move
            {
                if (Active)
                {
                    string CurrentPath = GetCurrentPath(true, false);
                    Selected--;
                }
            }
            else if (info.Key == ConsoleKey.F8) //delete
            {
                if (Active)
                {
                        Selected--;
                }
            }
        }
        
        public void Draw(int x, int y)
        {
            int NumberOfRows = rows.Count;
            List<int> widths = Widths();

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.SetCursorPosition(StartingWidth, 1 );

            DrawData(null, widths, '+', '-');
            DrawData(headers, widths, '|', ' ');
            DrawData(null, widths, '+', '-');
           
            for (int i = offset; i < offset + Math.Min(Count, NumberOfRows) ; i++)
            {
                if (Active)
                {
                    if (i == Selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                } 

                DrawData(rows[i].Data, widths, '|', ' ');
                Console.ResetColor();
            }

            if ((Console.WindowHeight - 8) > Math.Min(Count, NumberOfRows))
            {
                for (int i = 0; i < Console.WindowHeight - 8 - NumberOfRows; i++)
                {
                    DrawData(null, widths, '|', ' ');
                }
            }
            DrawData(null, widths, '+', '-');
            CurrentPathElement(CurrentPath);
            DrawData(null, widths, '─', '─');
            
        }

       private void DrawData(List<string>? data, List<int> widths, char sep, char pad)
        {
            
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            int i = 0;

                foreach (int width in widths)
                {
                    string value = data != null ? data[i] : "";
                    
                    Console.Write(sep);
                    Console.Write(pad);
                    Console.Write(value.PadRight(widths[i] + 1, pad));

                    i++;
                } 

            Console.Write(sep);
            Console.SetCursorPosition(x, y + 1);
          
        }
        private List<int> Widths()
        {
            //pak udelat aby dohromady byli full width
            int baseColumn = (int)Math.Floor(Console.WindowWidth / 4d);
            int sizeColumn = 8;
            int dateColumn = 10;
            while ((baseColumn + 18)  != Console.WindowWidth / 2)
            {
                if (baseColumn + 18 > Console.WindowWidth / 2)
                {
                    baseColumn--; 
                }
                else
                {
                    baseColumn++;
                }
            }
            
            List<int> widths = new List<int> { baseColumn-10, sizeColumn, dateColumn };
            return widths;
        }

        private void NoAccess()
        {
            int y = Console.WindowHeight-4;
            int x = StartingWidth + 4;

            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.SetCursorPosition(x, y);
            Console.Write("No Access to this directory");
            Thread.Sleep(1500);
            Console.ResetColor();
        }

        private void CurrentPathElement(string CurrentPath)  
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            int messageLenght = Console.WindowWidth / 2 - 6;
            string CurrentPathText = CurrentPath;
            CurrentPathText = CurrentPathText.PadRight(messageLenght + 1);
            string oldCurrentPath = CurrentPath;

            //if (CurrentPathText.Length > messageLenght + 1)
            //{
            //    //CurrentPathText = CurrentPathText.Remove(messageLenght, CurrentPathText.Length - messageLenght) + "~";
            //}

            string CurrentPathElement = "|   " + CurrentPathText + "|";

            Console.Write(CurrentPathElement);
            Console.SetCursorPosition(x, y + 1);
            CurrentPath = oldCurrentPath;
        }
        
        public string GetCurrentPath(bool active, bool withName)
        {
            string CurrentPathTechnical = "";
            if (Active == active)
            {
                CurrentPathTechnical = CurrentPath;
                if (withName)
                {
                    string selectedItemName = rows[Selected].Data[0];
                    if (CurrentPath.Length > 4)
                    {
                        CurrentPathTechnical = CurrentPath + "\\" + selectedItemName;
                    }
                    else
                    {
                        CurrentPathTechnical = CurrentPath + selectedItemName;
                    }
                }
            }
            return CurrentPathTechnical;
        }


        public string GetDirName()
        {
            string selectedItemName = "";
            if (Active)
            {
                selectedItemName = rows[Selected].Data[0];
            }
            return selectedItemName;
        }
    }
}
