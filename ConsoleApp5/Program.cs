using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MagicLibraryApp
{
    class Program
    {
        static string masterConn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";

        static string dbConn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MagicLibraryDB;Integrated Security=True";


        static void CreateDatabase()
        {
            using (var conn = new SqlConnection(masterConn))
            {
                conn.Open();
                string createDb = "IF DB_ID('MagicLibraryDB') IS NULL CREATE DATABASE MagicLibraryDB;";
                new SqlCommand(createDb, conn).ExecuteNonQuery();
            }

            using (var conn = new SqlConnection(dbConn))
            {
                conn.Open();
                string createTable = @"IF OBJECT_ID('Books') IS NULL
            CREATE TABLE Books (
            Id INT IDENTITY PRIMARY KEY,
            Title NVARCHAR(100),
            Author NVARCHAR(100),
            Genre NVARCHAR(50),
            Copies INT
            );";
                new SqlCommand(createTable, conn).ExecuteNonQuery();
            }

            Console.WriteLine("✅ База данных и таблица Books готовы!");
        }
        static void AddBook()
        {
            Console.Write("Название книги: ");
            string title = Console.ReadLine();

            Console.Write("Автор: ");
            string author = Console.ReadLine();

            Console.Write("Жанр: ");
            string genre = Console.ReadLine();

            Console.Write("Количество экземпляров: ");
            int copies = int.Parse(Console.ReadLine());

            using (var conn = new SqlConnection(dbConn))
            {
                conn.Open();
                string sql = "INSERT INTO Books (Title, Author, Genre, Copies) VALUES (@t, @a, @g, @c)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@a", author);
                    cmd.Parameters.AddWithValue("@g", genre);
                    cmd.Parameters.AddWithValue("@c", copies);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("📚 Книга добавлена!");
        }
        static void ShowBooks()
        {
            using (var conn = new SqlConnection(dbConn))
            {
                conn.Open();
                string sql = "SELECT * FROM Books";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nСписок книг:");
                    Console.WriteLine("ID | Название | Автор | Жанр | Кол-во");
                    Console.WriteLine("---------------------------------------");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"],2} | {reader["Title"],-15} | {reader["Author"],-10} | {reader["Genre"],-8} | {reader["Copies"]}");
                    }
                }
            }
        }
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CreateDatabase();

            while (true)
            {
                Console.WriteLine("\n=== МАГИЧЕСКАЯ БИБЛИОТЕКА ===");
                Console.WriteLine("1. Показать все книги");
                Console.WriteLine("2. Добавить книгу");
                Console.WriteLine("3. Выход");
                Console.Write("Выберите пункт: ");

                switch (Console.ReadLine())
                {
                    case "1": ShowBooks(); break;
                    case "2": AddBook(); break;
                    case "3": return;
                    default: Console.WriteLine("Неверный выбор."); break;
                }
            }
        }
    }
}