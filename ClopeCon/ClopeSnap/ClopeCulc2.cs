using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    class ClopeCulc2
    {
        private IEnumerator _dsProvider;
        private double repulsCoeff;
        private Dictionary<int, ClopeCluster> clusterDict = new Dictionary<int, ClopeCluster>();
        private List<Transaction> transactionsTable = new List<Transaction>();

        public ClopeCulc2(IEnumerator dsProvider, double repulsion)
        {
            _dsProvider = dsProvider;
            repulsCoeff = repulsion;
        }

        public CalculationResult Initialise()
        {
            //StreamReader sr = new StreamReader(filePath);


            // Максимальное добавление - опорное значение
            double maxDelta;

            // Текущее добавление либо к пустому либо к существующему кластеру
            // Сравнивается с опорным
            double delta;

            // Получили самую первую транзакцию
            if (_dsProvider.MoveNext())
            {
                Transaction firstTransaction = (Transaction)_dsProvider.Current;
                // Кладем самую первую транзакцию в самый первый кластер
                firstTransaction.ClusterNumber = 1;
                var firstCluster = new ClopeCluster();
                firstCluster.AddTransaction(firstTransaction); //Транзакция лежит в коллеции своего кластера
                clusterDict.Add(1, firstCluster); // Первый кластер лежит в словаре под номером 1
                transactionsTable.Add(firstTransaction); // Транзакции из файла хранятся в коллекции
            }

            // Теперь пошли циклом по остальным транзакциям
            while (_dsProvider.MoveNext())
            {
                Transaction currentTransaction = (Transaction)_dsProvider.Current;
                //Создаем новый пустой кластер-приемник. определим, каким он будет
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

            return new CalculationResult
            {
                ClustersTable = clusterDict,
                TransactionsTable = transactionsTable
            };
        }

        public CalculationResult Iterate()
        {
            int countIteration = 0;

            bool moved = true;
            while (moved) // цикл по итерациям
            {
                countIteration++;
                moved = false;

                foreach (Transaction tr in transactionsTable)
                {
                    // Цена добавления транзакции в новый кластер. Отправная точка сравнения
                    double maxDelta = DeltaAdd(null, tr, repulsCoeff);

                    // Запоминаем номер старого кластера, в котором лежала транзакция
                    var oldClusterNumber = tr.ClusterNumber;
                    var newClusterNumber = -1;

                    // Узнаем цену удаления транзакции из ее текущего кластера
                    double removeDelta = DeltaRemove(clusterDict[oldClusterNumber], tr, repulsCoeff);

                    //Теперь перебираем все кластеры, кроме того, где лежала наша транзакция
                    //и узнаем стоимость запиха транзакции туда
                    var arrayWithoutCurrentCluster = clusterDict.Where(kvp => kvp.Key != tr.ClusterNumber).ToArray();
                    foreach (KeyValuePair<int, ClopeCluster> pair in arrayWithoutCurrentCluster)
                    {
                        double addDelta = DeltaAdd(pair.Value, tr, repulsCoeff);

                        // Если стоимость от добавления транзакции в существующий кластер больше, чем в новый
                        // предварительно решаем положить транзакцию в этот существующий кластер
                        if (addDelta > maxDelta)
                        {
                            maxDelta = addDelta;

                            // Запомнили новый кластер у транзакции
                            newClusterNumber = pair.Key;
                        }
                    }

                    // Окончательное решение, куда выгоднее положить транзакцию
                    if (maxDelta + removeDelta > 0)
                    {
                        //Если не нашлось подходящих существующих кластеров
                        if (newClusterNumber == -1)
                        {
                            var clast = new ClopeCluster();
                            clusterDict[oldClusterNumber].RemoveTransaction(tr);
                            newClusterNumber = clusterDict.Count + 1;
                            tr.ClusterNumber = newClusterNumber;
                            clusterDict.Add(newClusterNumber, clast);
                            clusterDict[newClusterNumber].AddTransaction(tr);
                        }
                        // Если имеющийся кластер нашелся
                        else
                        {
                            clusterDict[oldClusterNumber].RemoveTransaction(tr);
                            clusterDict[newClusterNumber].AddTransaction(tr);
                            tr.ClusterNumber = newClusterNumber;
                        }

                        // В любом случае перестановка имела место, вторая итерация будет
                        moved = true;
                    }
                }
            }
            var clusterDictNew = clusterDict.Where(kvp => kvp.Value.TransCount != 0).ToDictionary(k => k.Key, k => k.Value);

            return new CalculationResult
            {
                ClustersTable = clusterDictNew,
                TransactionsTable = transactionsTable,
                IterationCount = countIteration
            };
        }

        private double DeltaAdd(ClopeCluster cluster, Transaction transaction, double r)
        {
            if (cluster == null)
                return transaction.ArrayValues.Count / Math.Pow(transaction.UniqueParameters, r);

            double squareNew = cluster.Square + transaction.ArrayValues.Count;
            double widthNew = cluster.Width;
            for (int i = 0; i < transaction.ArrayValues.Count; i++)
                if (!cluster.Occ.ContainsKey(transaction.ArrayValues[i]))
                    widthNew++;
            var res = squareNew * (cluster.TransCount + 1) / Math.Pow(widthNew, r) - cluster.Square * cluster.TransCount / Math.Pow(cluster.Width, r);
            return res;
        }

        private double DeltaRemove(ClopeCluster cluster, Transaction transaction, double r)
        {
            // Удаление посленей транзакции из кластера равносильно
            // добавлению транзакции в новый (пустой) кластер только с другим знаком
            if (cluster.TransCount == 1)
                return -cluster.Square / Math.Pow(cluster.Width, r);

            double squareNew = cluster.Square - transaction.ArrayValues.Count;
            double widthNew = cluster.Width;
            for (int i = 0; i < transaction.ArrayValues.Count; i++)
                if (cluster.Occ[transaction.ArrayValues[i]] == 1)
                    widthNew--;
            return squareNew * (cluster.TransCount - 1) / Math.Pow(widthNew, r) - cluster.Square * cluster.TransCount / Math.Pow(cluster.Width, r);
        }
    }
}
