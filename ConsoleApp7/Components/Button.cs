using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7.Components
{
    public class Button : IComponent
    {
        public event Action Clicked;

        public string Title { get; set; } = "";
        
        public  void Draw(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write($"[ {this.Title} ]");
        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            if (info.Key == ConsoleKey.Enter)
            {
                this.Clicked();
            }
        }
    }
}
