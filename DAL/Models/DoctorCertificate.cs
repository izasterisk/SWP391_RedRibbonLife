using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class DoctorCertificate
{
    [Key]
    [Column("certificate_id")]
    public int CertificateId { get; set; }

    [Column("doctor_id")]
    public int? DoctorId { get; set; }

    [Column("certificate_name")]
    [StringLength(100)]
    public string? CertificateName { get; set; }

    [Column("issued_by")]
    [StringLength(100)]
    public string? IssuedBy { get; set; }

    [Column("issue_date")]
    public DateOnly? IssueDate { get; set; }

    [Column("expiry_date")]
    public DateOnly? ExpiryDate { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("DoctorCertificates")]
    public virtual Doctor? Doctor { get; set; }
}
