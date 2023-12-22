using System;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace BaiTap_phan3.DBContext{

    public class DapperContext 
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public DapperContext(IConfiguration configuration){
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()=> 
            new NpgsqlConnection(_connectionString);
    }
}