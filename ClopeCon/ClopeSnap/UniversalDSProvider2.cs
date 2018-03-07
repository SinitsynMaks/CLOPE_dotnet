using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    class UniversalDSProvider2<T> : IEnumerator<T> where T : class, new ()
    {
        private StreamReader sr;
        private Dictionary<Tuple<int, string>, int> indexValueDict = new Dictionary<Tuple<int, string>, int>();

        object IEnumerator.Current => Current;

        public T Current => GetCurrentTranzaction;

        private T GetCurrentTranzaction()
        {
            string str = sr.ReadLine();
            string[] values = str.Substring(2).Split(',');
            var intValuesList = new List<int>();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != "?")
                {
                    var item = new Tuple<int, string>(i, values[i]);
                    if (indexValueDict.ContainsKey(item))
                    {
                        intValuesList.Add(indexValueDict[item]);
                    }
                    else
                    {
                        var val = indexValueDict.Count;
                        intValuesList.Add(val);
                        indexValueDict[item] = val;
                    }
                }

                var transaction = new T(intValuesList);
                switch (str[0])
                {
                    case 'p':
                        //transaction.IsPoison = true;
                        break;
                    case 'e':
                       // transaction.IsPoison = false;
                        break;
                }

                return transaction;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
