namespace Dunder_Store.Interfaces.IServices
{
    public interface IEmailService
    {
        System.Threading.Tasks.Task SendWelcomeAsync(Dunder_Store.Entities.Cliente cliente);
        System.Threading.Tasks.Task SendOrderReceiptAsync(Dunder_Store.Entities.Pedido pedido);
    }
}