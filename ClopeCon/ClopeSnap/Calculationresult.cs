using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    /// <summary>
    /// Класса-аггрегатор результатов работы фаз алгоритма CLOPE.
    /// </summary>
    public class CalculationResult
    {
        public Dictionary<int, ClopeCluster> ClustersTable { get; set; }
        public List<Transaction> TransactionsTable { get; set; }
        public int IterationCount { get; set; }
    }
}
