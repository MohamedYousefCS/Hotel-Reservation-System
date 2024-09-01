using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Models;



[Index("Email", Name = "UQ__Users__A9D10534ECAE0B43", IsUnique = true)]



public partial class User
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
    public string Email { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; }

    [StringLength(50)]

    public string? Address { get; set; }

    [MaxLength(50)]
    [MinLength(11)]
    public string? Phone {  get; set; }

    [Required]
    [StringLength(50)]
    public string Role { get; set; }

    [InverseProperty("Admin")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [InverseProperty("User")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [InverseProperty("User")]
    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
}
