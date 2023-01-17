using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
    [PrimaryKey("MemberId", "BookId")]
    public class Borrowed
    {
        public int MemberId { get; set; }

        public int BookId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime BorrowedAt { get; set; }

        [DefaultValue(false)]
        public bool IsReturned { get; set; }

        public virtual Book? BorrowedBook { get; set; }

        public virtual Member? BorrowingMember { get; set; }

    }
}
