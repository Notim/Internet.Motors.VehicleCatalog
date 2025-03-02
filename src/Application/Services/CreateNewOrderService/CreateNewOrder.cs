namespace Application.Services.CreateNewOrderService;

public record CreateNewOrder
{
    public CreateNewOrder(
        Guid orderId,
        Guid vehicleId,
        string? customerDocument,
        string? carName,
        decimal price
    )
    {
        VehicleId = vehicleId;
        CustomerDocument = customerDocument;
        CarName = carName;
        Price = price;
        OrderId = orderId;
        OrderedAt = DateTime.Now;
    }

    public Guid VehicleId { get; private set; }

    public Guid OrderId { get; private set; }

    public string? CustomerDocument { get; private set; }

    public string? CarName { get; private set; }

    public decimal Price { get; private set; }

    public DateTime OrderedAt { get; private set; }

}