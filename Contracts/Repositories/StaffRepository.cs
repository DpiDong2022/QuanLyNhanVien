
using BaiTap_phan3.Models;
using BaiTap_phan3.DBContext;
using Dapper;

namespace BaiTap_phan3.Contracts.Repositories
{

    public interface IStaffRepository<T> : IGenericRepository<T>
    {
        Task<PageResult<NhanVien>> TimKiem(string searhKey, int phongBanId, Pagination pagination, bool IsPaginated=true);
        Task<int> Count(int phongBanId = -1, string tuKhoaTimKiem = "");
    }

    public class StaffRepository : IStaffRepository<NhanVien>
    {
        private readonly DapperContext<NhanVien> _dbContext;

        public StaffRepository(DapperContext<NhanVien> dapperContext)
        {
            _dbContext = dapperContext;
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

        public async Task<NhanVien> Insert(NhanVien obj)
        {
            return await _dbContext.Insert(obj);
        }

        public async Task<NhanVien> Update(NhanVien obj, object id)
        {
            return await _dbContext.Update(id, obj);
        }


        public async Task<PageResult<NhanVien>> TimKiem(string searchKey, int phongBanId, Pagination pagination, bool IsPaginated = true)
        {
            string sql = "SELECT n.*, p.* FROM \"NhanVien\" as n " +
            "JOIN \"PhongBan\" as p ON n.\"PhongBanId\" = p.\"Id\" ";

            // loc phong ban
            if (phongBanId >= 1)
            {
                sql += "WHERE n.\"PhongBanId\" = " + phongBanId;
            }

            //lọc bằng từ khóa
            if (!string.IsNullOrEmpty(searchKey))
            {

                string[] keys = searchKey.Split(" ");
                keys = keys.Where(c => c != "").ToArray();

                if (phongBanId >= 1)
                {
                    sql += " AND (";
                }
                else
                {
                    sql += "WHERE ";
                }

                foreach (var key in keys)
                {
                    sql += $" n.\"HoVaTen\" ILIKE '%{key}%' OR";
                }

                sql = sql.Substring(0, sql.Length - 3);

                if (phongBanId >= 1)
                {
                    sql += ")";
                }
            }

            // lấy ra bản ghi phân trang
            int skipAmount = (pagination.PageNumber - 1) * pagination.PageSize;

            sql += " ORDER BY n.\"Id\" ";
            if (IsPaginated)
            {
                sql += " LIMIT(" + pagination.PageSize + ")" +
                       " OFFSET " + skipAmount;
            }
            pagination.OffsetAt = skipAmount;

            using (var conenction = _dbContext.CreateConnection())
            {
                IEnumerable<NhanVien> nhanViens = await conenction.QueryAsync<NhanVien, PhongBan, NhanVien>(sql, (nhanVien, phongBan) =>
                {
                    nhanVien.PhongBan = phongBan;
                    return nhanVien;
                }, splitOn: "PhongBanId");

                int totalPage;
                int totalRecord = await Count(phongBanId, searchKey);
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

        public async Task<int> Count(int phongBanId = -1, string tuKhoaTimKiem = "")
        {
            string sql = "SELECT count(*) FROM \"NhanVien\" as n " +
            "JOIN \"PhongBan\" as p ON n.\"PhongBanId\" = p.\"Id\" ";

            // loc phong ban
            if (phongBanId >= 1)
            {
                sql += "WHERE n.\"PhongBanId\" = " + phongBanId;
            }


            //lọc bằng từ khóa
            if (!string.IsNullOrEmpty(tuKhoaTimKiem))
            {
                string[] keys = tuKhoaTimKiem.Split(" ");
                keys = keys.Where(c => c != "").ToArray();

                if (phongBanId >= 1)
                {
                    sql += " AND (";
                }
                else
                {
                    sql += " where ";
                }


                foreach (var key in keys)
                {
                    sql += $" n.\"HoVaTen\" LIKE '%{key}%' OR";
                }

                sql = sql.Substring(0, sql.Length - 3);

                if (phongBanId >= 1)
                {
                    sql += ")";
                }
            }

            using (var conenction = _dbContext.CreateConnection())
            {
                int count = await conenction.QuerySingleAsync<int>(sql);

                return count;
            }
        }
    }
}