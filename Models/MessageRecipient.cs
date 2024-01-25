

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaiTap_phan3.Models{

    public class MessageRecipient{
        [Key]
        public int Id { get; set; }
        public int RecipientId { get; set; }
        [ForeignKey("User")]
        [Display(Name = "Người nhận tin nhắn")]
        public virtual User? Recipient{get; set;}
        public int MessageId { get; set; }
        [ForeignKey("Message")]
        public virtual Message? MessageBody {get; set;}
        public int IsRead { get; set; }
    }
}