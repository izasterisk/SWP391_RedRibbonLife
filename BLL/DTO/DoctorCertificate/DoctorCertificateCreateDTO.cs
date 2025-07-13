using System.ComponentModel.DataAnnotations;
using BLL.Utils;

namespace BLL.DTO.DoctorCertificate;

public class DoctorCertificateCreateDTO
{
    public int CertificateId { get; set; }

    [Required(ErrorMessage = "Doctor ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Doctor ID must be a positive number")]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "Certificate name is required")]
    [StringLength(100, ErrorMessage = "Certificate name cannot exceed 100 characters")]
    public string CertificateName { get; set; }

    [Required(ErrorMessage = "Issued by is required")]
    [StringLength(100, ErrorMessage = "Issued by cannot exceed 100 characters")]
    public string IssuedBy { get; set; }

    [Required(ErrorMessage = "Issue date is required")]
    public DateOnly IssueDate { get; set; }

    [Required(ErrorMessage = "Expiry date is required")]
    [DateRange("IssueDate")]
    public DateOnly ExpiryDate { get; set; }

    [StringLength(255, ErrorMessage = "Certificate image path cannot exceed 255 characters")]
    public string? CertificateImage { get; set; }
}