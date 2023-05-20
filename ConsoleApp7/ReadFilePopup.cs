using ConsoleApp7.Components;
using ConsoleApp7.Services;
using ConsoleApp7.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7
{
    internal class ReadFilePopup : Popups
    {
        private string path;
        private List<string> lines;
        public event Action Remove;
        private int selectedX = 2;
        private int selectedY = 2;
        private int selectedRow = 0;
        private bool firstTime = true;
        private int oldSelectedX;

        public ReadFilePopup(ListWindow CurrentWindow)
        {
            Console.CursorVisible = true;
            path = CurrentWindow.GetPaths(0);
            Functionality(path);


        }
        public override void Draw()
        {
            Console.CursorVisible = false;
            int Width = Console.WindowWidth;
            int Height = Console.WindowHeight-1;
            Background(1, 1, Width, Height);

            int i = 0;
            try
            {
                foreach (var item in lines)
                {
                    Console.SetCursorPosition(2, i + 2);
                    Console.Write(lines[i]);
                    i++;
                }
                Console.ResetColor();
                Console.SetCursorPosition(selectedX, selectedY);
                Console.CursorVisible = true;
            }
            catch (Exception)
            {
                ConsoleKeyInfo info = new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false);
                HandleKey(info);            
            }
        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Delete)
            {
                Console.CursorVisible = false;
                this.Remove();
            }
            else if (info.Key == ConsoleKey.LeftArrow)
            {
                if (selectedX > 2)
                {
                    selectedX--;
                }
            }
            else if (info.Key == ConsoleKey.RightArrow)
            {
                if (selectedX <= lines[selectedRow].Length)
                {
                    selectedX++;
                }
            }
            else if (info.Key == ConsoleKey.Enter)
            {
                firstTime = true;
                lines.Insert(selectedRow + 1, " ");
                selectedX = 1;
                selectedY++;
                selectedRow++;
            }
            else if (info.Key == ConsoleKey.Backspace)
            {
                if (lines[selectedRow].Length < 2 && selectedRow > 0)
                {
                    try
                    {
                        selectedRow--;
                        selectedY--;
                        if (lines[selectedRow].Length > 1) 
                        {
                            selectedX = lines[selectedRow].Length;
                        }
;                   }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                    

                try
                {
                    lines[selectedRow] = lines[selectedRow].Remove(selectedX-2, 1);
                }
                catch (Exception)
                {
                    
                }
                if (selectedX > 2)
                {
                    selectedX--;
                }
            }
            else if (info.Key == ConsoleKey.UpArrow)
            {
                if (selectedRow > 0)
                {
                    selectedRow--;
                    selectedY--;
                    if (lines[selectedRow].Length < selectedX - 2)
                    {
                        selectedX = 2;
                    }
                }
            }
            else if (info.Key == ConsoleKey.DownArrow)
            {
                if (selectedY < Console.WindowHeight-2)
                {
                    selectedY++;
                    selectedRow++;
                    if (lines.Count()-1 <  selectedRow)
                    {
                        lines.Add("");
                    }
                    if (lines[selectedRow].Length < selectedX - 2) 
                    {
                        selectedX = 2;
                    }
                }
            }
            else if (info.Key == ConsoleKey.F1)
            {
                Save(path, lines);
            }
            else if (info.Key == ConsoleKey.F2)
            {
                Save(path, lines);
                Console.CursorVisible = false;
                this.Remove();
            }
            else
            {
                if (lines[selectedRow].Length < Console.WindowWidth-2)
                {
                    try
                    {
                        lines[selectedRow] = lines[selectedRow].Insert(selectedX - 1, info.KeyChar.ToString());
                        selectedX++;
                    }
                    catch (Exception) { lines[selectedRow] = info.KeyChar.ToString(); }
                }
            }
          
        }

        private void Background(int x, int y, int Width, int Height)
        {
          
            string background = "".PadRight(Width, ' ');
            int origX = x;
            int origY = y;
            if (firstTime == true)
            {
                for (int i = 0; i < Height - 1; i++)
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(background);
                    y++;
                }
                for (int i = 0; i < Console.WindowWidth; i++)
                {
                    Console.SetCursorPosition(i, 1);
                    Console.Write("-");
                }
                firstTime = false;
            }
            else
            {
                Console.SetCursorPosition(x, selectedY);
                Console.Write(background);
            }
            
            Console.ResetColor();
        }

        public void Functionality(string path)
        {
            FilesService service = new FilesService();
            lines = service.ReadFile(path);
            if (lines.Count == 0)
            {
                lines.Add(" ");
            }
        }
        private void Save(string path, List<string> lines)
        {
            FilesService service = new FilesService();
            service.SaveFile(path, lines);
        }
    }
}


