
using System.Net.Mail;

namespace BaiTap_phan3.Models
{

    public class Test
    {
        public int Id { get; set; }

        public Test(int id)
        {
            Id = id;
        }

        public Test(){
            Id = 0;
        }
    }
}