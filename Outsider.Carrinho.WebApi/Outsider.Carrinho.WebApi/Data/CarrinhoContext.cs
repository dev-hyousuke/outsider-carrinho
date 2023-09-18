using Microsoft.EntityFrameworkCore;

namespace Outsider.Carrinho.WebAPI.Data
{
    public sealed class CarrinhoContext : DbContext
    {
        public DbSet<CarrinhoItem> CarrinhoItens => Set<CarrinhoItem>();
        public DbSet<CarrinhoCliente> CarrinhoCliente => Set<CarrinhoCliente>();

        public CarrinhoContext(DbContextOptions<CarrinhoContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }
    }
}