using System;
using Npgsql;
using System.Data;
using System.Text;
using BaiTap_phan3.Models;
using Npgsql.Replication;
using Dapper;

namespace BaiTap_phan3.Services{

    public class DBServices<T>{
        private readonly IConfiguration _configuration;
        private static NpgsqlConnection _connection;
        public DBServices(IConfiguration configuration){
            _configuration = configuration;
            _connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<IEnumerable<T>> GetTable(string tableName){
            await _connection.OpenAsync();
            IEnumerable<T> result = _connection.Query<T>($"SELECT * FROM \"{tableName}\"");

            await _connection.CloseAsync();
            return result;
        }

        public async Task Insert(T obj){
            await _connection.OpenAsync();
            string tableName = obj.GetType().ToString();
            string sql = $"INSERT INTO \"{tableName}\" VALUES({obj})";
            await _connection.ExecuteAsync(sql);
            await _connection.CloseAsync();
        }
    }
}