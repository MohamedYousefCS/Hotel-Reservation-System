using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Models;

public partial class Testimonial
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int HotelId { get; set; }

    [Required]
    [Column(TypeName = "text")]
    public string Content { get; set; }

    [Range(1, 10, ErrorMessage = "Please enter a value between 1 and 10.")]
    public int? Rating { get; set; }


    [ForeignKey("HotelId")]
    [InverseProperty("Testimonials")]
    public virtual Hotel Hotel { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Testimonials")]
    public virtual User User { get; set; }
}
