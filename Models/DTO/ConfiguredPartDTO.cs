using Newtonsoft.Json;

namespace MerryClosets.Models.DTO{

    public class ConfiguredPartDto : EmbedabbleDto{

        public string ConfiguredChildReference { get; set; }
        public string ChosenSlotReference { get; set; }

        [JsonConstructor]
        public ConfiguredPartDto(string configuredChildReference, string chosenSlotReference){
            this.ConfiguredChildReference = configuredChildReference;
            this.ChosenSlotReference = chosenSlotReference;
        }
        
        public override string ToString()
        {
            return this.ConfiguredChildReference + " " + this.ChosenSlotReference;
        }
    }
}