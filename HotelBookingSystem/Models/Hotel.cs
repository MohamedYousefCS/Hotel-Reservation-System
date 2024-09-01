using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Models;

public partial class Hotel
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Location { get; set; }

    [Column(TypeName = "text")]
    public string Description { get; set; }

    [InverseProperty("Hotel")]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    [InverseProperty("Hotel")]
    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
}
