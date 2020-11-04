using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace BazyDanych
{
    class Program
    {
        static void Main(string[] args)
        {
            //dapper();
            //adonet();
            //entityframework();
            //compiled();
            //rawsql();
        }

        static void dapper()
        {
            string connString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            Console.WriteLine("Hello World!");
            using (var comm = new SqlConnection(connString))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var zamówienia = comm.Query("Select * from Zamówienia").ToList();
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine(elapsedMs);
            }

        }
        static void adonet()
        {
            string connString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connString);

            sqlConnection.Open();

            using (SqlCommand comm = new SqlCommand("SELECT * from Zamówienia", sqlConnection))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                using (SqlDataReader reader = comm.ExecuteReader())
                {
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    Console.WriteLine(elapsedMs);
                }
            }

            sqlConnection.Close();
        }

        static void entityframework()
        {
            using (var context = new ZamówieniaContext())
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var zm = context.Zamówienia.Select(z => z);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine(elapsedMs);
            }
        }

        static void compiled()
        {
            var query = EF.CompileQuery((ZamówieniaContext context) =>
                    context.Zamówienia.Select(z => z));

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine(query);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMs);
        }

        static void rawsql()
        {
            using (var ctx = new ZamówieniaContext())
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                
                var zm = ctx.Zamówienia
                                    .FromSqlInterpolated($"Select * from Zamówienia")
                                    .ToList();

                Console.WriteLine(zm);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine(elapsedMs);
            }
        }
    }


    public class ZamówieniaContext : DbContext
    {
        public DbSet<Zamówienia> Zamówienia { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server==LAPTOP-HJ934Q3G;Database=Northwind;Trusted_Connection=True;");
        }
    }

    public class Zamówienia
    {
        [Key]
        public int IDZamówienia { get; set; }
        public string IDKlienta { get; set; }
        public int IDPracownika { get; set; }
        public DateTime DataZamówienia { get; set; }
        public DateTime DataWymagana { get; set; }
        public DateTime DataWysyłki { get; set; }
        public int IDSpedytora { get; set; }
        public double Fracht { get; set; }
        public string NazwaOdbiorcy { get; set; }   
        public string AdresOdbiorcy { get; set; }   
        public string MiastoOdbiorcy { get; set; }   
        public string RegionOdbiorcy { get; set; }   
        public string KodPocztowyOdbiorcy { get; set; }
        public string KrajOdbiorcy { get; set; }
    }
}
