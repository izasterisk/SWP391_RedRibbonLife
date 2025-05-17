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

    public virtual DbSet<AppointmentReminder> AppointmentReminders { get; set; }

    public virtual DbSet<Arvregimen> Arvregimens { get; set; }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; }

    public virtual DbSet<EducationalMaterial> EducationalMaterials { get; set; }

    public virtual DbSet<Hivmonitoring> Hivmonitorings { get; set; }

    public virtual DbSet<Hivtreatment> Hivtreatments { get; set; }

    public virtual DbSet<MedicationReminder> MedicationReminders { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

    public virtual DbSet<User> Users { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCA2BE7C935E");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsOnline).HasDefaultValue(false);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Scheduled");

            entity.HasOne(d => d.DoctorCodeNavigation).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Doctors");

            entity.HasOne(d => d.PatientCodeNavigation).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Patients");
        });

        modelBuilder.Entity<AppointmentReminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Appointm__01A830A711910989");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsSent).HasDefaultValue(false);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentReminders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentReminders_Appointments");

            entity.HasOne(d => d.PatientCodeNavigation).WithMany(p => p.AppointmentReminders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppointmentReminders_Patients");
        });

        modelBuilder.Entity<Arvregimen>(entity =>
        {
            entity.HasKey(e => e.RegimenId).HasName("PK__ARVRegim__4BAC09D107919452");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__BlogPost__AA1260387DB74AC9");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsPublished).HasDefaultValue(false);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.AuthorNavigation).WithMany(p => p.BlogPosts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BlogPosts_Users");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorCode).HasName("PK__Doctors__2BDC63EA1A763E18");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Doctors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_Users");
        });

        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__DoctorSc__9C8A5B69244B322B");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.DoctorCodeNavigation).WithMany(p => p.DoctorSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorSchedules_Doctors");
        });

        modelBuilder.Entity<EducationalMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Educatio__C50613171D6A3CFA");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsPublished).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AuthorNavigation).WithMany(p => p.EducationalMaterials).HasConstraintName("FK_EducationalMaterials_Users");
        });

        modelBuilder.Entity<Hivmonitoring>(entity =>
        {
            entity.HasKey(e => e.MonitoringId).HasName("PK__HIVMonit__CAC3C0777AEBE520");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.PatientCodeNavigation).WithMany(p => p.Hivmonitorings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HIVMonitoring_Patients");
        });

        modelBuilder.Entity<Hivtreatment>(entity =>
        {
            entity.HasKey(e => e.TreatmentId).HasName("PK__HIVTreat__1A57B71192EB71DB");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Active");

            entity.HasOne(d => d.DoctorCodeNavigation).WithMany(p => p.Hivtreatments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HIVTreatments_Doctors");

            entity.HasOne(d => d.PatientCodeNavigation).WithMany(p => p.Hivtreatments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HIVTreatments_Patients");

            entity.HasOne(d => d.Regimen).WithMany(p => p.Hivtreatments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HIVTreatments_ARVRegimens");
        });

        modelBuilder.Entity<MedicationReminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Medicati__01A830A7FECC0BE5");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.PatientCodeNavigation).WithMany(p => p.MedicationReminders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedicationReminders_Patients");

            entity.HasOne(d => d.Treatment).WithMany(p => p.MedicationReminders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedicationReminders_HIVTreatments");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientCode).HasName("PK__Patients__B9C66DFF4A994B0E");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsHivpositive).HasDefaultValue(false);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Patients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A4AA78EF3");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK__TestResu__8CC33100CD29A68B");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Appointment).WithMany(p => p.TestResults).HasConstraintName("FK_TestResults_Appointments");

            entity.HasOne(d => d.PatientCodeNavigation).WithMany(p => p.TestResults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TestResults_Patients");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.TestResults).HasConstraintName("FK_TestResults_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC081300B9");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsAnonymous).HasDefaultValue(false);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
