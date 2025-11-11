using System;
using System.Diagnostics;

namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static T Measure<T>(string label, Func<T> func)
        {
            var sw = Stopwatch.StartNew();
            var result = func();
            sw.Stop();
            Console.WriteLine($"{label}: {sw.ElapsedMilliseconds} ms");
            return result;
        }
    }
}
