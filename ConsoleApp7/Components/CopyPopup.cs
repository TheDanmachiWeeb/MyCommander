using ConsoleApp7.Services;
using ConsoleApp7.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7.Components
{
    internal class CopyPopup : Popups
    {
        private List<IComponent> components = new List<IComponent>();
        private int selected = 0;
        public event Action Remove;
        ListWindow CurrentWindow;
        private bool clicked = false;
        private string originPath;
        private string copyPath;
        private string name;
        private string originPathWithoutName;

        public CopyPopup(ListWindow CurrentWindow)
        {
            this.CurrentWindow = CurrentWindow;

            Button btnOk = new Button() { Title = "OK" };
            btnOk.Clicked += BtnOk_Clicked;

            Button btnCancel = new Button() { Title = "Cancel" };
            btnCancel.Clicked += BtnCancel_Clicked;

            this.components.Add(btnOk);
            this.components.Add(btnCancel);

            originPath = CurrentWindow.GetPaths(0);
            name = CurrentWindow.GetPaths(1);
            copyPath = CurrentWindow.GetPaths(2);
            originPathWithoutName = CurrentWindow.GetPaths(4);

            if (copyPath.Length > 5)
            {
                string firstThree = copyPath.Substring(0, 3);
                string secondThree = copyPath.Substring(3, 3);

                if (firstThree == secondThree)
                {
                    copyPath = firstThree;
                }
            }
        
        }
        public override void Draw()
        {
            int Width = Console.WindowWidth / 2;
            int Height = Console.WindowHeight - 8;
            int x = Console.WindowWidth / 4;
            int y = 4;

            Background(x, y, Width, Height);

            int i = 0;
            int startHeight = (Console.WindowHeight / 4) + 4;
            int firstPos = Console.WindowWidth / 2 - 8 - 4; //-4 = lenght of message so its centered
            int secondPos = Console.WindowWidth / 2 + 8;
            bool switcher = false;
        
            
                foreach (IComponent item in this.components)
                {

                    if (i == this.selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    if (switcher == false)
                    {
                        item.Draw(firstPos, startHeight);
                        switcher = !switcher;
                    }
                    else if (switcher == true)
                    {
                        item.Draw(secondPos, startHeight);
                        switcher = !switcher;
                    }
                    Console.ResetColor();
                    i++;
                }
            

        }

        public override void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Tab)
            {
                this.selected = (this.selected + 1) % this.components.Count;
            }
            else
            {
                this.components[this.selected].HandleKey(info);
            }
        }

        private void Background(int x, int y, int Width, int Height)
        {
            string background = "".PadRight(Width, ' ');
            int origX = x;
            int origY = y;

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.Black;
            for (int i = 0; i < Height; i++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(background);
                y++;
            }
            Console.ResetColor();

            LocalText(origX, origY);
          
        }

        private void LocalText(int origX, int origY)
        {
            Console.SetCursorPosition(origX + 3, origY + 1);
            Console.Write($"Do you really wanna copy *{name}*");

            Console.SetCursorPosition(origX + 6, origY + 3);
            Console.Write($"from {originPathWithoutName}");

            Console.SetCursorPosition(origX + 6, origY + 4);
            Console.Write($"to   {copyPath}");
        }
        public void Functionality()
        {
            FilesService service = new FilesService();

            copyPath = copyPath + "\\" + name;

            try
            {
                service.CopyDirectory(originPath, copyPath, true);
            }
            catch (IOException)
            {
                service.CopyFile(originPath, copyPath, true);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("No Access");
            }
        }
        private void BtnCancel_Clicked()
        {
            clicked = true;
            this.Remove();
        }
        private void BtnOk_Clicked()
        {
            clicked = true;
            Functionality();
            this.Remove();
        }
    }
}
