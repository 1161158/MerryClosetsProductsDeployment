using System.Collections.Generic;

namespace MerryClosets.Models.Animation
{
    public class Component : ValueObject
    {
        public string Name { get; set; }
        public Animation Animation {get; set;}

        public Component(string name, Animation animation)
        {
            this.Name = name;
            this.Animation = animation;
        }

        public Component(string name){
            this.Name = name;
        }

        protected Component(){}
    }
}