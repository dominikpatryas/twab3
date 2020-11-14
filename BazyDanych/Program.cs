using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
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
            dapper();
            adonet();
            entityframework();
            compiled();
            rawsql();
        }

        static void dapper()
        {
            Console.WriteLine("Dapper: ");
            string connString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;

            using (var comm = new SqlConnection(connString))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var zamówienia = comm.Query("Select * from Orders").ToList();
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine(elapsedMs);
            }
        }
        static void adonet()
        {
            Console.WriteLine("ADO.NET: ");

            string connString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connString);

            sqlConnection.Open();

            using (SqlCommand comm = new SqlCommand("SELECT * from Orders", sqlConnection))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                List<Order> orders = new List<Order>();
                using (SqlDataReader reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string oid = reader["OrderID"].ToString();
                        orders.Add(new Order { OrderID = int.Parse(oid) });
                    }
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    Console.WriteLine(elapsedMs);
                }
            }

            sqlConnection.Close();
        }

        static void entityframework()
        {
            Console.WriteLine("EF: ");

            using (var context = new ZamówieniaContext())
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var zm = context.Orders.Select(z => z).ToList();
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine(elapsedMs);
            }
        }

        static void compiled()
        {
            Console.WriteLine("Compiled: ");
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var query = EF.CompileQuery((ZamówieniaContext context) =>
                    context.Orders.Select(z => z));

            using (var context = new ZamówieniaContext())
            {
                var results = query.Invoke(context).ToList();
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMs);
        }

        static void rawsql()
        {
            Console.WriteLine("Raw sql: ");

            using (var ctx = new ZamówieniaContext())
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                var zm = ctx.Orders
                                    .FromSqlInterpolated($"Select * from Orders")
                                    .ToList();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine(elapsedMs);
            }
        }
    }

    public class ZamówieniaContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString);
        }
    }

    public class Order
    {
        [Key]
        public int OrderID { get; set; }
    }
}