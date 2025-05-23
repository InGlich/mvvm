using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGE.Model
{
    public class AppDbContext : DbContext
    {
       
        public DbSet<Abiturient> Abiturient { get; set; }
        public DbSet<School> Shool { get; set; }
        public DbSet<Otdelenie> Otdelenie { get; set; }
        public DbSet<Specialnosti> Specialnosti { get; set; }
        public DbSet<ResultExem> ResultExem { get; set; }
    

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(ConfigurationManager.ConnectionStrings["BaseEGEConnection"].ConnectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка первичных ключей
            modelBuilder.Entity<Abiturient>().HasKey(a => a.IDAbitur);
            modelBuilder.Entity<School>().HasKey(s => s.IDSchool);
            modelBuilder.Entity<Otdelenie>().HasKey(o => o.IDOtdel);
            modelBuilder.Entity<Specialnosti>().HasKey(s => s.IDSpecial);
            modelBuilder.Entity<ResultExem>().HasKey(r => r.IDResult);

            // Настройка связей
            modelBuilder.Entity<Abiturient>()
                .HasOne(a => a.Shool)
                .WithMany(s => s.Abiturients)
                .HasForeignKey(a => a.IDSchool);

            modelBuilder.Entity<Otdelenie>()
    .Property(o => o.IDOtdel)
    .ValueGeneratedOnAdd();

            modelBuilder.Entity<Specialnosti>()
       .HasOne(s => s.Otdelenie)
       .WithMany(o => o.Specialnostis)
       .HasForeignKey(s => s.IDOtdel);

            modelBuilder.Entity<ResultExem>()
                .HasOne(r => r.Abiturient)
                .WithMany(a => a.ResultExems)
                .HasForeignKey(r => r.IDAbitur);
        }
    }

 }


