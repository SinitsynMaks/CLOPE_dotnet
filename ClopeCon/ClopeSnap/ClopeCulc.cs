using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    public class ClopeCulc
    {
        private double repulsCoeff;
        private Dictionary<int, ClopeCluster> clusterDict = new Dictionary<int, ClopeCluster>();
        private List<Transaction> transactionsTable = new List<Transaction>();

        public ClopeCulc(double repulsion)
        {
            repulsCoeff = repulsion;
        }

        public void Initialise(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);

            // Максимальное добавление - опорное значение
            double maxDelta;

            // Текущее добавление либо к пустому либо к существующему кластеру
            // Сравнивается с опорным
            double delta;

            // Получили самую первую транзакцию
            string str = sr.ReadLine();
            Transaction firstTransaction = str.GetTransaction();

            // Кладем самую первую транзакцию в самый первый кластер
            firstTransaction.ClusterNumber = 1;
            var firstCluster = new ClopeCluster();
            firstCluster.AddTransaction(firstTransaction); //Транзакция лежит в коллеции своего кластера
            clusterDict.Add(1, firstCluster); // Первый кластер лежит в словаре под номером 1
            transactionsTable.Add(firstTransaction); // Транзакции из файла хранятся в коллекции

            // Теперь пошли циклом по остальным транзакциям
            while (!sr.EndOfStream)
            {
                //Получаем очередную транзакцию
                Transaction currentTransaction = sr.ReadLine().GetTransaction();

                //Создаем новй пустой кластер-приемник. определим, каким он будет
                var clustNew = new ClopeCluster();

                //Считаем цену добавления транз в новый(пустой) кластер
                //Это значение - отправная точка для нас
                maxDelta = DeltaAdd(null, currentTransaction, repulsCoeff);

                var currentClustersCount = clusterDict.Count;
                for (int i = 0; i < currentClustersCount; i++)
                {
                    delta = DeltaAdd(clusterDict[i + 1], currentTransaction, repulsCoeff);

                    if (delta > maxDelta)
                    {
                        maxDelta = delta;
                        clustNew = clusterDict[i + 1]; //Пока кластер-приемник у нас будет этот текущий

                        //Отметили принадлежность транзакции к существующему кластеру
                        currentTransaction.ClusterNumber = i + 1;
                    }
                }

                //Если все таки положить в пустой кластер оказалось выгоднее, кладем туда
                if (clustNew.TransCount == 0)
                {
                    var newClustNumb = clusterDict.Keys.Max() + 1; //У нового кластера - новый номер по порядку
                    currentTransaction.ClusterNumber = newClustNumb; //Транзакция пока принадлежит этому кластеру
                    clustNew.AddTransaction(currentTransaction); //Кладем в новый кластер очередную транзакцию
                    clusterDict.Add(newClustNumb, clustNew); //Положили кластер в общий словарик
                }
                else
                {
                    //Если же это один из существующих кластеров
                    clustNew.AddTransaction(currentTransaction); //Добавляем в него транзакцию
                    clusterDict[currentTransaction.ClusterNumber] = clustNew;//Обновляем в словаре этот кластер
                }

                //Транзакцию кладем в наш кэш-лист для второй итерации
                transactionsTable.Add(currentTransaction);
            }
            sr.Close();

            Console.WriteLine("Результаты фазы инициализации:");
            foreach (var kvp in clusterDict)
            {
                Console.WriteLine($"Кластер №{kvp.Key} - {kvp.Value.TransCount} транзакций," +
                                  $" p={(kvp.Value.TransactionList.Where(t => t.IsPoison)).Count()}," +
                                  $" e={(kvp.Value.TransactionList.Where(t => !t.IsPoison)).Count()}");
            }
            Console.WriteLine($"Общее количество кластеров - {clusterDict.Count}");
            Console.WriteLine($"Общее число транзакций в словаре - {clusterDict.Values.Select(k => k.TransCount).Sum()}");
            Console.WriteLine($"Число транзакций в листе с номером кластера 12 - {transactionsTable.Count(t => t.ClusterNumber == 12)}");
            Console.WriteLine("-------------------------------------------");
        }

        public void Iterate()
        {
            int countIteration = 0;

            bool moved = true;
            while (moved) // цикл по итерациям
            {
                countIteration++;
                moved = false;

                var count1 = transactionsTable.Count(t => t.ClusterNumber == 12);
                var t1 = transactionsTable[3001];
                var t2 = transactionsTable[3040];

                for  (int i = 0; i < transactionsTable.Count; i++)
                {
                    // Цена добавления транзакции в новый кластер. Отправная точка сравнения
                    double maxDelta = DeltaAdd(null, transactionsTable[i], repulsCoeff);

                    // Запоминаем номер старого кластера, в котором лежала транзакция
                    var oldClusterNumber = transactionsTable[i].ClusterNumber;
                    var newClusterNumber = 0;

                    // Узнаем цену удаления транзакции из ее текущего кластера
                    double removeDelta = DeltaRemove(clusterDict[oldClusterNumber], transactionsTable[i], repulsCoeff);

                    //Теперь перебираем все кластеры, кроме того, где лежала наша транзакция
                    //и узнаем стоимость запиха транзакции туда
                    var arrayWithoutCurrentCluster = clusterDict.Where(kvp => kvp.Key != transactionsTable[i].ClusterNumber).ToArray();
                    foreach (KeyValuePair<int, ClopeCluster> pair in arrayWithoutCurrentCluster)
                    {
                        double addDelta = DeltaAdd(pair.Value, transactionsTable[i], repulsCoeff);

                        // Если стоимость от добавления транзакции в существующий кластер больше, чем в новый
                        // предварительно решаем положить транзакцию в этот существующий кластер
                        if (addDelta > maxDelta)
                        {
                            maxDelta = addDelta;

                            // Запомнили новый кластер у транзакции
                            newClusterNumber = pair.Key;
                            transactionsTable[i].ClusterNumber = newClusterNumber;
                        }
                    }

                    // Окончательное решение, куда выгоднее положить
                    if (maxDelta + removeDelta > 0)
                    {
                        //Если не нашлось подходящих существующих кластеров
                        if (newClusterNumber == 0)
                        {
                            var clast = new ClopeCluster();
                            clusterDict[oldClusterNumber].RemoveTransaction(transactionsTable[i]);
                            newClusterNumber = clusterDict.Count + 1;
                            transactionsTable[i].ClusterNumber = newClusterNumber;
                            clusterDict.Add(newClusterNumber, clast);
                            clusterDict[newClusterNumber].AddTransaction(transactionsTable[i]);
                        }
                        // Если имеющийся кластер нашелся
                        else
                        {
                            clusterDict[oldClusterNumber].RemoveTransaction(transactionsTable[i]);
                            clusterDict[newClusterNumber].AddTransaction(transactionsTable[i]);
                        }

                        // В любом случае перестановка имела место, вторая итерация будет
                        moved = true;

                        var count = transactionsTable.Count(t => t.ClusterNumber == 12);
                        var position1 = transactionsTable.FindIndex(t => t.ClusterNumber == 12);
                        var pos2 = transactionsTable.FindIndex(position1+1,t => t.ClusterNumber == 12);
                    }
                }
            }
            // Удаляем пустые кластеры
            var clusterDictNew = clusterDict.Where(kvp => kvp.Value.TransCount != 0).ToDictionary(k => k.Key, k => k.Value);
            var clustDel = clusterDict.Where(kvp => kvp.Value.TransCount == 0);

            Console.WriteLine("Результаты фазы итерации:");
            Console.WriteLine($"Количество итераций: {countIteration}");
            foreach (var kvp in clusterDictNew)
            {
                Console.WriteLine($"Кластер №{kvp.Key} - {kvp.Value.TransCount} транзакций," +
                                  $" p={(kvp.Value.TransactionList.Where(t => t.IsPoison)).Count()}," +
                                  $" e={(kvp.Value.TransactionList.Where(t => !t.IsPoison)).Count()}");
            }
            Console.WriteLine($"Количество непустых кластеров - {clusterDictNew.Count}");
            Console.WriteLine($"Исчезли кластеры:");
            foreach (var clust in clustDel)
            {
                Console.WriteLine(clust.Key);
            }
            Console.WriteLine($"Общее число транзакций - {clusterDictNew.Values.Select(k => k.TransCount).Sum()}");
            Console.WriteLine("-------------------------------------------");
        }

        private double DeltaAdd(ClopeCluster cluster, Transaction transaction, double r)
        {
            if (cluster == null)
                return transaction.ArrayValues.Length / Math.Pow(transaction.UniqueParameters, r);

            double squareNew = cluster.Square + transaction.ArrayValues.Length;
            double widthNew = cluster.Width;
            for (int i = 0; i < transaction.ArrayValues.Length; i++)
                if (!cluster.Occ.ContainsKey(transaction.ArrayValues[i]))
                    widthNew++;
            var res = squareNew * (cluster.TransCount + 1) / Math.Pow(widthNew, r) - cluster.Square * cluster.TransCount / Math.Pow(cluster.Width, r);
            return res;
        }

        private double DeltaRemove(ClopeCluster cluster, Transaction transaction, double r)
        {
            double squareNew = cluster.Square - transaction.ArrayValues.Length;
            double widthNew = cluster.Width;
            for (int i = 0; i < transaction.ArrayValues.Length; i++)
                if (cluster.Occ[transaction.ArrayValues[i]] == 1)
                    widthNew--;
            return squareNew * (cluster.TransCount - 1) / Math.Pow(widthNew, r) - cluster.Square * cluster.TransCount / Math.Pow(cluster.Width, r);
        }
    }
}
