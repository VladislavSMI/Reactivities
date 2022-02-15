using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
  public class DataContext : DbContext
  {
    // Constructor for DataContext => it is based on DbContext class so we are passing options from DataContext contractor to DbContext contractor via base(options) (base is constructor inside of DbContext)
    public DataContext(DbContextOptions options) : base(options)
    {
    }


  // Our database table is called Activities which contains collumns that match names of our properties in Activity(Domain => Activity.cs class) 
    public DbSet<Activity> Activities { get; set; }
  }
}