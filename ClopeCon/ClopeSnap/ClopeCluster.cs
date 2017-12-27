using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    public class ClopeCluster
    {
        public double Square => _square;
        public double Width => _width;
        public int TransCount => _transCount; // Количество транзакций в кластере
        public Dictionary<int, int> Occ => _occ; // Количество вхождений каждого элемента транзакции в кластере
        public List<Transaction> TransactionList { get; } = new List<Transaction>();

        private double _square;
        private double _width;
        private int _transCount;
        private Dictionary<int, int> _occ = new Dictionary<int, int>();

        public void AddTransaction(Transaction transaction)
        {
            var length = transaction.ArrayValues.Length;
            _square += length;
            for (int i = 0; i < length; i++)
            {
                if (!_occ.ContainsKey(transaction.ArrayValues[i]))
                    _occ.Add(transaction.ArrayValues[i], 1);
                _occ[transaction.ArrayValues[i]] += 1;
            }
            _width = _occ.Count;
            _transCount++;
            TransactionList.Add(transaction);
        }

        public void RemoveTransaction(Transaction transaction)
        {
            var length = transaction.ArrayValues.Length;
            _square -= length;
            for (int i = 0; i < length; i++)
            {
                _occ[transaction.ArrayValues[i]] -= 1;
                if (_occ[transaction.ArrayValues[i]] == 0)
                    _occ.Remove(transaction.ArrayValues[i]);
            }
            _width = _occ.Count;
            _transCount--;
            TransactionList.Remove(transaction);
        }
    }
}
