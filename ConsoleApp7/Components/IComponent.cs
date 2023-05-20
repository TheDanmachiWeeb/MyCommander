using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7.Components
{
    public interface IComponent
    {
        public void HandleKey(ConsoleKeyInfo info);

        public void Draw(int x, int y);
    }
}
