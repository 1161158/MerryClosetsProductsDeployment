using System.Collections.Generic;

namespace MerryClosets.Models.DTO
{
    public class ComponentDto : ValueObjectDto
    {
        public string Name { get; set; }
        public AnimationDto Animation {get; set;}

        public ComponentDto(string name, AnimationDto animation)
        {
            this.Name = name;
            this.Animation = animation;
        }

        public ComponentDto(string name){
            this.Name = name;
        }

        protected ComponentDto(){}
    }
}