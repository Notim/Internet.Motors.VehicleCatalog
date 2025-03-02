namespace Application.Services.CreateNewOrderService;

public interface ICreateNewOrderService
{

    Task CreateNewOrderAsync(CreateNewOrder createNewOrder, CancellationToken cancellationToken);

}