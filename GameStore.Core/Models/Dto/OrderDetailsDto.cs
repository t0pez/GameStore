namespace GameStore.Core.Models.Dto;

public class OrderDetailsDto
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    
    public string GameKey { get; set; }
    public ProductDto Game { get; set; }
    
    public OrderDto Order { get; set; }

    public decimal TotalPrice { get; set; }
}