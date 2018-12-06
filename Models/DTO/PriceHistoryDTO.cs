using System;
using MerryClosets.Models;
using MerryClosets.Models.DTO;
using Newtonsoft.Json;

public class PriceHistoryDto : ValueObjectDto
{
    public DateTime Date { get; set; }

    public PriceDto Price { get; set; }

    public PriceHistoryDto(PriceDto price)
    {
        this.Price = price;
        this.Date = DateTime.Now;
    }

    public PriceHistoryDto(PriceDto price, DateTime date)
    {
        this.Price = price;
        this.Date = date;
    }

    protected PriceHistoryDto() { }
    
    public override string ToString()
    {
        return this.Date.ToString();
    }
}
