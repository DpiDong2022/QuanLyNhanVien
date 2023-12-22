using System;
using BaiTap_phan3.DBContext;
using BaiTap_phan3.Interfaces;
using BaiTap_phan3.Models;
using BaiTap_phan3.Response;
using Dapper;

namespace BaiTap_phan3.Services
{
    public class PhongBanService : IPhongBanService
    {

        private readonly DapperContext _dapper;
        public PhongBanService(DapperContext dapper){
            _dapper = dapper;
        }
        public async Task<IEnumerable<PhongBan>> GetList()
        {
            string query = "SELECT * FROM \"PhongBan\"";
            using (var connection = _dapper.CreateConnection())
            {
                IEnumerable<PhongBan> phongBans = await connection.QueryAsync<PhongBan>(query);
                return phongBans;
            }
        }

        public Task<ResponseMvc> Sua(int id, PhongBan nhanVien)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMvc> Them(PhongBan nhanVien)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseMvc> Xoa(int id)
        {
            throw new NotImplementedException();
        }
    }
}