

namespace BaiTap_phan3.Models{

    public class Pagination{
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public int OffsetAt { get; set; }
    }
}