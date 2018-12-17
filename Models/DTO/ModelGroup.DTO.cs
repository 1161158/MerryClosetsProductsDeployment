using System.Collections.Generic;

namespace MerryClosets.Models.DTO
{
    public class ModelGroupDto : ValueObjectDto
    {
        public string RelativeURL { get; set; }
        public List<ComponentDto> Components { get; set; } = new List<ComponentDto>();
        public ModelGroupDto(string RelativeURL, List<ComponentDto> components)
        {
            this.RelativeURL = RelativeURL;
            this.Components = components;
        }

        protected ModelGroupDto(){}
    }
}