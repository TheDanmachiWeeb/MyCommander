using ConsoleApp7.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7.Components
{
        public abstract class Popups : Window
        { 
        public abstract override void HandleKey(ConsoleKeyInfo info);

        public abstract override void Draw();

        }
}
