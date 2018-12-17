using System.Collections.Generic;

namespace MerryClosets.Models.Animation
{
    public class ModelGroup : ValueObject
    {
        public string RelativeURL { get; set; }
        public List<Component> Components { get; set; } = new List<Component>();

        public ModelGroup(string RelativeURL, List<Component> components)
        {
            this.RelativeURL = RelativeURL;
            this.Components = components;
        }

        protected ModelGroup(){}
    }
}