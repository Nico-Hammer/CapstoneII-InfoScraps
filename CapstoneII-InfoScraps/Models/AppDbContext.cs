using CapstoneII_InfoScraps.Models.DB;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Users> Users { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<ScrapedData> ScrapedData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Users>()
            .HasOne(u => u.Account)
            .WithMany(a => a.Users)
            .HasForeignKey(u => u.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmailTemplate>()
            .HasOne(e => e.Account)
            .WithMany(a => a.Email_Templates)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ScrapedData>()
            .HasOne(s => s.Account)
            .WithMany(a => a.Scraped_Data)
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}