using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Models;

public partial class Room
{
    [Key]
    public int Id { get; set; }

    public int HotelId { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string RoomType { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }


    [StringLength(255)]
    public string ImagePath { get; set; }

    public bool? AvailabilityStatus { get; set; }

    [ForeignKey("HotelId")]
    [InverseProperty("Rooms")]
    public virtual Hotel Hotel { get; set; }

    [InverseProperty("Room")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
