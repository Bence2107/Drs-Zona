using Entities.Models;
using Entities.Models.News;
using Entities.Models.Polls;
using Entities.Models.RaceTracks;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;

namespace Context;

public class EfContext(DbContextOptions<EfContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Article> Articles { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Poll> Polls { get; set; }

    public DbSet<PollOption> PollOptions { get; set; }

    public DbSet<Circuit> Circuits { get; set; }

    public DbSet<GrandPrix> GrandsPrix { get; set; }

    public DbSet<Brand> Brands { get; set; }

    public DbSet<Constructor> Constructors { get; set; }
    
    public DbSet<ConstructorCompetition> ConstructorCompetitions { get; set; }
    
    public DbSet<ConstructorsChampionship> ConstructorsChampionships { get; set; }
    
    public DbSet<Contract> Contracts { get; set; }

    public DbSet<Driver> Drivers { get; set; }

    public DbSet<DriverParticipation> DriverParticipates { get; set; }

    public DbSet<DriversChampionship> DriversChampionships { get; set; }

    public DbSet<Result> Results { get; set; }

    public DbSet<Series> Series { get; set; }
    
    public DbSet<Vote> Votes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //------------NEWS------------
        //Article relationships
        modelBuilder.Entity<Article>(options =>
            {
                // Article -> User (Many-to-One)
                options
                    .HasOne(a => a.Author)
                    .WithMany()
                    .HasForeignKey(a => a.AuthorId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Article -> GrandPrix (Many-to-One)
                options
                    .HasOne(a => a.GrandPrix)
                    .WithMany()
                    .HasForeignKey(a => a.GrandPrixId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired(false);
                
                options.Property(a => a.GrandPrixId).HasDefaultValue(null);
                options.Property(a => a.IsSummary).HasDefaultValue(false);
                options.Property(a => a.SecondSection).HasDefaultValue(null);
                options.Property(a => a.ThirdSection).HasDefaultValue(null);
                options.Property(a => a.FourthSection).HasDefaultValue(null);
                options.Property(a => a.DateUpdated).HasDefaultValue(null);

            }
        );

        //Comment relationships
        modelBuilder.Entity<Comment>(options =>
        {
            // Comment -> User (Many-to-One)
            options
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Comment -> Article (Many-to-One)
            options
                .HasOne(c => c.Article)
                .WithMany()
                .HasForeignKey(c => c.ArticleId)
                .OnDelete(DeleteBehavior.NoAction);

            // Comment -> Comment (Self-referencing for replies)
            options
                .HasOne(c => c.ReplyToComment)
                .WithMany()
                .HasForeignKey(c => c.ReplyToCommentId)
                .OnDelete(DeleteBehavior.NoAction);
            
            options.Property(c => c.ReplyToCommentId).HasDefaultValue(null);
            options.Property(c => c.UpVotes).HasDefaultValue(0);
            options.Property(c => c.DownVotes).HasDefaultValue(0);
            options.Property(c => c.DateUpdated).HasDefaultValue(null);
        });
        
        //------------POLL------------
        // Poll -> User (Many-to-One)
        modelBuilder.Entity<Poll>(options =>
            options
                .HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.NoAction)
        );

        // PollOption -> Poll (Many-to-One)
        modelBuilder.Entity<PollOption>(options =>
            options
                .HasOne(po => po.Poll)
                .WithMany()
                .HasForeignKey(po => po.PollId)
                .OnDelete(DeleteBehavior.Cascade)
        );

        // Vote composite key and relationships
        modelBuilder.Entity<Vote>(options =>
        {
            // Composite primary key
            options.HasKey(v => new { v.UserId, v.PollOptionId });

            options
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            options
                .HasOne(v => v.PollOption)
                .WithMany()
                .HasForeignKey(v => v.PollOptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        //------------RACETRACKS------------
        // GrandPrix -> Circuit (Many-to-One)
        modelBuilder.Entity<GrandPrix>(options =>
            options
                .HasOne(gp => gp.Circuit)
                .WithMany()
                .HasForeignKey(gp => gp.CircuitId)
                .OnDelete(DeleteBehavior.NoAction)
        );
        
        //------------STANDINGS------------
        // Contract relationships
        modelBuilder.Entity<Contract>(options =>
        {
            options
                .HasOne(c => c.Driver)
                .WithMany()
                .HasForeignKey(c => c.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            options
                .HasOne(c => c.Constructor)
                .WithMany()
                .HasForeignKey(c => c.ConstructorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DriverParticipation relationships
        modelBuilder.Entity<DriverParticipation>(options =>
        {
            options.HasKey(dp => new { dp.DriverId, dp.DriverChampId });
            options.Property(dp => dp.DriverNumber).HasDefaultValue(-1);
            
            options
                .HasOne(dp => dp.Driver)
                .WithMany()
                .HasForeignKey(dp => dp.DriverId)
                .OnDelete(DeleteBehavior.NoAction);

            options
                .HasOne(dp => dp.DriversChampionship)
                .WithMany()
                .HasForeignKey(dp => dp.DriverChampId)
                .OnDelete(DeleteBehavior.NoAction);
        });
        // Result relationships
        modelBuilder.Entity<Result>(options =>
        {
            options
                .HasOne(r => r.GrandPrix)
                .WithMany()
                .HasForeignKey(r => r.GrandPrixId)
                .OnDelete(DeleteBehavior.NoAction);

            options
                .HasOne(r => r.Driver)
                .WithMany()
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.NoAction);

            options
                .HasOne(r => r.Constructor)
                .WithMany()
                .HasForeignKey(r => r.ConstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            options
                .HasOne(r => r.DriversChampionship)
                .WithMany()
                .HasForeignKey(r => r.DriversChampId)
                .OnDelete(DeleteBehavior.NoAction);

            options
                .HasOne(r => r.ConsChampionship)
                .WithMany()
                .HasForeignKey(r => r.ConsChampId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Team -> Brand (Many-to-One)
        modelBuilder.Entity<Constructor>(options =>
            options
                .HasOne(t => t.Brand)
                .WithMany()
                .HasForeignKey(t => t.BrandId)
                .OnDelete(DeleteBehavior.NoAction)
        );

        // TeamCompetition relationships
        modelBuilder.Entity<ConstructorCompetition>(options =>
        {
            options.HasKey(tc => new { TeamId = tc.ConstructorId, tc.ConstChampId });
            
            options
                .HasOne(tc => tc.Constructor)
                .WithMany()
                .HasForeignKey(tc => tc.ConstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            options
                .HasOne(tc => tc.ConstructorsChampionship)
                .WithMany()
                .HasForeignKey(tc => tc.ConstChampId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<User>(options =>
        {
            options.Property(u => u.Role).HasDefaultValue("user");
            options.Property(u => u.HasAvatar).HasDefaultValue(false);
        });
        
        
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var idProperty = entity.FindProperty("Id");
            if (idProperty != null && idProperty.ClrType == typeof(Guid))
            {
                idProperty.SetDefaultValueSql("gen_random_uuid()");
            }
        }
    }
}