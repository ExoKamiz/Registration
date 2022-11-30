using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Registration.Models
{
    public class RegistrationContext : DbContext
    {
        public RegistrationContext(DbContextOptions<RegistrationContext> options)
            : base(options)
        {
        }

        public DbSet<RegistrUser> RegistrUsers { get; set; }
    }
}
