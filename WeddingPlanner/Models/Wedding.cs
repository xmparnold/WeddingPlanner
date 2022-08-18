#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId { get; set; }

    [Required(ErrorMessage ="is required")]
    [MinLength(2, ErrorMessage ="must be at least 2 characters")]
    [MaxLength(15, ErrorMessage ="must be less than 15 characters")]
    public string WedderOne { get; set; }

    [Required(ErrorMessage ="is required")]
    [MinLength(2, ErrorMessage ="must be at least 2 characters")]
    [MaxLength(15, ErrorMessage ="must be less than 15 characters")]
    public string WedderTwo { get; set; }

    [Required]
    [FutureDate]
    public DateTime Date { get; set; }

    [Required]
    public string WeddingAddress { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int UserId { get; set; }
    public User? Creator { get; set; }
    public List<RSVP> Attendees { get; set; } = new List<RSVP>();

    
}