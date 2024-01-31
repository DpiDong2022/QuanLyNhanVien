using System;

namespace BaiTap_phan3.Models
{
    public class ChucVu
    {
        public int Id { get; set; }
        public string TenChucVu { get; set; }

        public override string ToString()
        {
            return TenChucVu;
        }

        public ChucVu()
        {

        }
    }


}