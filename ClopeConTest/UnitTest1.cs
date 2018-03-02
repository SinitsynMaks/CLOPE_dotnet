using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClopeCon.ClopeSnap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClopeConTest
{
    [TestClass]
    public class UnitTest1
    {
        string path = "trans.txt";
        string preparedDataFilePath = "PreparedData.txt";

       // [TestInitialize]
        public void Init()
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Input file data not found");
            }

            StreamReader sr = new StreamReader(path);
            StreamWriter sw = new StreamWriter(preparedDataFilePath);
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine().Replace(",", "");
                char[] charSplit = line.ToCharArray();
                string transformString = "";
                for (int i = 0; i < charSplit.Length; i++)
                {
                    char currChar = charSplit[i];
                    if (currChar != '?')
                        transformString += Convert.ToInt32(currChar) + ",";
                }
                sw.WriteLine(transformString.Substring(0, transformString.Length - 1));
            }
            sr.Close();
            sw.Close();
        }

        [TestMethod]
        public void Initialise_TestSuccess()
        {
            //MushroomDataSet.Normalize(path, preparedDataFilePath);
            double repulsCoeff = 2.6;
            List<Transaction> transactTable;
           // ClopeCulc.Initialise(preparedDataFilePath, repulsCoeff, out transactTable);

           // var result = ClopeCulc.clusterDict;
           // var clustCount = result.Count;
           // var transCount = result.Select(kvp => kvp.Value.TransCount).Sum();

        }

        [TestMethod]
        public void Execute_TestSuccess()
        {

            double repulsCoeff = 2.6;

            //ClopeCulc.Execute(preparedDataFilePath, repulsCoeff);
            //var res = ClopeCulc.clusterDict;
        }
    }
}
