using System;

namespace BaiTap_phan3.Models
{
    public class PhongBan
    {
        public int Id { get; set; }
        public string TenPhongBan { get; set; }

        public override string ToString()
        {
            return TenPhongBan;
        }

        public PhongBan()
        {

        }
    }


}