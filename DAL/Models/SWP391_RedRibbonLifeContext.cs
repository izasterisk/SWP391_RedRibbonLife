using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class SWP391_RedRibbonLifeContext : DbContext
{
    public SWP391_RedRibbonLifeContext()
    {
    }

    public SWP391_RedRibbonLifeContext(DbContextOptions<SWP391_RedRibbonLifeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Arvcomponent> Arvcomponents { get; set; }

    public virtual DbSet<Arvregimen> Arvregimens { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorCertificate> DoctorCertificates { get; set; }

    public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

    public virtual DbSet<TestType> TestTypes { get; set; }

    public virtual DbSet<Treatment> Treatments { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=ADMIN-PC\\SQLEXPRESS;Database=SWP391_RedRibbonLife;Uid=sa;Pwd=12345;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__A50828FC024FC867");

            entity.Property(e => e.AppointmentType).HasDefaultValue("Appointment");
            entity.Property(e => e.IsAnonymous).HasDefaultValue(false);
            entity.Property(e => e.Status).HasDefaultValue("Scheduled");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appointments_doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments).HasConstraintName("fk_appointments_patients");

            entity.HasOne(d => d.TestType).WithMany(p => p.Appointments).HasConstraintName("fk_appointments_test_type");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__CC36F660515F1847");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.Articles).HasConstraintName("fk_articles_category");

            entity.HasOne(d => d.User).WithMany(p => p.Articles).HasConstraintName("fk_articles_user");
        });

        modelBuilder.Entity<Arvcomponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__ARVCompo__AEB1DA590ED4632D");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Arvregimen>(entity =>
        {
            entity.HasKey(e => e.RegimenId).HasName("PK__ARVRegim__36DA3D9EFA6AE27A");

            entity.Property(e => e.Frequency).HasDefaultValue(1);
            entity.Property(e => e.IsCustomized).HasDefaultValue(true);

            entity.HasOne(d => d.Component1).WithMany(p => p.ArvregimenComponent1s)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_component1");

            entity.HasOne(d => d.Component2).WithMany(p => p.ArvregimenComponent2s).HasConstraintName("fk_component2");

            entity.HasOne(d => d.Component3).WithMany(p => p.ArvregimenComponent3s).HasConstraintName("fk_component3");

            entity.HasOne(d => d.Component4).WithMany(p => p.ArvregimenComponent4s).HasConstraintName("fk_component4");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__D54EE9B4A01EC514");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__F3993564D9BCCA1C");

            entity.HasOne(d => d.User).WithMany(p => p.Doctors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_doctors_users");
        });

        modelBuilder.Entity<DoctorCertificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId).HasName("PK__DoctorCe__E2256D3121B1C5C6");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorCertificates).HasConstraintName("fk_certificates_doctors");
        });

        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__DoctorSc__C46A8A6F2ACFFCF5");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_schedules_doctors");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__E059842F6CB04F00");

            entity.Property(e => e.RetryCount).HasDefaultValue(0);
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Notifications).HasConstraintName("fk_notifications_appointments");

            entity.HasOne(d => d.Treatment).WithMany(p => p.Notifications).HasConstraintName("fk_notifications_treatment");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_notifications_users");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__4D5CE4768827148B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsPregnant).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Patients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_patients_users");
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.TestResultId).HasName("PK__TestResu__152BCEDA8D9B7751");

            entity.HasOne(d => d.Appointment).WithMany(p => p.TestResults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_test_results_appointments");

            entity.HasOne(d => d.Doctor).WithMany(p => p.TestResults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_test_results_doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.TestResults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_test_results_patients");

            entity.HasOne(d => d.TestType).WithMany(p => p.TestResults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_test_results_test_type");
        });

        modelBuilder.Entity<TestType>(entity =>
        {
            entity.HasKey(e => e.TestTypeId).HasName("PK__TestType__56DCFA21B1B720BC");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Unit).HasDefaultValue("N/A");
        });

        modelBuilder.Entity<Treatment>(entity =>
        {
            entity.HasKey(e => e.TreatmentId).HasName("PK__Treatmen__302D3CA021A884F3");

            entity.Property(e => e.Status).HasDefaultValue("Active");

            entity.HasOne(d => d.Regimen).WithMany(p => p.Treatments).HasConstraintName("fk_treatment_regimen");

            entity.HasOne(d => d.TestResult).WithMany(p => p.Treatments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_treatment_test_results");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FCD3F6D18");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
