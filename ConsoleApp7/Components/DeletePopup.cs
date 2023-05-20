using ConsoleApp7.Services;
using ConsoleApp7.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7.Components
{
    internal class DeletePopup : Popups
    {
            private List<IComponent> components = new List<IComponent>();
            private int selected = 0;
            public event Action Remove;
            ListWindow CurrentWindow;
            private string delPath;
            private string itemFutureLocation;
            private string originPathWithoutName;

            public DeletePopup(ListWindow CurrentWindow)
            {
                this.CurrentWindow = CurrentWindow;

                Button btnOk = new Button() { Title = "OK" };
                btnOk.Clicked += BtnOk_Clicked;

                Button btnCancel = new Button() { Title = "Cancel" };
                btnCancel.Clicked += BtnCancel_Clicked;

                this.components.Add(btnOk);
                this.components.Add(btnCancel);

                delPath = CurrentWindow.GetPaths(0);
                itemFutureLocation = CurrentWindow.GetPaths(2); //selected table path + selected row name = for moving
                originPathWithoutName = CurrentWindow.GetPaths(4);
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
                int maxLenght = 25;
                Console.SetCursorPosition(origX + 3, origY + 1);
                string localdelPath = delPath;

                if (localdelPath.Length > maxLenght)
                {
                    int difference = localdelPath.Length - maxLenght;
                    localdelPath = localdelPath.Remove(0, difference - 1);
                    localdelPath = "~" + localdelPath;
                }

                Console.Write($"Do you really wanna delete *{localdelPath}*");

                Console.SetCursorPosition(origX + 6, origY + 3);
                string localOrig = originPathWithoutName;
                if (localOrig.Length > maxLenght)
                {
                    int difference = localOrig.Length - maxLenght;
                    localOrig = localOrig.Remove(0, difference - 1);
                    localOrig = "~" + localOrig;
                }

                Console.Write($"from        {localOrig}");
            }
            public void Functionality()
            {

            FilesService service = new FilesService();

                if (delPath != itemFutureLocation)
                {
                    service.DeleteItem(delPath);
                }
                else
                {
                    Console.WriteLine("Nothing will be deleted");
                    Console.ReadLine();
                }

            string regenPath = CurrentWindow.GetPaths(4); //navigate to folder where you deleted something
            CurrentWindow.Table_Navigate(regenPath);

            }   
            private void BtnCancel_Clicked()
            {
                this.Remove();
            }
            private void BtnOk_Clicked()
            {
                Functionality();
                this.Remove();
            }
        }
    }


