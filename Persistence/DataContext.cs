using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
  //we are using here IdentityDbContext with type AppUser where we will have assecc to all predefined properties such as UserName, Email etc.
  //Without identity we can derive from just DbContext
  public class DataContext : IdentityDbContext<AppUser>
  {
    // Constructor for DataContext => it is based on DbContext class so we are passing options from DataContext constractor to DbContext constractor via base(options) (base is constructor inside of DbContext)
    public DataContext(DbContextOptions options) : base(options)
    {
    }


    // Our database table is called Activities which contains collumns that match names of our properties in Activity(Domain => Activity.cs class) 
    public DbSet<Activity> Activities { get; set; }
    //We have to create new DbSet for our join table ActivityAttnedee
    public DbSet<ActivityAttendee> ActivityAttendees { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserFollowing> UserFollowings { get; set; }


    //thanks to polymorphism, we are changing the OnModelCreating method that is defined on IdentityDbContext base class => we are adding aditional configuration

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      //now we have access to our entity framework configuration
      //this will form primary key in our db table
      //we put builder then Entity <Here we sepcify which Domain entity do we want to change> => basically we want to form key for our 2 properties that have keys => AppUserId and ActivityId
      builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new { aa.AppUserId, aa.ActivityId }));

      builder.Entity<ActivityAttendee>()
      .HasOne(u => u.AppUser)
      .WithMany(a => a.Activities)
      .HasForeignKey(aa => aa.AppUserId);

      builder.Entity<ActivityAttendee>()
      .HasOne(u => u.Activity)
      .WithMany(a => a.Attendees)
      .HasForeignKey(aa => aa.ActivityId);

      //We will add configuration when activity is deleted that a lso all comments asociated with that activity will be deleted.
      builder.Entity<Comment>()
      .HasOne(a => a.Activity)
      .WithMany(c => c.Comments)
      .OnDelete(DeleteBehavior.Cascade);

      builder.Entity<UserFollowing>(b =>
      {
        b.HasKey(k => new { k.ObserverId, k.TargetId });

        b.HasOne(o => o.Observer)
          .WithMany(f => f.Followings)
          .HasForeignKey(o => o.ObserverId)
          .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(o => o.Target)
          .WithMany(f => f.Followers)
          .HasForeignKey(o => o.TargetId)
          .OnDelete(DeleteBehavior.Cascade);
      });

    }


  }
}