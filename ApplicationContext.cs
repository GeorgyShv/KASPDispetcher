using KASPDispetcher.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;


namespace KASPDispetcher
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public virtual DbSet<Master> Masters { get; set; }

        public virtual DbSet<СonstructionSite> СonstructionSites { get; set; }

        public virtual DbSet<Position> Positions { get; set; }

        public virtual DbSet<Report> Reports { get; set; }

        public virtual DbSet<ReportState> ReportStates { get; set; }

        public virtual DbSet<ReportStateJournal> ReportStateJournals { get; set; }

        public virtual DbSet<Work> Works { get; set; }

        public virtual DbSet<WorkType> WorkTypes { get; set; }

        public virtual DbSet<Подразделение> Подразделениеs { get; set; }

        public virtual DbSet<Помещение> Помещениеs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Связь между Master и User
            modelBuilder.Entity<Master>()
                .HasOne(m => m.User)
                .WithMany(u => u.Masters)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Master>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK_MASTER");

                entity.ToTable("Master");

                entity.HasIndex(e => e.PositionId, "is_FK");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.HasOne(d => d.Position).WithMany(p => p.Masters)
                    .HasForeignKey(d => d.PositionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_MASTER_IS_POSITION");

                entity.HasOne(m => m.User)
                    .WithMany(u => u.Masters)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<СonstructionSite>(entity =>
            {
                entity.HasKey(e => e.ObjectId)
                    .HasName("PK_OBJECT")
                    .IsClustered(false);

                entity.ToTable("Object");

                entity.Property(e => e.ObjectId).ValueGeneratedNever();
                entity.Property(e => e.ObjectName)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(e => e.PositionId)
                    .HasName("PK_POSITION")
                    .IsClustered(false);

                entity.ToTable("Position");

                entity.Property(e => e.PositionId).ValueGeneratedNever();
                entity.Property(e => e.PositionName)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportId)
                    .HasName("PK_REPORT")
                    .IsClustered(false);

                entity.ToTable("Report");

                entity.HasIndex(e => e.UserId, "make_FK");

                entity.HasIndex(e => e.ObjectId, "составляется_FK");

                entity.HasIndex(e => e.DepartmentId, "формируется_FK");

                entity.Property(e => e.ReportId).ValueGeneratedNever();
                entity.Property(e => e.Data).HasColumnType("datetime");
                entity.Property(e => e.Note).HasColumnType("text");

                entity.HasOne(d => d.Department).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_REPORT_ФОРМИРУЕТ_ПОДРАЗДЕ");

                entity.HasOne(d => d.Object).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.ObjectId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_REPORT_СОСТАВЛЯЕ_OBJECT");

                entity.HasOne(d => d.User).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_REPORT_MAKE_MASTER");
            });

            modelBuilder.Entity<ReportState>(entity =>
            {
                entity.HasKey(e => e.StateId)
                    .HasName("PK_Report_states")
                    .IsClustered(false);

                entity.ToTable("Report states");

                entity.Property(e => e.StateId).ValueGeneratedNever();
                entity.Property(e => e.StateName)
                    .HasMaxLength(80)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReportStateJournal>(entity =>
            {
                entity.HasKey(e => new { e.ReportId, e.StateId }).HasName("PK_REPORT STATE JOURNAL");

                entity.ToTable("Report state journal");

                entity.HasIndex(e => e.StateId, "Report state journal2_FK");

                entity.HasIndex(e => e.ReportId, "Report state journal_FK");

                entity.Property(e => e.StateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Report).WithMany(p => p.ReportStateJournals)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK_REPORT S_REPORT ST_REPORT");

                entity.HasOne(d => d.State).WithMany(p => p.ReportStateJournals)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_REPORT S_REPORT ST_POSITION");
            });

            modelBuilder.Entity<Work>(entity =>
            {
                entity.HasKey(e => e.WorkId)
                    .HasName("PK_WORK")
                    .IsClustered(false);

                entity.ToTable("Work");

                entity.HasIndex(e => e.ReportId, "consists_FK");

                entity.HasIndex(e => e.UserId, "ЯвляетсяОтвИсполнителемРаботы_FK");

                entity.HasIndex(e => e.TypeId, "относится_FK");

                entity.Property(e => e.WorkId).ValueGeneratedNever();
                entity.Property(e => e.Attribute31)
                    .HasColumnType("datetime")
                    .HasColumnName("Attribute_31");
                entity.Property(e => e.ЗанятоЧелПлан).HasColumnName("Занято чел., план");
                entity.Property(e => e.ПериодКонец)
                    .HasColumnType("datetime")
                    .HasColumnName("Период, конец");

                entity.HasOne(d => d.Report).WithMany(p => p.Works)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK_WORK_CONSISTS_REPORT");

                entity.HasOne(d => d.Type).WithMany(p => p.Works)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WORK_ОТНОСИТСЯ_WORK TYP");
            });

            modelBuilder.Entity<WorkType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK_WORK TYPE")
                    .IsClustered(false);

                entity.ToTable("Work type");

                entity.Property(e => e.TypeId).ValueGeneratedNever();
                entity.Property(e => e.НаименованиеРаботы)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("Наименование работы");
            });

            modelBuilder.Entity<Подразделение>(entity =>
            {
                entity.HasKey(e => e.DepartmentId)
                    .HasName("PK_ПОДРАЗДЕЛЕНИЕ")
                    .IsClustered(false);

                entity.ToTable("Подразделение");

                entity.Property(e => e.DepartmentId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Помещение>(entity =>
            {
                entity.HasKey(e => e.DepartmentId2)
                    .HasName("PK_ПОМЕЩЕНИЕ")
                    .IsClustered(false);

                entity.ToTable("Помещение");

                entity.HasIndex(e => e.ObjectId, "содержит_FK");

                entity.Property(e => e.DepartmentId2).ValueGeneratedNever();

                entity.HasMany(d => d.Works).WithMany(p => p.DepartmentId2s)
                    .UsingEntity<Dictionary<string, object>>(
                        "WorkJournal",
                        r => r.HasOne<Work>().WithMany()
                            .HasForeignKey("WorkId")
                            .HasConstraintName("FK_WORK JOU_WORK JOUR_WORK"),
                        l => l.HasOne<Помещение>().WithMany()
                            .HasForeignKey("DepartmentId2")
                            .HasConstraintName("FK_WORK JOU_WORK JOUR_ПОМЕЩЕНИ"),
                        j =>
                        {
                            j.HasKey("DepartmentId2", "WorkId").HasName("PK_WORK JOURNAL");
                            j.ToTable("Work journal");
                            j.HasIndex(new[] { "WorkId" }, "Work journal2_FK");
                            j.HasIndex(new[] { "DepartmentId2" }, "Work journal_FK");
                        });
            });

        }
    }
}
