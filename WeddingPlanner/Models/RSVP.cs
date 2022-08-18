#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;

// [NotMapped] taghelper will define a class/property to not be stored in your db
public class RSVP
{
    [Key]
    public int RSVPId { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public int WeddingId { get; set; }
    public Wedding? WeddingAttending { get; set; }


}