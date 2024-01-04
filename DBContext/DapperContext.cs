using System;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Reflection;
using Microsoft.VisualBasic;
using OfficeOpenXml.Utils;
using System.Diagnostics;

namespace BaiTap_phan3.DBContext
{

    public class DapperContext<T> where T : class
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;
        private readonly string _keyColumnName;
        private readonly string _tableName;
        private readonly string[] _thuocTinhs;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");

            // lấy tên khóa chính
            foreach (PropertyInfo Info in typeof(T).GetProperties())
            {
                if (Info.Name.ToLower().Contains("id"))
                {
                    _keyColumnName = Info.Name;
                    break;
                }
            }

            // lấy tên bảng
            _tableName = typeof(T).FullName.Split('.').Last();

            // lấy tên các thuộc tính
            _thuocTinhs = typeof(T).GetProperties().Select(thuocTinh => thuocTinh.Name).ToArray();
        }
        public IDbConnection CreateConnection() =>
            new NpgsqlConnection(_connectionString);

        public async Task<IEnumerable<T>> GetAll()
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    string sql = "select * from \"" + _tableName + "\"";
                    IEnumerable<T> values = await connection.QueryAsync<T>(sql);

                    return values;
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<bool> Update(object id, T obj)
        {
            try
            {
                string sql = "update \"" + _tableName + "\" set ";
                foreach (string thuocTinh in _thuocTinhs)
                {
                    sql += "\"" + thuocTinh + "\"=@" + thuocTinh.Substring(0, 1).ToLower() + thuocTinh.Substring(1) + ",";
                }
                sql = sql.Remove(sql.Length - 1, 1);
                sql += " where \"" + _keyColumnName + "\" = " + id.ToString();

                using (var connection = CreateConnection())
                {
                    int rowAffected = await connection.ExecuteAsync(sql, obj);
                    if (rowAffected > 0)
                    {
                        return true;
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> Delete(object id)
        {
            try
            {
                string sql = "Delete from \"" + _tableName + "\" where \"" + _keyColumnName + "\"=" + id.ToString();
                using (var connection = CreateConnection())
                {
                    int rowAffected = await connection.ExecuteAsync(sql);
                    if (rowAffected > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<object> Insert(T obj)
        {
            try
            {
                string sql = "insert into \"" + _tableName + "\"(";
                foreach (string tenThuocTinh in _thuocTinhs)
                {
                    if (tenThuocTinh != _keyColumnName)
                    {
                        sql += "\"" + tenThuocTinh + "\",";
                    }

                }
                sql = sql.Remove(sql.Length - 1, 1);
                sql += ") values(";

                foreach (string tenThuocTinh in _thuocTinhs)
                {
                    if (tenThuocTinh != _keyColumnName)
                    {
                        sql += "@" + tenThuocTinh.Substring(0, 1).ToLower() + tenThuocTinh.Substring(1) + ",";
                    }
                }
                sql = sql.Remove(sql.Length - 1, 1);
                sql += ") returning \"" + _keyColumnName + "\"";


                using (var connection = CreateConnection())
                {
                    int id = connection.QuerySingle<int>(sql, obj);
                    return id;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}