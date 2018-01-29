using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    public class Transaction
    {
        public Transaction(List<int> values)
        {
            ArrayValues = values;
        }

        //Массив всех значений текущей транзакции
        public List<int> ArrayValues { get; }

        //Количество уникальных элементов в транзакции, она же ширина кластера
        //Параметр необходим при добавлении транзакции в пустой кластер
        public int UniqueParameters => ArrayValues.Distinct().Count();

        public int ClusterNumber { get; set; }

        public bool IsPoison;
    }
}
