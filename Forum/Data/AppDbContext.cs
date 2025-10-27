using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Forum.Models;

namespace Forum.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<Answer> Answers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if(tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Replace("AspNet", ""));
                }
            }
        }
    }
}
