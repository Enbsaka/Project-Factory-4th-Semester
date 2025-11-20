using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IRepositories;
using Dunder_Store.Interfaces.IServices;

namespace Dunder_Store.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly ICupomRepository _cupomRepository;
        private readonly IPedidoProdutoRepository _pedidoProdutoRepository;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IProdutoRepository produtoRepository,
            IClienteRepository clienteRepository,
            ICupomRepository cupomRepository,
            IPedidoProdutoRepository pedidoProdutoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _produtoRepository = produtoRepository;
            _clienteRepository = clienteRepository;
            _cupomRepository = cupomRepository;
            _pedidoProdutoRepository = pedidoProdutoRepository;
        }

        public Task<Paginador<Pedido>> GetPedidosAsync(int pagina = 1, int tamanhoPagina = 10)
            => _pedidoRepository.GetPagedAsync(pagina, tamanhoPagina);

        public Task<Pedido?> GetDetalhadoAsync(Guid id)
            => _pedidoRepository.GetDetalhadoByIdAsync(id);

        public Task<Paginador<Pedido>> GetPedidosPorClienteAsync(Guid clienteId, PedidoStatus? status = null, int pagina = 1, int tamanhoPagina = 10)
            => _pedidoRepository.GetByClientePagedAsync(clienteId, status, pagina, tamanhoPagina);

        public async Task<Pedido> GetOrCreateCarrinhoAsync(Guid clienteId)
        {
            var carrinho = await _pedidoRepository.GetCarrinhoByClienteAsync(clienteId);
            if (carrinho != null) return carrinho;

            var cliente = await _clienteRepository.GetByIdAsync(clienteId);
            if (cliente == null) throw new Exception("Cliente não encontrado.");

            carrinho = new Pedido(cliente, DateTime.Now)
            {
                Status = PedidoStatus.Carrinho
            };
            await _pedidoRepository.AddAsync(carrinho);
            return carrinho;
        }

        public async Task<Pedido> CriarPedidoAsync(string clienteCpf, List<(string CodigoDeBarra, int Quantidade)> itens, string? cupomCodigo, decimal? freteValor)
        {
            if (itens == null || itens.Count == 0) throw new Exception("É necessário enviar a lista de produtos.");

            var cliente = await _clienteRepository.GetByCpfAsync(clienteCpf);
            if (cliente == null) throw new Exception("Cliente inválido.");

            var novoPedido = new Pedido(cliente, DateTime.Now)
            {
                Status = PedidoStatus.Carrinho
            };

            // Map itens
            foreach (var item in itens)
            {
                var produto = await FindProdutoByCodigoAsync(item.CodigoDeBarra);
                if (produto == null) throw new Exception($"Produto com código de barras '{item.CodigoDeBarra}' não encontrado.");
                novoPedido.PedidoProdutos.Add(new PedidoProduto
                {
                    ProdutoId = produto.Id,
                    Produto = produto,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = 0m
                });
            }

            // Cupom opcional
            if (!string.IsNullOrWhiteSpace(cupomCodigo))
            {
                var cupom = await _cupomRepository.GetByCodigoAsync(cupomCodigo.Trim().ToUpper());
                if (cupom == null || !cupom.Ativo || cupom.DataExpiracao < DateTime.Now)
                    throw new Exception("Cupom inválido ou expirado.");
                novoPedido.CupomId = cupom.Id;
                novoPedido.Cupom = cupom;
            }

            // Frete opcional
            if (freteValor.HasValue)
            {
                novoPedido.FreteValor = freteValor.Value;
            }

            await _pedidoRepository.AddAsync(novoPedido);
            return novoPedido;
        }

        public async Task AtualizarItensAsync(Guid pedidoId, List<(string CodigoDeBarra, int Quantidade)> itens)
        {
            var pedido = await _pedidoRepository.GetDetalhadoByIdAsync(pedidoId);
            if (pedido == null) throw new Exception("Pedido não encontrado.");
            if (pedido.Status != PedidoStatus.Carrinho) throw new Exception("Não é possível alterar um pedido que já foi finalizado.");
            if (itens == null || itens.Count == 0) throw new Exception("É necessário enviar a lista de produtos.");

            // Limpa itens atuais
            pedido.PedidoProdutos.Clear();

            // Mapeia códigos enviados para produtos
            var codigos = itens.Select(i => i.CodigoDeBarra.Trim()).ToList();
            // Carrega todos produtos correspondentes
            var todosProdutos = new List<Produto>();
            foreach (var codigo in codigos.Distinct())
            {
                var prod = await FindProdutoByCodigoAsync(codigo);
                if (prod == null) throw new Exception($"Produto com código de barras '{codigo}' não encontrado.");
                todosProdutos.Add(prod);
            }

            foreach (var item in itens)
            {
                var produto = todosProdutos.First(p => string.Equals(p.CodigoDeBarra, item.CodigoDeBarra.Trim(), StringComparison.OrdinalIgnoreCase));
                if (item.Quantidade <= 0) throw new Exception("Quantidade deve ser maior que zero.");
                pedido.PedidoProdutos.Add(new PedidoProduto
                {
                    PedidoId = pedido.Id,
                    ProdutoId = produto.Id,
                    Produto = produto,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = 0m
                });
            }

            await _pedidoRepository.UpdateAsync(pedido);
        }

        public async Task AtualizarCupomAsync(Guid pedidoId, string? cupomCodigo)
        {
            var pedido = await _pedidoRepository.GetDetalhadoByIdAsync(pedidoId);
            if (pedido == null) throw new Exception("Pedido não encontrado.");
            if (pedido.Status != PedidoStatus.Carrinho) throw new Exception("Não é possível alterar um pedido já finalizado.");

            if (string.IsNullOrWhiteSpace(cupomCodigo))
            {
                pedido.CupomId = null;
                pedido.Cupom = null;
            }
            else
            {
                var cupom = await _cupomRepository.GetByCodigoAsync(cupomCodigo.Trim().ToUpper());
                if (cupom == null || !cupom.Ativo || cupom.DataExpiracao < DateTime.Now)
                    throw new Exception("Cupom inválido ou expirado.");
                pedido.CupomId = cupom.Id;
                pedido.Cupom = cupom;
            }

            await _pedidoRepository.UpdateAsync(pedido);
        }

        public async Task AtualizarFreteAsync(Guid pedidoId, decimal? freteValor)
        {
            var pedido = await _pedidoRepository.GetDetalhadoByIdAsync(pedidoId);
            if (pedido == null) throw new Exception("Pedido não encontrado.");
            if (pedido.Status != PedidoStatus.Carrinho) throw new Exception("Não é possível alterar um pedido já finalizado.");
            pedido.FreteValor = freteValor ?? pedido.FreteValor;
            await _pedidoRepository.UpdateAsync(pedido);
        }

        public async Task FinalizarPedidoAsync(Guid id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null) throw new Exception("Pedido não encontrado.");
            if (pedido.Status != PedidoStatus.Carrinho) throw new Exception("Pedido já está finalizado.");
            foreach (var item in pedido.PedidoProdutos)
            {
                item.PrecoUnitario = item.Produto.Preco;
            }
            pedido.Status = PedidoStatus.Finalizado;
            pedido.DataPedido = DateTime.Now;
            await _pedidoRepository.UpdateAsync(pedido);
        }

        public async Task RemoverPedidoAsync(Guid id)
        {
            var pedido = await _pedidoRepository.GetDetalhadoByIdAsync(id);
            if (pedido == null) return;

            // Política: preservar histórico — não remover pedidos finalizados
            if (pedido.Status == PedidoStatus.Finalizado)
                throw new Exception("Não é permitido remover pedidos finalizados.");

            await _pedidoRepository.DeleteAsync(pedido);
        }

        public async Task RemoverPedidosClienteAsync(Guid clienteId, bool incluirFinalizados = false)
        {
            var paginador = await _pedidoRepository.GetByClientePagedAsync(clienteId, null, 1, int.MaxValue);
            foreach (var pedido in paginador.Itens)
            {
                if (pedido.Status == PedidoStatus.Finalizado && !incluirFinalizados) continue;
                await _pedidoRepository.DeleteAsync(pedido);
            }
        }

        public Task<IEnumerable<object>> GetProdutosMaisVendidosAsync(int top = 5)
            => _pedidoProdutoRepository.GetTopMaisVendidosAsync(top);

        public async Task<decimal> GetReceitaMensalAsync(DateTime referencia)
        {
            var inicioMes = new DateTime(referencia.Year, referencia.Month, 1);
            var fimMes = inicioMes.AddMonths(1);
            var pedidos = await _pedidoRepository.GetFinalizadosInRangeAsync(inicioMes, fimMes);
            return pedidos.Sum(p => p.ValorTotal);
        }

        private async Task<Produto?> FindProdutoByCodigoAsync(string codigo)
        {
            // Não há método direto no repositório, busca por todos e filtra
            var pagina = await _produtoRepository.GetAllAsync(null, null, null, null, null, null, 1, int.MaxValue);
            return pagina.Itens.FirstOrDefault(p => string.Equals(p.CodigoDeBarra, codigo, StringComparison.OrdinalIgnoreCase));
        }
    }
}
