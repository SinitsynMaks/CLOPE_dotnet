using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClopeCon.ClopeSnap;

namespace ClopeCon
{
    class Program
    {
        const string inputDataFilepath = "../../DataFiles/OriginDataSet.txt";
        const string preparedDataFilePath = "../../DataFiles/PreparedData.txt";

        public static void Main(string[] args)
        {
            Console.WriteLine("Выполняется инициализация данных");
            MushroomDataSet.Normalize(inputDataFilepath, preparedDataFilePath);

            double repulsion = 2.6;

            /*
            while (true)
            {
                Console.WriteLine("Введите коэффициэнт отталкивания r");
                string r = Console.ReadLine();
                if (!double.TryParse(r, out repulsion))
                {
                    Console.WriteLine("Параметр отталкивания r введен неверно");
                    continue;
                }
                break;
            }
            */

            //Task tasks = Task.Run(() => ClopeCulc.Execute(preparedDataFilePath, repulsion));
            //tasks.Wait();

            Console.WriteLine($"r = {repulsion}");
            var clope = new ClopeCulc(repulsion);
            clope.Initialise(preparedDataFilePath);
            clope.Iterate();

            Console.ReadKey();
        }
    }
}
