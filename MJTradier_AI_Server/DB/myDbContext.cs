using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJTradier_AI_Server.AI
{
    public class myDbContext : DbContext
    {

        public DbSet<ScaleDatas> scaleDatasDict { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=221.149.119.60;port=2023;database=mjtradierdb;user=meancl;password=1234");
        }

        // 엔티티의 제약조건을 삽입해준다.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ScaleDatas>(entity =>
            {
                entity.HasKey(k => new { k.dTime, k.sScaleMethod, k.sVariableName, k.sModelName });
            });

        }
    }
}
