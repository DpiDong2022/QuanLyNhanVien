

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Intrinsics.X86;

namespace BaiTap_phan3.Models{

    public class Message{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageId { get; set; }
         [ForeignKey("Users")]
        public int CreatorId { get; set; }
        [Display(Name ="Người tạo tin nhắn")]
        public virtual User? Creator {get; set;}
        [MinLength(1, ErrorMessage = "Tin nhắn không được trống")]
        [Display(Name = "Đoạn tin nhắn")]
        public string? MessageBody { get; set; }
        public DateTime CreateDate { get; set; }
        [ForeignKey("Messages")]
        public int ParentMessageId { get; set; }
        [Display(Name = "Parent message")]
        public virtual Message? ParentMessage {get; set;}
    }
}