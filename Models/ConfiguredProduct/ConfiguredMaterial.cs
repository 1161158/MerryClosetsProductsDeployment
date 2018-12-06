namespace MerryClosets.Models.ConfiguredProduct
{
    public class ConfiguredMaterial : ValueObject
    {
        public string OriginMaterialReference { get; set; }
        public string ColorReference { get; set; }
        public string FinishReference { get; set; }

        public ConfiguredMaterial(string originMaterialReference, string colorReference,
            string finishReference)
        {
            this.OriginMaterialReference = originMaterialReference;
            this.ColorReference = colorReference;
            this.FinishReference = finishReference;
        }

        protected ConfiguredMaterial()
        {
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var cm = (ConfiguredMaterial) obj;
            return this.ColorReference.Equals(cm.ColorReference)
                   && this.FinishReference.Equals(cm.FinishReference)
                   && this.OriginMaterialReference.Equals(cm.OriginMaterialReference);
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(ColorReference, FinishReference, OriginMaterialReference).GetHashCode();
        }
    }
}