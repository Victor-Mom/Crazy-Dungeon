using System.Collections.Generic;
using System.Linq;

public static class Utils
{
    public static IEnumerable<int> Range(int start, int end)
    {
        int count = end - start;
        if (count < 0)
            return Enumerable.Range(count, -count + start).Reverse();
        return Enumerable.Range(start, count);
    }
}