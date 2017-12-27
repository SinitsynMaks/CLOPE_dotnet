using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClopeCon.ClopeSnap
{
    public static class MushroomDataSet
    {
        private static readonly Dictionary<int, Dictionary<char, int>> NormalizeDict = new Dictionary<int, Dictionary<char, int>>
            {
                {1, new Dictionary<char, int> {
                    {'b', 1},
                    {'c', 2},
                    {'x', 3},
                    {'f', 4},
                    {'k', 5},
                    {'s', 6}
                } },
                {2, new Dictionary<char, int> {
                    {'f', 7},
                    {'g', 8},
                    {'y', 9},
                    {'s', 10}
                } },
                {3, new Dictionary<char, int> {
                    {'n', 11},
                    {'b', 12},
                    {'c', 13},
                    {'g', 14},
                    {'r', 15},
                    {'p', 16},
                    {'u', 17},
                    {'e', 18},
                    {'w', 19},
                    {'y', 20}
                } },
                {4, new Dictionary<char, int> {
                    {'t', 21},
                    {'f', 22}
                } },
                {5, new Dictionary<char, int> {
                    {'a', 23},
                    {'l', 24},
                    {'c', 25},
                    {'y', 26},
                    {'f', 27},
                    {'m', 28},
                    {'n', 29},
                    {'p', 30},
                    {'s', 31}
                } },
                {6, new Dictionary<char, int> {
                    {'a', 32},
                    {'d', 33},
                    {'f', 34},
                    {'n', 35}
                } },
                {7, new Dictionary<char, int> {
                    {'c', 36},
                    {'w', 37},
                    {'d', 38}
                } },
                {8, new Dictionary<char, int> {
                    {'b', 39},
                    {'n', 40}
                } },
                {9, new Dictionary<char, int> {
                    {'k', 41},
                    {'n', 42},
                    {'b', 43},
                    {'h', 44},
                    {'g', 45},
                    {'r', 46},
                    {'o', 47},
                    {'p', 48},
                    {'u', 49},
                    {'e', 50},
                    {'w', 51},
                    {'y', 52},
                } },
                {10, new Dictionary<char, int> {
                    {'e', 53},
                    {'t', 54}
                } },
                {11, new Dictionary<char, int> {
                    {'b', 55},
                    {'c', 56},
                    {'u', 57},
                    {'e', 58},
                    {'z', 59},
                    {'r', 60}
                } },
                {12, new Dictionary<char, int> {
                    {'f', 61},
                    {'y', 62},
                    {'k', 63},
                    {'s', 64}
                } },
                {13, new Dictionary<char, int> {
                    {'f', 65},
                    {'y', 66},
                    {'k', 67},
                    {'s', 68}
                } },
                {14, new Dictionary<char, int> {
                    {'n', 69},
                    {'b', 70},
                    {'c', 71},
                    {'g', 72},
                    {'o', 73},
                    {'p', 74},
                    {'e', 75},
                    {'w', 76},
                    {'y', 77}
                } },
                {15, new Dictionary<char, int> {
                    {'n', 78},
                    {'b', 79},
                    {'c', 80},
                    {'g', 81},
                    {'o', 82},
                    {'p', 83},
                    {'e', 84},
                    {'w', 85},
                    {'y', 86}
                } },
                {16, new Dictionary<char, int> {
                    {'p', 87},
                    {'u', 88}
                } },
                {17, new Dictionary<char, int> {
                    {'n', 89},
                    {'o', 90},
                    {'w', 91},
                    {'y', 92}
                } },
                {18, new Dictionary<char, int> {
                    {'n', 93},
                    {'o', 94},
                    {'t', 95}
                } },
                {19, new Dictionary<char, int> {
                    {'c', 96},
                    {'e', 97},
                    {'f', 98},
                    {'l', 99},
                    {'n', 100},
                    {'p', 101},
                    {'s', 102},
                    {'z', 103}
                } },
                {20, new Dictionary<char, int> {
                    {'k', 104},
                    {'n', 105},
                    {'b', 106},
                    {'h', 107},
                    {'r', 108},
                    {'o', 109},
                    {'u', 110},
                    {'w', 111},
                    {'y', 112}
                } },
                {21, new Dictionary<char, int> {
                    {'a', 113},
                    {'c', 114},
                    {'n', 115},
                    {'s', 117},
                    {'v', 118},
                    {'y', 119}
                } },
                {22, new Dictionary<char, int> {
                    {'g', 120},
                    {'l', 121},
                    {'m', 122},
                    {'p', 123},
                    {'u', 124},
                    {'w', 125},
                    {'d', 126}
                } }
            };

        public static void Normalize(string inputFilePath, string outputFilePath)
        {
            StreamReader sr = new StreamReader(inputFilePath);
            StreamWriter sw = new StreamWriter(File.Create(outputFilePath));
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                string result = line[0] + ",";
                string line1 = line.Substring(2, line.Length - 2);
                string[] attributes = line1.Split(',');
                
                for (int i = 0; i < attributes.Length; i++)
                {
                    if (attributes[i] != "?")
                        result += NormalizeDict[i+1][attributes[i][0]] + ",";
                }
                sw.WriteLine(result.Substring(0, result.Length - 1));
            }
            sr.Close();
            sw.Close();
        }
    }
}
