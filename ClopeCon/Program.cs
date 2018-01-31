using System;
using System.Collections;
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
       // const string preparedDataFilePath = "../../DataFiles/PreparedData.txt";

        public static void Main(string[] args)
        {
            //IDataSetProvider dsProvider = new MushroomDataSetProvider(inputDataFilepath);
            IEnumerator dsProvider = new UniversalDSProvider(inputDataFilepath);

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
            //var clope = new ClopeCulc(dsProvider, repulsion);
            var clope = new ClopeCulc2(dsProvider, repulsion);

            var initResult = clope.Initialise();

            Console.WriteLine("Результаты фазы инициализации:");

            foreach (var kvp in initResult.ClustersTable)
            {
                Console.WriteLine($"Кластер №{kvp.Key} - {kvp.Value.TransCount} транзакций," +
                                  $" p={(kvp.Value.TransactionList.Where(t => t.IsPoison)).Count()}," +
                                  $" e={(kvp.Value.TransactionList.Where(t => !t.IsPoison)).Count()}");
            }
            Console.WriteLine($"Общее количество кластеров - {initResult.ClustersTable.Count}");
            Console.WriteLine($"Общее число транзакций в словаре - {initResult.ClustersTable.Values.Select(k => k.TransCount).Sum()}");
            //Console.WriteLine($"Число транзакций в листе с номером кластера 12 - {InitResult.TransactionsTable.Count(t => t.ClusterNumber == 12)}");
            Console.WriteLine("-------------------------------------------");

            var iterateResult = clope.Iterate();

            Console.WriteLine("Результаты фазы итерации:");
            Console.WriteLine($"Количество итераций: {iterateResult.IterationCount}");
            foreach (var kvp in iterateResult.ClustersTable)
            {
                Console.WriteLine($"Кластер №{kvp.Key} - {kvp.Value.TransCount} транзакций," +
                                  $" p={(kvp.Value.TransactionList.Where(t => t.IsPoison)).Count()}," +
                                  $" e={(kvp.Value.TransactionList.Where(t => !t.IsPoison)).Count()}");
            }
            Console.WriteLine($"Количество непустых кластеров - {iterateResult.ClustersTable.Count}");
            Console.WriteLine($"Исчезли кластеры:");
            foreach (var clustNumb in initResult.ClustersTable.Keys.Where(t => !iterateResult.ClustersTable.Keys.Contains(t)))
            {
                Console.WriteLine(clustNumb);
            }
            Console.WriteLine($"Добавились новые кластеры:");
            foreach (var clustNumb1 in iterateResult.ClustersTable.Keys.Where(t => t > initResult.ClustersTable.Keys.Max()))
            {
                Console.WriteLine(clustNumb1);
            }
            Console.WriteLine($"Общее число транзакций в словаре - {iterateResult.ClustersTable.Values.Select(k => k.TransCount).Sum()}");
            Console.WriteLine("-------------------------------------------");

            Console.ReadKey();
        }
    }
}
