

using Newtonsoft.Json;

namespace MerryClosets.Models.DTO{
    public class ConfiguredSlotDto : Embedabble {
        public int Size { get; set; }
        public string Reference { get; set; }

        [JsonConstructor]
        public ConfiguredSlotDto(int size, string reference){
            this.Size = size;
            this.Reference = reference;
        }
    }
}