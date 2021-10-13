using System;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace PolygonTest.Database
{
    public class MyDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Place> Place { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }
    }

    public class User
    {
        public int Id { get; set; }

        public Polygon PlacePoly { get; set; }
    }

    public class Place
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
