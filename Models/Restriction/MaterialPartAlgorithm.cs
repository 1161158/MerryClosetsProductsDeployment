using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using MerryClosets.Models.Product;
using MerryClosets.Services.EF;
using MerryClosets.Services.Interfaces;
using SQLitePCL;

namespace MerryClosets.Models.Restriction
{
    //Not used
    public class MaterialPartAlgorithm : PartAlgorithm
    {
        //List of parts material
        private static readonly string MaterialPart = "MaterialPart";
        //Material of the configured product
        private static readonly string Material = "Material";

        public MaterialPartAlgorithm(){}

        public override bool validate(Dictionary<string, object> parameters)
        {
           var configuredMaterial = (Material.Material)parameters[Material];
           var listMaterialPart = (List<Material.Material>) parameters[MaterialPart];
            
           return listMaterialPart.Contains(configuredMaterial);
        }

        public override Dictionary<string, string> parameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("No parameters", "This algorithm requires no parameters");
            return parameters;
        }
    }
}