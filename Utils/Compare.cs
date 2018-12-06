using System.Collections.Generic;
using System.Linq;

public class Compare
{

    public static bool ContentOfTwoLists<T>(List<T> list1, List<T> list2)
    {
        var firstNotSecond = list1.Except(list2).ToList();
        var secondNotFirst = list2.Except(list1).ToList();
        return !firstNotSecond.Any() && !secondNotFirst.Any();
    }
}