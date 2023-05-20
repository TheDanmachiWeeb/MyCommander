using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7.Components
{
    public class TextBox : IComponent
    {
        public string Label { get; set; } = "";

        public event Action Clicked;
        public string Value { get; set; } = "";

        public int Size { get; set; } = 20;

        public void Draw(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(this.Label + ":");
            Console.SetCursorPosition(x + this.Label.Length, y);
            Console.Write(":" + this.Value.PadRight(this.Size, '_'));

  
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Enter)
            {
                this.Clicked();
            }
            else if (info.Key == ConsoleKey.Backspace)
            {
                if (this.Value.Length == 0)
                    return;

                this.Value = this.Value.Substring(0, this.Value.Length - 1);

            }
            else
            {
                if (this.Value.Length < 20)
                {
                    this.Value += info.KeyChar;
                }
            }

        }
    }
}
