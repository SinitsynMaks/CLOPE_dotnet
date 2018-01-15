using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    public class Calculationresult
    {
        public Dictionary<int, ClopeCluster> ClustersTable { get; set; }
        public List<Transaction> TransactionsTable { get; set; }
        public int IterationCount { get; set; }
    }
}
