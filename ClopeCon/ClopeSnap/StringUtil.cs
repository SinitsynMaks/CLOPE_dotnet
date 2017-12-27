using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    public static class StringUtil
    {
        public static Transaction GetTransaction(this string str)
        {
            string line = str.Substring(2, str.Length - 2);
            string[] stringValues = line.Split(',');
            
            int[] numericVal = stringValues.Select(v => Convert.ToInt32(v)).ToArray();

            Transaction transaction = new Transaction(numericVal);
            switch (str[0])
            {
                case 'p':
                    transaction.IsPoison = true;
                    break;
                case 'e':
                    transaction.IsPoison = false;
                    break;
            }
            return transaction;
        }
    }
}