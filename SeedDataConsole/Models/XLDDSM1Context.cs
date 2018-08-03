using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SeedDataConsole.Models
{
    public partial class XLDDSM1Context : DbContext
    {
        public XLDDSM1Context()
        {
        }

        public XLDDSM1Context(DbContextOptions<XLDDSM1Context> options)
            : base(options)
        {
        }

        public virtual DbSet<ProjectInfo> ProjectInfo { get; set; }
        public virtual DbSet<SensorDataOrigin> SensorDataOrigin { get; set; }
        public virtual DbSet<SensorInfo> SensorInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=XLDDSM1;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectInfo>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<SensorDataOrigin>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => new { e.SensorId, e.MeaTime })
                    .HasName("SensorId_MeaTime_Clustered")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.MeaTime).HasColumnType("datetime");

                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SensorId).HasColumnName("SensorID");

                entity.HasOne(d => d.Sensor)
                    .WithMany(p => p.SensorDataOrigin)
                    .HasForeignKey(d => d.SensorId)
                    .HasConstraintName("FK_SensorDataOrigin_SensorInfo");
            });

            modelBuilder.Entity<SensorInfo>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => new { e.ProjectId, e.SensorCode })
                    .HasName("ProjectID_Name_ClusteredIndex")
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.LayLocation)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LocationX)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LocationY)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LocationZ)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.ProjectSiteId).HasColumnName("ProjectSiteID");

                entity.Property(e => e.SensorCode).HasMaxLength(50);

                entity.Property(e => e.SensorTypeId).HasColumnName("SensorTypeID");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.SensorInfo)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SensorInfo_ProjectInfo");
            });
        }
    }
}
