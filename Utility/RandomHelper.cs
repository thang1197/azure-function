using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Utility
{
    public class RandomHelper
    {
        public static int GetRandomNumber(int min, int max)
        {
            Random _random = new();
            return _random.Next(min, max);
        }
    }
}