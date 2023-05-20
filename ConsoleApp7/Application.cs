using ConsoleApp7.Components;
using ConsoleApp7.Services;
using ConsoleApp7.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApp7
{
    public class Application
    {
        private Window window; //udelat list jich
        private ListWindow listWindow = new ListWindow();
        public List<Window> Windows = new List<Window>();

        public Application()
        {
            listWindow = new ListWindow();
            Windows.Add(listWindow);

            foreach (var item in Windows)
            {
                this.SwitchWindow(item);
            }
            listWindow.MyCopyEvent += Table_MyCopy;
            listWindow.MyMkDirEvent += Table_MyMkDir;
            listWindow.MyDeleteEvent += Table_MyDelete;
            listWindow.ReadFileEvent += Table_ReadFile;

        }

        public void HandleKey(ConsoleKeyInfo info)
        {
            int last = Windows.ToList().Count - 1;
            Windows[last].HandleKey(info);
        }

        public void Draw()
        {
            int last = Windows.ToList().Count - 1;
            Windows[last].Draw();
        }

        public void SwitchWindow(Window window)
        {
            window.Application = this;
            this.window = window;
        }


        private void Table_MyCopy()
        {
            CopyPopup CopyGuy = new CopyPopup(listWindow);
            Windows.Add(CopyGuy); 
            CopyGuy.Remove += RemoveLast;
        }
           

        private void Table_MyDelete()
        {
            DeletePopup DeleteGuy = new DeletePopup(listWindow);
            Windows.Add(DeleteGuy);
            DeleteGuy.Remove += RemoveLast;
        }

        private void Table_MyMkDir()
        {
            MkdirPopup MkdirGuy = new MkdirPopup(listWindow);
            Windows.Add(MkdirGuy);
            MkdirGuy.Remove += RemoveLast;
        }

        private void RemoveLast()
        {
            int last = Windows.Count - 1;
            Windows.RemoveAt(last);
        }

        private void Table_ReadFile()
        {
            ReadFilePopup ReadFileGuy = new ReadFilePopup(listWindow);
            Windows.Add(ReadFileGuy);
            ReadFileGuy.Remove += RemoveLast;
        }
    }
}
