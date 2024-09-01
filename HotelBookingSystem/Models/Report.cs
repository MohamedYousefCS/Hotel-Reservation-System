using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Models;

public partial class Report
{
    [Key]
    public int Id { get; set; }

    public int AdminId { get; set; }

    [Required]
    [StringLength(50)]
    public string ReportType { get; set; }

    [Column(TypeName = "date")]
    public DateTime GeneratedOn { get; set; }

    [ForeignKey("AdminId")]
    [InverseProperty("Reports")]
    public virtual User Admin { get; set; }
}
