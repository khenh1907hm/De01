using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace De01.Model
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Data_Connect")
        {
        }

        public virtual DbSet<Lop> Lops { get; set; }
        public virtual DbSet<SinhVien> SinhViens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lop>()
                .Property(e => e.MaLop)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<SinhVien>()
                .Property(e => e.MaSV)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<SinhVien>()
                .Property(e => e.MaLop)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
