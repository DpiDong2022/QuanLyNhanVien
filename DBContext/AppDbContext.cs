
using BaiTap_phan3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MessageApp2.DATA{

    public class AppDbContext: DbContext{
        private readonly IConfiguration _configuration;

        public AppDbContext(IConfiguration configuration)
        {   
            _configuration = configuration;
        }

        public DbSet<User> Users {get; set;}
        public DbSet<Message> Messages {get; set;}
        public DbSet<MessageRecipient> MessageRecipients {get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("MessageApp"));
            //optionsBuilder.UseNpgsql("server = localhost; Port = 5433; User Id = postgres; password = admin; database = MessageApp");
        }
    }
}