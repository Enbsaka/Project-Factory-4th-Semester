using Dunder_Store.DTO;
using Dunder_Store.Entities;
using Dunder_Store.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        private string? ToAbsoluteImageUrl(string? storedUrl)
        {
            if (string.IsNullOrWhiteSpace(storedUrl)) return storedUrl;

            if (Uri.TryCreate(storedUrl, UriKind.Absolute, out var uri))
            {
                var path = uri.AbsolutePath;
                var file = System.IO.Path.GetFileName(path);
                if (!string.IsNullOrEmpty(file) && path.Contains("/produtos"))
                {
                    return $"{Request.Scheme}://{Request.Host}/produtos/{file}";
                }
                return storedUrl;
            }

            var rel = storedUrl.TrimStart('/');
            if (rel.StartsWith("produtos"))
            {
                var file = System.IO.Path.GetFileName(rel);
                return $"{Request.Scheme}://{Request.Host}/produtos/{file}";
            }

            var onlyFile = System.IO.Path.GetFileName(rel);
            if (!string.IsNullOrEmpty(onlyFile))
            {
                return $"{Request.Scheme}://{Request.Host}/produtos/{onlyFile}";
            }

            return storedUrl;
        }

        private Guid? GetLoggedClienteId()
        {
            var id = User?.FindFirst("Id")?.Value ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(id, out var guid)) return guid;
            return null;
        }

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Paginador<Pedido>>> GetPedidos(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            if (pagina <= 0) pagina = 1;
            if (tamanhoPagina <= 0) tamanhoPagina = 10;

            var resultado = await _pedidoService.GetPedidosAsync(pagina, tamanhoPagina);
            if (resultado.Itens == null || !resultado.Itens.Any()) return NoContent();
            return Ok(resultado);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetPedido(Guid id)
        {
            var pedido = await _pedidoService.GetDetalhadoAsync(id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            var retorno = new
            {
                pedido.Id,
                pedido.ClienteId,
                ClienteNome = pedido.Cliente.Nome,
                pedido.Status,
                DataPedido = pedido.DataPedido,
                Produtos = pedido.PedidoProdutos.Select(pp => new
                {
                    pp.ProdutoId,
                    pp.Produto.Nome,
                    pp.Produto.Preco,
                    pp.Produto.CodigoDeBarra,
                    ImagemURL = ToAbsoluteImageUrl(pp.Produto.ImagemURL),
                    pp.Quantidade,
                    ValorTotalProduto = pp.ValorTotal
                }),
                ValorTotalSemDesconto = pedido.ValorTotalSemDesconto,
                Frete = pedido.FreteValor ?? 0m,
                Cupom = pedido.Cupom != null ? new
                {
                    pedido.Cupom.Codigo,
                    pedido.Cupom.DescontoPercentual,
                    pedido.Cupom.DataExpiracao
                } : null,
                ValorTotalComDesconto = pedido.ValorTotal
            };

            return Ok(retorno);
        }


        [HttpGet("cliente/{clienteId:guid}")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<ActionResult<Paginador<Pedido>>> GetPedidosPorCliente(
            Guid clienteId,
            [FromQuery] PedidoStatus? status = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            if (User.IsInRole("Cliente"))
            {
                var userId = GetLoggedClienteId();
                if (userId == null || userId.Value != clienteId)
                    return Forbid();
            }
            var resultado = await _pedidoService.GetPedidosPorClienteAsync(clienteId, status, pagina, tamanhoPagina);
            if (resultado.Itens == null || !resultado.Itens.Any()) return NoContent();
            return Ok(resultado);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<ActionResult<Pedido>> CreatePedido(PedidoDTO novoPedidoDTO)
        {
            try
            {
                var itens = novoPedidoDTO.produtos.Select(p => (p.CodigoDeBarra, p.Quantidade)).ToList();
                var pedido = await _pedidoService.CriarPedidoAsync(novoPedidoDTO.clientecpf, itens, novoPedidoDTO.cupomCodigo, novoPedidoDTO.freteValor);
                if (User.IsInRole("Cliente"))
                {
                    var userId = GetLoggedClienteId();
                    if (userId == null || userId.Value != pedido.ClienteId)
                        return Forbid();
                }
                return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("carrinho/{clienteId:guid}")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<ActionResult<object>> GetCarrinhoPorCliente(Guid clienteId)
        {
            if (User.IsInRole("Cliente"))
            {
                var userId = GetLoggedClienteId();
                if (userId == null || userId.Value != clienteId)
                    return Forbid();
            }
            var carrinho = await _pedidoService.GetOrCreateCarrinhoAsync(clienteId);

            var retorno = new
            {
                carrinho.Id,
                carrinho.ClienteId,
                ClienteNome = carrinho.Cliente.Nome,
                carrinho.Status,
                Produtos = carrinho.PedidoProdutos.Select(pp => new
                {
                    pp.ProdutoId,
                    pp.Produto.Nome,
                    pp.Produto.Preco,
                    pp.Produto.CodigoDeBarra,
                    ImagemURL = ToAbsoluteImageUrl(pp.Produto.ImagemURL),
                    pp.Quantidade,
                    ValorTotalProduto = pp.ValorTotal
                }),
                ValorTotalSemDesconto = carrinho.ValorTotalSemDesconto,
                Frete = carrinho.FreteValor ?? 0m,
                Cupom = carrinho.Cupom != null ? new
                {
                    carrinho.Cupom.Codigo,
                    carrinho.Cupom.DescontoPercentual,
                    carrinho.Cupom.DataExpiracao
                } : null,
                ValorTotalComDesconto = carrinho.ValorTotal
            };

            return Ok(retorno);
        }

        [HttpGet("receita-mensal")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<decimal>> GetReceitaMensal()
        {
            var receita = await _pedidoService.GetReceitaMensalAsync(DateTime.Now);
            return Ok(receita);
        }

        [HttpGet("produtos-mais-vendidos")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetProdutosMaisVendidos()
        {
            var produtosMaisVendidos = await _pedidoService.GetProdutosMaisVendidosAsync(5);
            return Ok(produtosMaisVendidos);
        }



        [HttpPost("finalizar/{id:guid}")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> FinalizarPedido(Guid id)
        {
            var pedido = await _pedidoService.GetDetalhadoAsync(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (User.IsInRole("Cliente"))
            {
                var userId = GetLoggedClienteId();
                if (userId == null || pedido.ClienteId != userId.Value)
                    return Forbid();
            }
            try
            {
                await _pedidoService.FinalizarPedidoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> UpdatePedido(Guid id, PedidoDTO pedidoAtualizadoDTO)
        {
            var pedido = await _pedidoService.GetDetalhadoAsync(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (User.IsInRole("Cliente"))
            {
                var userId = GetLoggedClienteId();
                if (userId == null || pedido.ClienteId != userId.Value)
                    return Forbid();
            }
            try
            {
                if (pedidoAtualizadoDTO.produtos == null || pedidoAtualizadoDTO.produtos.Count == 0)
                    return BadRequest("É necessário enviar a lista de produtos.");

                var itens = pedidoAtualizadoDTO.produtos.Select(p => (p.CodigoDeBarra, p.Quantidade)).ToList();
                await _pedidoService.AtualizarItensAsync(id, itens);
                if (pedidoAtualizadoDTO.freteValor.HasValue)
                    await _pedidoService.AtualizarFreteAsync(id, pedidoAtualizadoDTO.freteValor);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id:guid}/cupom")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> AtualizarCupom(
            Guid id,
            [FromForm] string? cupomCodigo,
            [FromBody] Dunder_Store.DTO.CupomAplicarDTO? model)
        {
            var pedido = await _pedidoService.GetDetalhadoAsync(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (User.IsInRole("Cliente"))
            {
                var userId = GetLoggedClienteId();
                if (userId == null || pedido.ClienteId != userId.Value)
                    return Forbid();
            }
            var codigo = string.IsNullOrWhiteSpace(cupomCodigo) ? model?.cupomCodigo : cupomCodigo;
            try
            {
                await _pedidoService.AtualizarCupomAsync(id, codigo);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePedido(Guid id)
        {
            try
            {
                await _pedidoService.RemoverPedidoAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("cliente/{clienteId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePedidosCliente(Guid clienteId)
        {
            try
            {
                await _pedidoService.RemoverPedidosClienteAsync(clienteId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
