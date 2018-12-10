using System;
using System.Collections.Generic;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Material;

namespace MerryClosets.Models.Restriction
{
    /* restringir o material e acabamento de um complemento (produto) em função do material e acabamento do produto que está a ser complementado.
       E.g. Restringir que as prateleiras que complementam um dado armário são construídas no mesmo material e acabamento do respetivo armário. */
    public class MaterialFinishPartAlgorithm : PartAlgorithm
    {
        //Parent Configured Product
        private static readonly string ConfiguredParent = "ParentConfiguredProduct";

        //Child Configured Product
        private static readonly string ConfiguredChild = "ChildConfiguredProduct";

        public MaterialFinishPartAlgorithm()
        {
        }

        public override bool validate(Dictionary<string, object> parameters)
        {
            var configuredParent = (Models.ConfiguredProduct.ConfiguredProduct)parameters[ConfiguredParent];
            var configuredChild = (ConfiguredProductDto)parameters[ConfiguredChild];

            return HasSameMaterial(configuredParent, configuredChild) && HasSameFinish(configuredParent, configuredChild);
        }

        private static bool HasSameMaterial(Models.ConfiguredProduct.ConfiguredProduct configuredParent,
            ConfiguredProductDto configuredChild)
        {
            return string.Equals(configuredParent.ConfiguredMaterial.OriginMaterialReference, configuredChild.ConfiguredMaterial.OriginMaterialReference, StringComparison.Ordinal);
        }

        private static bool HasSameFinish(Models.ConfiguredProduct.ConfiguredProduct configuredParent,
            ConfiguredProductDto configuredChild)
        {
            return string.Equals(configuredParent.ConfiguredMaterial.FinishReference, configuredChild.ConfiguredMaterial.FinishReference, StringComparison.Ordinal);
        }

        public override Dictionary<string, string> parameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("No parameters", "This algorithm requires no parameters");
            return parameters;
        }

        public override bool Equals(object obj)
        {
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return true;
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}