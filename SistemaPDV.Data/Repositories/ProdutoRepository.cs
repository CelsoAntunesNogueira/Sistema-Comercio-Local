using Microsoft.EntityFrameworkCore;
using SistemaPDV.Data.Context;
using SistemaPDV.Models.Entities;

namespace SistemaPDV.Data.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>
    {
        public ProdutoRepository(AppDbContext context) : base(context) { }

        public async Task<Produto> GetByCodigoBarrasAsync(string codigoBarras)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras && p.Ativo);
        }

        public async Task<List<Produto>> GetProdutosAtivoAsync()
        {
            return await _dbSet
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<bool> AtualizarEstoqueAsync(int produtoId, int quantidade)
        {
            var produto = await GetByIdAsync(produtoId);
            if (produto == null) return false;

            produto.EstoqueAtual -= quantidade;
            await UpdateAsync(produto);
            return true;
        }
    }
}