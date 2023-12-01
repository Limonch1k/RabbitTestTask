using Microsoft.EntityFrameworkCore;

public class SqliteContext : DbContext
{
    private string dbpath {get;set;}
    public SqliteContext(string path)
    {
        dbpath = path;
    }
    public DbSet<ModuleCategory> ModuleCategories {get;set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=" + dbpath);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ModuleCategory>(entity =>
        {
            entity.HasKey(e => e.ModuleCategoryID).HasName("ModuleCategoryID");

            entity.ToTable("ModuleCategories");

            entity.Property(x => x.ModuleCategoryID).HasColumnName("ModuleCategoryID");

            entity.Property(x => x.ModuleState).HasColumnName("ModuleState");
        });
    }
}