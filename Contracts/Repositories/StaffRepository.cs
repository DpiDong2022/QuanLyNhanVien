
using BaiTap_phan3.Models;
using BaiTap_phan3.DBContext;
using Dapper;
using System.Text;
using System.Security.Cryptography.Pkcs;
using System.Text.RegularExpressions;

namespace BaiTap_phan3.Contracts.Repositories
{

    public interface IStaffRepository<T> : IGenericRepository<T>
    {
        Task<PageResult<NhanVien>> TimKiem(string searhKey, int phongBanId, int chucVu, Pagination pagination, bool IsPaginated = true);
        Task<int> Count(int phongBanId = -1, int chucVuId = -1, string searchKey = "");
        Task<bool> KiemTraTrung(NhanVien nhanVien);
    }

    public class StaffRepository : IStaffRepository<NhanVien>
    {
        private readonly DapperContext<NhanVien> _dbContext;
        private readonly IConfiguration _configuration;

        public StaffRepository(DapperContext<NhanVien> dapperContext, IConfiguration configuration)
        {
            _dbContext = dapperContext;
            _configuration = configuration;
        }

        public async Task<bool> Delete(object id)
        {
            return await _dbContext.Delete(id);
        }

        public async Task<List<NhanVien>> GetAll()
        {
            string sql = "SELECT n.*, p.* FROM \"NhanVien\" as n " +
            "JOIN \"PhongBan\" as p ON n.\"PhongBanId\" = p.\"Id\"";

            using (var conenction = _dbContext.CreateConnection())
            {
                IEnumerable<NhanVien> nhanViens = await conenction.QueryAsync<NhanVien, PhongBan, NhanVien>(sql, (nhanVien, phongBan) =>
                {
                    nhanVien.PhongBan = phongBan;
                    return nhanVien;
                }, splitOn: "PhongBanId");

                return nhanViens.ToList();
            }
        }

        public async Task<NhanVien> GetById(object id)
        {
            return await _dbContext.GetById(id);
        }

        public async Task<int> Insert(NhanVien obj)
        {
            return await _dbContext.Insert(obj);
        }

        public async Task<bool> Update(NhanVien obj, object id)
        {
            return await _dbContext.Update(id, obj);
        }


        public async Task<PageResult<NhanVien>> TimKiem(string searchKey, int phongBanId, int chucVuId, Pagination pagination, bool IsPaginated = true)
        {
            List<string> conditions = new List<string>();
            StringBuilder sqlBuilder = new StringBuilder("SELECT n.\"Id\", n.\"HoVaTen\", n.\"NgaySinh\", n.\"DienThoai\", " +
                                                        "p.\"Id\", p.\"TenPhongBan\", " +
                                                        "c.\"Id\", c.\"TenChucVu\" " +
                                                        "FROM \"NhanVien\" as n " +
                                                        "JOIN \"ChucVu\" as c on c.\"Id\" = n.\"ChucVuId\" " +
                                                        "JOIN \"PhongBan\" as p on p.\"Id\" = n.\"PhongBanId\"");

            if (phongBanId >= 1)
            {
                conditions.Add($"n.\"PhongBanId\" = {phongBanId}");
            }
            if (chucVuId >= 1)
            {
                conditions.Add($"n.\"ChucVuId\" = {chucVuId}");
            }

            if (!string.IsNullOrEmpty(searchKey))
            {
                string tempSearchKey = searchKey.Trim();

                tempSearchKey = Regex.Replace(searchKey, @"[^\p{L}0-9 ]", "").ToLower();
                tempSearchKey = Regex.Replace(tempSearchKey, @"\s+", " ");
                tempSearchKey = tempSearchKey.Replace(" ", " | ");

                string searchTerm = $"unaccent('{tempSearchKey}')::tsquery";
                conditions.Add($"n.\"Document\" @@ {searchTerm} ORDER BY ts_rank(n.\"Document\", {searchTerm}) desc, n.\"Id\"");
            }

            if (conditions.Count() > 0)
            {
                sqlBuilder.Append(" WHERE ");
                sqlBuilder.Append(string.Join(" AND ", conditions));
            }

            if (IsPaginated)
            {
                // lấy ra bản ghi phân trang
                pagination.PageSize = int.Parse(_configuration["Frontend:PageSize"]);
                int skipAmount = (pagination.PageNumber - 1) * pagination.PageSize;
                pagination.OffsetAt = skipAmount;

                if (string.IsNullOrEmpty(searchKey))
                {
                    sqlBuilder.Append(" ORDER BY n.\"Id\"");
                }
                sqlBuilder.Append(" LIMIT(" + pagination.PageSize + ")" +
                       " OFFSET " + skipAmount);
            }
            else
            {
                sqlBuilder.Append(" ORDER BY n.\"PhongBanId\", n.\"ChucVuId\"");
            }


            using (var conenction = _dbContext.CreateConnection())
            {
                IEnumerable<NhanVien> nhanViens = await conenction.QueryAsync<NhanVien, PhongBan, ChucVu, NhanVien>(sqlBuilder.ToString(), (nhanVien, phongBan, chucVu) =>
                {
                    nhanVien.PhongBan = phongBan;
                    nhanVien.ChucVu = chucVu;
                    return nhanVien;
                }, splitOn: "Id");

                int totalPage;
                int totalRecord = await Count(phongBanId, chucVuId, searchKey);
                if (totalRecord < pagination.PageSize)
                {
                    totalPage = 1;
                }
                totalPage = (int)Math.Ceiling((decimal)totalRecord / pagination.PageSize);
                PageResult<NhanVien> result = new PageResult<NhanVien>() { TotalPage = totalPage, Data = nhanViens };
                result.Pagination = pagination;
                return result;
            }
        }

        public async Task<int> Count(int phongBanId = -1, int chucVuId = -1, string searchKey = "")
        {
            List<string> conditions = new List<string>();
            StringBuilder sqlBuilder = new StringBuilder("SELECT count(*) from (select 1 from \"NhanVien\" as n");

            if (phongBanId >= 1)
            {
                conditions.Add($"n.\"PhongBanId\"={phongBanId}");
            }
            if (chucVuId >= 1)
            {
                conditions.Add($"n.\"ChucVuId\"={chucVuId}");
            }

            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = searchKey.Trim();
                searchKey = Regex.Replace(searchKey, @"[^\p{L}0-9 ]", "").ToLower();
                searchKey = Regex.Replace(searchKey, @"\s+", " ");
                searchKey = searchKey.Replace(" ", " | ");
                string searchTerm = $"unaccent('{searchKey}')::tsquery";
                conditions.Add($"n.\"Document\" @@ {searchTerm} ORDER BY ts_rank(n.\"Document\", {searchTerm}) desc");
            }

            if (conditions.Count() > 0)
            {
                sqlBuilder.Append(" WHERE ");
                sqlBuilder.Append(string.Join(" AND ", conditions));
            }

            sqlBuilder.Append(") as temp");

            using (var conenction = _dbContext.CreateConnection())
            {
                int count = await conenction.QuerySingleAsync<int>(sqlBuilder.ToString());

                return count;
            }
        }

        public async Task<bool> KiemTraTrung(NhanVien nhanVien)
        {
            string hoVaTen = nhanVien.HoVaTen.ToLower();
            string sql = $"select exists (select 1 from \"NhanVien\" where \"HoVaTen\" ILIKE '{hoVaTen}' and \"NgaySinh\" = '{nhanVien.NgaySinh.ToString("yyyy-MM-dd")}' )";
            if (nhanVien.Id >= 1)
            {
                sql = sql.Insert(sql.Length - 2, $"and \"Id\"!={nhanVien.Id}");
            }

            using (var conenction = _dbContext.CreateConnection())
            {
                bool isExisted = await conenction.QuerySingleAsync<bool>(sql);

                return isExisted;
            }
        }
    }
}