
using BaiTap_phan3.Models;

namespace BaiTap_phan3.Interfaces{

    public interface IPhongBanService
    {
        Task<IEnumerable<PhongBan>> GetList();
    }
}