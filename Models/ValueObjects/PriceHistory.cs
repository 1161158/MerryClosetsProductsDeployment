using System;
using System.ComponentModel.DataAnnotations.Schema;
using MerryClosets.Models;
using MerryClosets.Models.DTO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

public class PriceHistory : ValueObject
{
    public DateTime Date { get; set; }

    public Price Price { get; set; }

    public PriceHistory(Price price)
    {
        this.Price = price;
        this.Date = DateTime.Now;
    }

    public PriceHistory(Price price, DateTime date)
    {
        this.Price = price;
        this.Date = date;
    }

    protected PriceHistory()
    {
    }

    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }

        PriceHistory m = obj as PriceHistory;
        return Date.Day == m.Date.Day && Date.Month == m.Date.Month && Date.Year == m.Date.Year &&
               this.Price.Equals(m.Price);
    }

    public override int GetHashCode()
    {
        return this.Date.GetHashCode() + this.Price.GetHashCode();
    }
}