using Junto.Application.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Junto.Infra.Config
{
    public class JuntoContext : DbContext
    {
        public JuntoContext() : base()
        {
        }

        public JuntoContext(DbContextOptions<JuntoContext> option) : base(option)
        {
        }

        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
