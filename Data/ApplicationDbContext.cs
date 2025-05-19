using HelpDeskSystem.AuditsManager;
using HelpDeskSystem.Controllers;
using HelpDeskSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<AuditTrail> AuditTrails { get; set; }

        public DbSet<TicketCategory> TicketCategories { get; set; }

        public DbSet<TicketSubCategory> TicketSubCategory { get; set; }

        public DbSet<SystemCode> SystemCodes { get; set; }

        public DbSet<SystemCodeDetail> SystemCodeDetails { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<TicketResolution> TicketResolutions { get; set; }

        public DbSet<SystemTask> SystemTasks { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }

        public DbSet<UserRoleProfile> UserRoleProfiles { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public virtual async Task<int> SaveChangesAsync(string userId = null)
        {
            OnBeforeSaveChanges(userId);
            var result = await base.SaveChangesAsync();
            return result;
        }

        private void OnBeforeSaveChanges(string userId)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditTrail || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.Module = entry.Entity.GetType().Name;
                auditEntry.UserId = userId;

                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties) // Untuk mengakses nilai untuk Column dan lain sebagai-nya
                {
                    string propertyName = property.Metadata.Name; // Nama Property

                    if (property.Metadata.IsPrimaryKey()) // Untuk mengidentifikasi apakah PrimaryKey
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue; // Untuk mengambil nilai PrimaryKey
                        continue; // Untuk melanjutkan ke Property selanjutnya
                    }

                    switch (entry.State) // Untuk mengidentifikasi State
                    {
                        case EntityState.Added: // Jika State adalah Added
                            auditEntry.AuditType = AuditType.Create; // Tipe Audit adalah Create
                            auditEntry.NewValues[propertyName] = property.CurrentValue; // Untuk mengambil nilai Property yang baru
                            break;

                        case EntityState.Deleted: // Jika State adalah Delete
                            auditEntry.AuditType = AuditType.Delete; // Tipe Audit adalah Delete
                            auditEntry.OldValues[propertyName] = property.OriginalValue; // Mengambil nilai Property yang lama
                            break;

                        case EntityState.Modified: // Jika State adalah Modified
                            if (property.IsModified) // Jika Property telah diubah
                            {
                                auditEntry.ChangedColumns.Add(propertyName); // Menambahkan Property yang telah diubah
                                auditEntry.AuditType = AuditType.Update; // Tipe Audit adalah Update
                                auditEntry.OldValues[propertyName] = property.OriginalValue; // Mengambil nilai Property yang lama
                                auditEntry.NewValues[propertyName] = property.CurrentValue;// Mengambil nilai Property yang baru
                            }
                            break;
                    }
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                AuditTrails.Add(auditEntry.ToAudit());
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var relationship in builder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            builder.Entity<Comment>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketResolution>()
                .HasOne(c => c.Status)
                .WithMany()
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketResolution>()
                .HasOne(c => c.Ticket)
                .WithMany()
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.Ticket)
                .WithMany(c => c.TicketComments) // Untuk relasi one to many
                .HasForeignKey(c => c.TicketId)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketCategory>()
                .HasOne(c => c.ModifiedBy)
                .WithMany()
                .HasForeignKey(c => c.ModifiedById)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TicketCategory>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .HasPrincipalKey(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SystemCodeDetail>()
                .HasOne(c => c.SystemCode)
                .WithMany()
                .HasForeignKey(c => c.SystemCodeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(c => c.Priority)
                .WithMany()
                .HasForeignKey(c => c.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasOne(c => c.Gender)
                .WithMany()
                .HasForeignKey(c => c.GenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasOne(c => c.Role)
                .WithMany()
                .HasForeignKey(c => c.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}