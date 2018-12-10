using System;
using System.Collections.Generic;
using MerryClosets.Models.ConfiguredProduct;

namespace MerryClosets.Models.Material
{
    public class Material : BaseEntityWithPriceHistory
    {
        /**
         * Name of the material.
         */
        public string Name { get; set; }

        /**
         * Description of the material.
         */
        public string Description { get; set; }

        /**
         * List of colors that the material has available.
         */
        public List<Color> Colors { get; set; } = new List<Color>();

        /**
         * List of finishes that the material has available.
         */
        public List<Finish> Finishes { get; set; } = new List<Finish>();

        public Material(string reference, string name, string description, Price price)
        {
            this.Reference = reference;
            this.Description = description;
            this.Name = name;
            this.Price = price;
        }

        public Material(string reference, string name, string description, Price price, List<Color> colors, List<Finish> finishes)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.Colors = colors;
            this.Finishes = finishes;
            this.Price = price;
        }

        protected Material()
        {
        }

        public void AddColor(Color color)
        {
            Colors.Add(color);
        }

        public void AddFinish(Finish finish)
        {
            Finishes.Add(finish);
        }
        
        public void RemoveColor(Color color)
        {
            Colors.Remove(color);
        }

        public void RemoveFinish(Finish finish)
        {
            Finishes.Remove(finish);
        }

        public void RemovePriceHistory(PriceHistory priceHistory)
        {
            PriceHistory.Remove(priceHistory);
        }

        public bool ContainsColor(string colorCode)
        {
            foreach (var color in Colors)
            {
                if (color.HexCode.Equals(colorCode))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsColor(Color color)
        {
            return Colors.Contains(color);
        }
        
        public bool ContainsFinish(Finish finish)
        {
            return Finishes.Contains(finish);
        }

        public bool ContainsFinish(string finishReference)
        {
            foreach (var finish in Finishes)
            {
                if (finish.Reference.Equals(finishReference))
                {
                    return true;
                }
            }

            return false;
        }

        public Finish GetFinish(string finishReference)
        {
            foreach (var finish in Finishes)
            {
                if (finish.Reference.Equals(finishReference))
                {
                    return finish;
                }
            }

            return null;
        }

        public bool IsConfigurationValid(ConfiguredMaterial configuredMaterial)
        {
            if (!Reference.Equals(configuredMaterial.OriginMaterialReference))
            {
                return false;
            }

            foreach (var color in Colors)
            {
                if (color.HexCode.Equals(configuredMaterial.ColorReference))
                {
                    foreach (var finish in Finishes)
                    {
                        if (finish.Reference.Equals(configuredMaterial.FinishReference))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            return false;
        }

        public bool ChosenColorIsValid(string referene)
        {
            foreach (var color in Colors)
            {
                if (color.ChosenColorIsValid(referene))
                {
                    return true;
                }
            }

            return false;
        }

        public Price ChosenFinishIsValid(string reference)
        {
            foreach (var finish in Finishes)
            {
                if (finish.ChosenFinishIsValid(reference))
                {
                    return finish.Price;
                }
            }

            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            Material m = obj as Material;
            return string.Equals(this.Reference, m.Reference, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(Reference).GetHashCode();
        }
        
    }
}
