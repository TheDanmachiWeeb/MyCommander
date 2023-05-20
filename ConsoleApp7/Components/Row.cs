using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7.Components
{
    public class Row
    {
        public List<string> Data { get; set; } = new List<string>();

        public Row(string[] data)
        {
            Data = new List<string>(data);
        }
    }
}
