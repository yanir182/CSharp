using System;
using System.Data;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApp11
{
    public partial class Form1 : Form
    {
        static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=StudentDB;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            CreateDatabase();
            CreateTables();
            SeedData();
        }

        static void CreateDatabase()
        {
            string masterConn = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
            string createDbSql = "IF DB_ID('StudentDB') IS NULL CREATE DATABASE StudentDB;";
            using (var conn = new SqlConnection(masterConn))
            using (var cmd = new SqlCommand(createDbSql, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("База данных StudentDB проверена/создана.");
        }

        static void CreateTables()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                conn.Open();
                cmd.CommandText = @"
IF OBJECT_ID('Students') IS NULL
 CREATE TABLE Students (
 Id INT PRIMARY KEY IDENTITY(1,1),
 FirstName NVARCHAR(50),
 LastName NVARCHAR(50),
 Age INT,
 GroupName NVARCHAR(20)
 );

 IF OBJECT_ID('Courses') IS NULL
 CREATE TABLE Courses (
 Id INT PRIMARY KEY IDENTITY(1,1),
 CourseName NVARCHAR(50)
 );

 IF OBJECT_ID('StudentCourses') IS NULL
 CREATE TABLE StudentCourses (
 StudentId INT REFERENCES Students(Id),
 CourseId INT REFERENCES Courses(Id),
 Grade INT
 );";
                cmd.ExecuteNonQuery();
                Console.WriteLine("Таблицы проверены/созданы.");
            }
        }

        static void SeedData()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                conn.Open();
                cmd.CommandText = @"
 IF NOT EXISTS (SELECT * FROM Students)
 BEGIN
 INSERT INTO Students (FirstName, LastName, Age, GroupName) VALUES
 ('Иван', 'Иванов', 20, 'A1'),
 ('Мария', 'Петрова', 19, 'A2'),
 ('Пётр', 'Сидоров', 21, 'A1');

 INSERT INTO Courses (CourseName) VALUES
 ('Программирование'), ('Базы данных'), ('Математика');

 INSERT INTO StudentCourses (StudentId, CourseId, Grade) VALUES
 (1,1,90), (1,2,88), (2,1,75), (3,3,95);
 END";
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Тестовые данные добавлены (если их не было).");
        }

        static void ShowStudents()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Students ORDER BY Id", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nСписок студентов:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]}. {reader["FirstName"]} {reader["LastName"]} — {reader["Age"]} лет, группа {reader["GroupName"]}");
                    }
                }
            }
        }

        static void AddStudent()
        {
            Console.Write("Имя: "); var f = Console.ReadLine();
            Console.Write("Фамилия: "); var l = Console.ReadLine();
            Console.Write("Возраст: "); var a = int.Parse(Console.ReadLine());
            Console.Write("Группа: "); var g = Console.ReadLine();
            string sql = "INSERT INTO Students (FirstName, LastName, Age, GroupName) VALUES (@f, @l, @a, @g)";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@f", f);
                cmd.Parameters.AddWithValue("@l", l);
                cmd.Parameters.AddWithValue("@a", a);
                cmd.Parameters.AddWithValue("@g", g);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Студент добавлен.");
        }

        static void UpdateAge()
        {
            Console.Write("ID студента: "); int id = int.Parse(Console.ReadLine());
            Console.Write("Новый возраст: "); int age = int.Parse(Console.ReadLine());
            string sql = "UPDATE Students SET Age=@a WHERE Id=@id";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@a", age);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Возраст обновлён.");
        }

        static void DeleteStudent()
        {
            Console.Write("ID студента для удаления: ");
            int id = int.Parse(Console.ReadLine());
            string sql = "DELETE FROM Students WHERE Id=@id";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Студент удалён.");
        }

        static void ShowStudentCourses()
        {
            Console.Write("Введите ID студента: ");
            int id = int.Parse(Console.ReadLine());
            string sql = @"
 SELECT s.FirstName, s.LastName, c.CourseName, sc.Grade
 FROM StudentCourses sc
 JOIN Students s ON sc.StudentId = s.Id
 JOIN Courses c ON sc.CourseId = c.Id
 WHERE s.Id=@id";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nКурсы студента:");
                    while (reader.Read())
                        Console.WriteLine($"{reader["FirstName"]} {reader["LastName"]} — {reader["CourseName"]} ({reader["Grade"]})");
                }
            }
        }

        static void CountByGroup()
        {
            string sql = "SELECT GroupName, COUNT(*) AS Cnt FROM Students GROUP BY GroupName";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    Console.WriteLine("\nКоличество студентов по группам:");
                    while (r.Read())
                        Console.WriteLine($"{r["GroupName"]}: {r["Cnt"]}");
                }
            }
        }

       
    }
}