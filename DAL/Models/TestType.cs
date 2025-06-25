using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

[Table("TestType")]
public partial class TestType
{
    [Key]
    [Column("test_type_id")]
    public int TestTypeId { get; set; }

    [Column("test_type_name")]
    [StringLength(200)]
    public string TestTypeName { get; set; } = null!;

    [Column("unit")]
    [StringLength(50)]
    public string Unit { get; set; } = null!;

    [Column("normal_range")]
    public string? NormalRange { get; set; }

    [InverseProperty("TestType")]
    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();
}
