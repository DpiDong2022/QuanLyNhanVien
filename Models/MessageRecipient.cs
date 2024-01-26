

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaiTap_phan3.Models
{

    public class MessageRecipient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessReciId { get; set; }
        [ForeignKey("Users")]
        public int RecipientId { get; set; }
        [Display(Name = "Người nhận tin nhắn")]
        public virtual User? Recipient { get; set; }
        [ForeignKey("Messages")]
        public int MessageId { get; set; }
        public virtual Message? MessageBody { get; set; }
        public int IsRead { get; set; }
    }
}