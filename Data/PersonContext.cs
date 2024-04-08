using Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data
{
    public class PersonContext: DbContext
    {
        public PersonContext(DbContextOptions<PersonContext> options)
                    : base(options)
        {

        }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<CreateEvent> CreateEvent { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        //public DbSet<eventmodel> events { get; set; }
        //public DbSet<eventbooking> eventbookings { get; set; }
    }
}
