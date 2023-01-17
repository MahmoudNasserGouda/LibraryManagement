using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
    [Index(nameof(ISBN), IsUnique = true)]
    public class Book
    {
        [Key]
        [ForeignKey("BorrowedBook")]
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Author { get; set; }

        [Required]
        [IsISBN]
        public string ISBN { get; set; }

        [Required]
        public int AllCopies { get; set; }

        public int AvailableCopies { get; set; }
    }
}
