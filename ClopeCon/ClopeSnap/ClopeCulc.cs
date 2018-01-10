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
                foreach (var tr in transactionsTable)
                {
                    // Цена добавления транзакции в новый кластер
                    double newDelta = DeltaAdd(null, tr, repulsCoeff); ;
                    var oldClusterNumber = tr.ClusterNumber;
                    var newClusterNumber = 0;

                    // Узнаем цену удаления транзакции из ее текущего кластера
                    double removeDelta = DeltaRemove(clusterDict[oldClusterNumber], tr, repulsCoeff);

                    // С этой опорной величиной добавления транзакции в новый кластер
                    // мы будем сравнивать перетасовку транзакций в имеющиеся кластеры
                    double maxDelta = newDelta + removeDelta;

                    //Теперь перебираем все кластеры, кроме того, где лежала наша транзакция
                    //и узнаем стоимость запиха транзакции туда
                    var arrayWithoutCurrentTrtansaction = clusterDict.Where(kvp => kvp.Key != tr.ClusterNumber).ToArray();
                    foreach (KeyValuePair<int, ClopeCluster> pair in arrayWithoutCurrentTrtansaction)
                    {
                        double addDelta = DeltaAdd(pair.Value, tr, repulsCoeff);

                        // Если суммарная стоимость от удаления и добавления в существующий кластер больше, чем в новый,
                        // решаем положить транзакцию в этот существующий кластер
                        if (addDelta + removeDelta > maxDelta)
                        {
                            maxDelta = addDelta + removeDelta;

                            // Поставили пока новый кластер у транзакции
                            newClusterNumber = pair.Key;
                            tr.ClusterNumber = newClusterNumber;
                        }
                    }

                    if (maxDelta > (newDelta + removeDelta))
                    {
                        // Если в имеющийся кластер оказалось положить выгоднее, кладем туда
                        clusterDict[oldClusterNumber].RemoveTransaction(tr);
                        clusterDict[newClusterNumber].AddTransaction(tr);
                        moved = true;
                    }
                    // Иначе кладем в новый кластер
                    else
                    {
                        var clast = new ClopeCluster();
                        clusterDict[oldClusterNumber].RemoveTransaction(tr);
                        newClusterNumber = clusterDict.Count + 1;
                        tr.ClusterNumber = newClusterNumber;
                        clusterDict.Add(newClusterNumber, clast);
                        clusterDict[newClusterNumber].AddTransaction(tr);
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
