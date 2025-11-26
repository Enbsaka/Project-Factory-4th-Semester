using Dunder_Store.Entities;

namespace Dunder_Store.Interfaces.IServices
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(Guid id);
        Task<Cliente> CriarClienteAsync(Cliente cliente);
        Task AtualizarClienteAsync(Cliente cliente);
        Task RemoverClienteAsync(Guid id);
        Task<Cliente?> AutenticarAsync(string email, string senha);
        Task<Cliente> CriarClienteComRegrasAsync(Cliente cliente, bool usuarioEhAdmin);
        Task RemoverClienteComRegrasAsync(Guid id);
        Task AtualizarClienteComRegrasAsync(Guid id, DTO.ClienteDTOInput dto);
        Task AtualizarMeuPerfilComRegrasAsync(Guid userId, DTO.ClienteDTOInput dto);
    }
}
