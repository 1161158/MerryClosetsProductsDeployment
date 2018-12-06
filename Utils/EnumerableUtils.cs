using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MerryClosets.Utils
{
    public class EnumerableUtils
    {
        public static string convert(IEnumerable<object> list)
        {
            StringBuilder builder = new StringBuilder();

            List<object> dtos = list.ToList();
            for (int i = 0; i < dtos.Count; i++)
            {
                builder.Append(dtos[i].ToString());
                if (i != dtos.Count - 1)
                {
                    builder.Append(", ");
                }
            }
            return builder.ToString();
        }
    }
}