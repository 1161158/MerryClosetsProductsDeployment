using System.Collections.Generic;

namespace MerryClosets.Models.Restriction
{
    public class MaterialAlgorithm : Algorithm{
        protected MaterialAlgorithm()
        {
        }

        public override Dictionary<string, string> parameters()
        {
            throw new System.NotImplementedException();
        }

        public override bool validate(Dictionary<string, object> parameters){ return true; }
    }
    // public abstract class MaterialAlgorithm : Algorithm
    // {
    //     public static readonly List<string> AvailableMaterialAlgorithms = new List<string>(new string[] { "MaterialAlgorithm" });
    // }
}