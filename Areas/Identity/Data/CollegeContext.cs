using College.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace College.Data;

public class CollegeContext : IdentityDbContext<AspNetUsers>
{
    public CollegeContext(DbContextOptions<CollegeContext> options)
        : base(options)
    {
    }

    public DbSet<DepartmentsModel> departments { get; set; }
    public DbSet<StudentsModel> students { get; set; }
    public DbSet<ClubsModel> clubs { get; set; }
    public DbSet<JoinedClubsModel> joinedClubs { get; set; }
    public DbSet<SuggestionsModel> suggestions { get; set; }
    public DbSet<EventsModel> events { get; set; }
    public DbSet<JoinedEventsModel> joinedEvents { get; set; }




    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
