using System;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Reflection;
using Microsoft.VisualBasic;
using OfficeOpenXml.Utils;
using System.Diagnostics;
using BaiTap_phan3.Models;

namespace BaiTap_phan3.DBContext
{

    public class DapperContext<T> where T : class
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;
        private readonly string _keyColumnName;
        private readonly string _tableName;
        private readonly List<string> _thuocTinhs;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("NhanVien");

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
            _thuocTinhs = typeof(T).GetProperties().Select(thuocTinh => thuocTinh.Name).ToList();
            for (int index = 0; index < _thuocTinhs.Count(); index++)
            {
                if (_thuocTinhs[index].ToLower().Contains("id"))
                {
                    for (int index2 = index+1; index2 < _thuocTinhs.Count(); index2++)
                    {
                        if (_thuocTinhs[index].ToLower().Contains(_thuocTinhs[index2].ToLower()))
                        {
                            _thuocTinhs.RemoveAt(index2);
                        }
                    }
                }

            }
        }
        public IDbConnection CreateConnection() =>
            new NpgsqlConnection(_connectionString);


        public async Task<List<T>> GetAll()
        {
                using (var connection = CreateConnection())
                {
                    string sql = "select * from \"" + _tableName + "\" ORDER BY \"Id\" DESC";
                    List<T> values = connection.QueryAsync<T>(sql).Result.ToList();

                    return values;
                }
        }

        public async Task<T> GetById(object id)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    string sql = "select * from \"" + _tableName + "\" where \"" + _keyColumnName + "\"=" + id.ToString() + " LIMIT 1";
                    IEnumerable<T> values = await connection.QueryAsync<T>(sql);

                    return values.First();
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<T> Update(object id, T obj)
        {
                string sql = "update \"" + _tableName + "\" set ";
                foreach (string thuocTinh in _thuocTinhs)
                {
                    sql += "\"" + thuocTinh + "\"=@" + thuocTinh.Substring(0, 1).ToLower() + thuocTinh.Substring(1) + ",";
                }
                sql = sql.Remove(sql.Length - 1, 1);
                sql += " where \"" + _keyColumnName + "\" = " + id.ToString();

                //sql += " returning (select * From \""+_tableName+"\" where \""+_keyColumnName+"\"="+id+" LiMIT 1)" ;

                using (var connection = CreateConnection())
                {
                    int rowEffected = connection.Execute(sql, obj);
                    return await GetById(id);
                }
        }

        public async Task<bool> Delete(object id)
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

        public async Task<T> Insert(T obj)
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
                    return await GetById(id);
                }
        }

    }
}