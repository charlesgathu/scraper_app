using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Scraper.Domain;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Infrastructure
{
    public class ScraperContext : DbContext, IUnitOfWork
    {

        private IDbContextTransaction _currentTransaction;

        public ScraperContext(DbContextOptions<ScraperContext> options) : base(options)
        { }

        public DbSet<Show> Shows { get; set; }

        public DbSet<Cast> Cast { get; set; }

        public DbSet<ShowCast> ShowCast { get; set; }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Show>(entity => {

                entity.HasKey(k => k.Id);
                entity.Property(p => p.Id).ValueGeneratedNever();
                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.Name).HasMaxLength(255);

                entity.ToTable("Shows");

            });

            modelBuilder.Entity<Cast>(entity =>
            {

                entity.HasKey(k => k.Id);
                entity.Property(p => p.Id).ValueGeneratedNever();
                entity.Property(p => p.Name).IsRequired();
                entity.Property(p => p.Name).HasMaxLength(175);
                entity.Property(p => p.Birthday).HasColumnType("DATE");

                entity.ToTable("Cast");

            });

            modelBuilder.Entity<ShowCast>(entity =>
            {

                entity.HasKey(k => new { k.ShowId, k.CastId });

                entity.HasOne(p => p.Show).WithMany(r => r.ShowCast).HasForeignKey(k => k.ShowId);
                entity.HasOne(p => p.Cast).WithMany(r => r.ShowCast).HasForeignKey(k => k.CastId);

                entity.ToTable("ShowCast");

            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
