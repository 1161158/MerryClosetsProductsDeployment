using System.Text;

namespace MerryClosets.Utils
{
    public class ValidationOutputForbidden : ValidationOutput
    {
        public ValidationOutputForbidden() : base()
        {
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (var st in FoundErrors)
            {
                s.Append(st.Key);
                s.Append(" => ");
                s.Append(st.Value);
                s.Append("\n");
            }
            return s.ToString();
        }
    }
}