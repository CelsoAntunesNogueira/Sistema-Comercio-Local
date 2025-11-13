using SistemaPDV.Data.Context;
using SistemaPDV.Data.Repositories;
using SistemaPDV.Models.Entities;

namespace SistemaPDV.Business.Services
{
    public class VendaService
    {
        private readonly AppDbContext _context;
        private readonly ProdutoRepository _produtoRepository;

        public VendaService(AppDbContext context)
        {
            _context = context;
            _produtoRepository = new ProdutoRepository(context);
        }

        public async Task<Venda> RealizarVendaAsync(
            int usuarioId,
            int? clienteId,
            List<ItemVenda> itens)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Criar venda
                var venda = new Venda
                {
                    UsuarioId = usuarioId,
                    ClienteId = clienteId,
                    DataVenda = DateTime.Now,
                    ValorTotal = itens.Sum(i => i.Subtotal)
                };

                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();

                // Adicionar itens e atualizar estoque
                foreach (var item in itens)
                {
                    item.VendaId = venda.Id;
                    _context.ItensVenda.Add(item);

                    await _produtoRepository.AtualizarEstoqueAsync(
                        item.ProdutoId,
                        item.Quantidade
                    );
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return venda;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}