using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Login.Models
{
    public class Weddings
    {
        [Key]
        public int WeddingId {get;set;}
        [Required]
        [Display(Name = "Wedder One")]
        public string WedderOne {get;set;} 
        [Required]
        [Display(Name = "Wedder Two")]
        public string WedderTwo {get;set;}
        [Required]
        [Display(Name = "Wedding Date")]
        public DateTime WeddingDate {get;set;}
        [Required]
        [Display(Name = "Wedding Location")]
        public string Address {get;set;}
        public int UserId {get;set;}
        public Users Planner {get;set;}
        public List<Guests> WeddingGuests {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
    }
}