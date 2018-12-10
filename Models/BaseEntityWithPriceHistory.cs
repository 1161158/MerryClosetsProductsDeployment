using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MerryClosets.Models
{
    public abstract class BaseEntityWithPriceHistory : BaseEntity
    {
        /**
        * Price associated with the entity who needs to keep track of its price history.
        */
        [NotMapped]
        public Price Price { get; set; }

        /**
         * History that holds all past and future values the price has been set to, since the entity (of a partiular material) has been created.
         */
        public List<PriceHistory> PriceHistory { get; set; } = new List<PriceHistory>();

        /**
         * Method that will return the price associated with the most recent date (as of the time this method gets executed).
         * Even though it can return null, it shouldn't because an entity with a price history always has, at least, one price during it's lifetime.
         * This is also the reason the validations are not performed in the respective service class.
         */
        public Price CurrentPrice()
        {
            List<PriceHistory> sortedPriceHistory = PriceHistory.OrderBy(o => o.Date).ToList();

            if (sortedPriceHistory.Count == 0)
            {
                return null;
            }

            if (sortedPriceHistory.Count == 1)
            {
                return sortedPriceHistory[0].Price;
            }

            int i = 0;
            for (i = 0; i < sortedPriceHistory.Count - 1; i++)
            {
                if (DateTime.Compare(sortedPriceHistory[i].Date, DateTime.Now) < 0 && DateTime.Compare(sortedPriceHistory[i + 1].Date, DateTime.Now) > 0)
                {
                    return sortedPriceHistory[i].Price;
                }
            }
            return sortedPriceHistory[i].Price;
        }

        /**
         * Method that allows the deletion of all records in the price history of the entity being considered.
         */
        public void ClearPriceHistory()
        {
            this.PriceHistory.Clear();
        }

        public void AddPriceToHistory(Price price)
        {
            PriceHistory.Add(new PriceHistory(price));
        }

        public void AddPriceToHistory(PriceHistory priceHistory)
        {
            PriceHistory.Add(priceHistory);
        }

        public bool ContainsPriceHistory(PriceHistory priceHistory)
        {
            return PriceHistory.Contains(priceHistory);
        }
    }
}
