using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7.Components
{
    public class Selectables : IComponent
    {
        private List<IComponent> components = new List<IComponent>();
        private int selected = 0;


        public void HandleKey(ConsoleKeyInfo info)
        {
   
        }

        public void Draw(int x, int y)
        {
        }
    }
}
