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

    public virtual DbSet<Arvregimen> Arvregimens { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorCertificate> DoctorCertificates { get; set; }

    public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; }

    public virtual DbSet<MedicationSchedule> MedicationSchedules { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<Reminder> Reminders { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

    public virtual DbSet<TreatmentHistory> TreatmentHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=ADMIN-PC\\SQLEXPRESS;Database=SWP391_RedRibbonLife;Uid=sa;Pwd=12345;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__A50828FC05F698CC");

            entity.Property(e => e.AppointmentType).HasDefaultValue("Appointment");
            entity.Property(e => e.IsAnonymous).HasDefaultValue(false);
            entity.Property(e => e.Status).HasDefaultValue("Scheduled");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_appointments_doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments).HasConstraintName("fk_appointments_patients");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__CC36F6603AA2E179");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.Articles).HasConstraintName("fk_articles_category");
        });

        modelBuilder.Entity<Arvregimen>(entity =>
        {
            entity.HasKey(e => e.RegimenId).HasName("PK__ARVRegim__36DA3D9E8412018C");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__D54EE9B4B8885AB0");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__F3993564FD42679C");

            entity.HasOne(d => d.User).WithMany(p => p.Doctors).HasConstraintName("fk_doctors_users");
        });

        modelBuilder.Entity<DoctorCertificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId).HasName("PK__DoctorCe__E2256D318262A2F1");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorCertificates).HasConstraintName("fk_certificates_doctors");
        });

        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__DoctorSc__C46A8A6FDE82C3D4");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSchedules).HasConstraintName("fk_schedules_doctors");
        });

        modelBuilder.Entity<MedicationSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Medicati__C46A8A6FBD180EA1");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicationSchedules).HasConstraintName("fk_medication_patients");

            entity.HasOne(d => d.Prescription).WithMany(p => p.MedicationSchedules).HasConstraintName("fk_medication_prescriptions");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__4D5CE4762573B084");

            entity.Property(e => e.IsPregnant).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Patients).HasConstraintName("fk_patients_users");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("PK__Prescrip__3EE444F8ED43E531");

            entity.HasOne(d => d.Regimen).WithMany(p => p.Prescriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_prescriptions_regimen");

            entity.HasOne(d => d.Treatment).WithMany(p => p.Prescriptions).HasConstraintName("fk_prescriptions_treatment");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Reminder__E27A36283A550BB7");

            entity.Property(e => e.ReminderType).HasDefaultValue("Appointment");
            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Reminders).HasConstraintName("fk_reminders_appointments");
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.TestResultId).HasName("PK__TestResu__152BCEDA12BFDA26");

            entity.Property(e => e.Unit).HasDefaultValue("N/A");

            entity.HasOne(d => d.Appointment).WithMany(p => p.TestResults).HasConstraintName("fk_test_results_appointments");

            entity.HasOne(d => d.Doctor).WithMany(p => p.TestResults).HasConstraintName("fk_test_results_doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.TestResults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_test_results_patients");
        });

        modelBuilder.Entity<TreatmentHistory>(entity =>
        {
            entity.HasKey(e => e.TreatmentId).HasName("PK__Treatmen__302D3CA0B32B87C0");

            entity.Property(e => e.Status).HasDefaultValue("Active");

            entity.HasOne(d => d.Doctor).WithMany(p => p.TreatmentHistories).HasConstraintName("fk_treatment_doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.TreatmentHistories).HasConstraintName("fk_treatment_patients");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FD92F78FB");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
