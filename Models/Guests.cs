using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Login.Models
{
    public class Guests
    {
        [Key]
        public int GuestId {get;set;}
        public int UserId {get;set;}
        public int WeddingId {get;set;}
        public Users User {get;set;}
        public Weddings Wedding {get;set;}
    }
}