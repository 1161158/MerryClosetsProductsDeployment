using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class ColorDto : ValueObjectDto
    {
        public string Name { get; set; }

        public string Description { get; set; }
        
        public string HexCode { get; set; }

        public ColorDto(string name, string description, string hexCode)
        {
            this.Name = name;
            this.Description = description;
            this.HexCode = SetHexCode(hexCode);
        }

        private string SetHexCode(string hex){
             if(hex.Length == 7 && hex.Contains("#")){
                return hex.Substring(1,6);
            }else{
               return hex;
            }
        }
        protected ColorDto() { }
    }
}