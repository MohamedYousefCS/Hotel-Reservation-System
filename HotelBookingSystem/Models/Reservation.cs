using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Models;

public partial class Reservation
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    [Column(TypeName = "date")]
    public DateTime CheckInDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime CheckOutDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalPrice { get; set; }

    public string PaymentStatus { get; set; }


    [ForeignKey("RoomId")]
    [InverseProperty("Reservations")]
    public virtual Room Room { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Reservations")]
    public virtual User User { get; set; }
}
