using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    public class Transaction
    {
        public Transaction(int[] parameters)
        {
            ArrayValues = parameters;
        }

        //Массив всех значений текущей транзакции
        public int[] ArrayValues { get; }

        //Количество уникальных элементов в транзакции, она же ширина кластера
        //Параметр необходим при добавлении транзакции в пустой кластер
        public int UniqueParameters => ArrayValues.Distinct().Count();

        public int ClusterNumber { get; set; }

        public bool IsPoison;
    }
}
