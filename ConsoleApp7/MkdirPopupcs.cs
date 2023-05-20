using ConsoleApp7.Components;
using ConsoleApp7.Services;
using ConsoleApp7.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp7
{
    internal class MkdirPopup : Popups
    {
        private List<IComponent> components = new List<IComponent>();
        private int selected = 0;
        public event Action Remove;
        ListWindow CurrentWindow;
        private string MkDirPath;
        private string DirName;
        private string regenPath;
        private bool txtFile = false;
        TextBox txtName = new TextBox() { Label = "Name" };

        public MkdirPopup(ListWindow CurrentWindow)
        {
            this.CurrentWindow = CurrentWindow;
            txtName.Clicked += TextBox_Clicked;

            Button btnCancel = new Button() { Title = "Cancel" };
            btnCancel.Clicked += BtnCancel_Clicked;

          
            this.components.Add(txtName);
            this.components.Add(btnCancel);

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
            int firstPos = Console.WindowWidth / 2 - 22; 
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
            else if (info.Key == ConsoleKey.F5)
            {
                txtFile = !txtFile;
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
            MkDirPath = CurrentWindow.GetPaths(5);
            Console.SetCursorPosition(origX + 3, origY + 1);
            if (txtFile == false)
            {
                Console.Write($"Create new directory in *{MkDirPath}*");
            }
            else if (txtFile == true)
            {
                Console.Write($"Create new file in *{MkDirPath}*");
            }
            Console.SetCursorPosition(origX + 3, origY + 3);
            if (txtFile == false)
            {
                Console.Write($"To create file, press F5");
            }
            else if (txtFile == true)
            {
                Console.Write($"To create directory, press F5");
            }
        }
        public void Functionality(string DirName)
        {
            FilesService service = new FilesService();
            if (txtFile == false)
            {
                if (DirName.Length < 1)
                {
                    DirName = "New Directory";
                }

                if (MkDirPath.Length > 5)
                {
                    string firstThree = MkDirPath.Substring(0, 3);
                    string secondThree = MkDirPath.Substring(3, 3);

                    if (firstThree == secondThree)
                    {
                        MkDirPath = firstThree;
                    }
                }

                MkDirPath = MkDirPath + "\\" + DirName;

                service.MkDir(MkDirPath);
            }
            else if (txtFile == true)
            {
                if (DirName.Length < 1)
                {
                    DirName = "New File";
                }

                if (MkDirPath.Length > 5)
                {
                    string firstThree = MkDirPath.Substring(0, 3);
                    string secondThree = MkDirPath.Substring(3, 3);

                    if (firstThree == secondThree)
                    {
                        MkDirPath = firstThree;
                    }
                }

                MkDirPath = MkDirPath + "\\" + DirName;
                service.Createtxt(MkDirPath);

                txtFile = false;
            }

                regenPath = CurrentWindow.GetPaths(4);

                if (regenPath == "")
                {
                    regenPath = CurrentWindow.GetPaths(0);
                }

                CurrentWindow.Table_Navigate(regenPath);
           
        }
        private void BtnCancel_Clicked()
        {
            this.Remove();
        }
        private void TextBox_Clicked()
        {
            DirName = txtName.Value;
            Functionality(DirName);
            this.Remove();
        }
    }
}

