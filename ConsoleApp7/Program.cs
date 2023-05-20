using ConsoleApp7.Components;
using ConsoleApp7.Services;

namespace ConsoleApp7
{
    internal class Program
    {
        static void Main(string[] args)
        {
          
            Console.CursorVisible = false;

            FilesService servis = new FilesService();
   
            Application app = new Application();

            while (true)
            {
                app.Draw();
                ConsoleKeyInfo info = Console.ReadKey();
                app.HandleKey(info);
            }
        }
    }
}