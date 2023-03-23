using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using HTTP_5101_Assignment_3.Models;
using MySql.Data.MySqlClient;

namespace HTTP_5101_Assignment_3.Controllers
{
    public class TeacherDataController : ApiController
    {
        // The database context class which allows us to access our MySQL Database.
        private SchoolDbContext School = new SchoolDbContext();

        //This Controller will access the teachers table of our school database. Non-Deterministic.
        /// <summary>
        /// Returns a list of Teachers in the system
        /// </summary>
        /// <returns>
        /// A list of Teacher Objects with fields mapped to the database column values (first name, last name).
        /// </returns>
        /// <example>
        /// GET api/TeacherData/ListTeachers -> {Teacher Object 1, Teacher Object 2, Teacher Object 3...}
        /// </example>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{MinSalary}/{MaxSalary}/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(int? MinSalary, int? MaxSalary, string SearchKey = null)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "SELECT * FROM Teachers "
                            + "WHERE (LOWER(teacherfname) LIKE LOWER(@key) "
                            + "OR LOWER(teacherlname) LIKE LOWER(@key) "
                            + "OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@key))";
            if (MinSalary.HasValue)
            {
                cmd.CommandText += " AND salary >= @MinSalary ";
                cmd.Parameters.AddWithValue("@MinSalary", MinSalary);
            }
            if (MaxSalary.HasValue)
            {
                cmd.CommandText += " AND salary <= @MaxSalary";
                cmd.Parameters.AddWithValue("@MaxSalary", MaxSalary);
            }
            cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            //Create an empty list of Teachers
            List<Teacher> Teachers = new List<Teacher> { };

            //Loop Through Each Row the Result Set
            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                Teacher NewTeacher = new Teacher();
                NewTeacher.TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                NewTeacher.TeacherFname = (string)ResultSet["teacherfname"];
                NewTeacher.TeacherLname = (string)ResultSet["teacherlname"];

                //Add the Teacher to the List
                Teachers.Add(NewTeacher);
            }

            //Close the connection between the MySQL Database and the WebServer
            Conn.Close();

            //Return the final list of teacher
            return Teachers;
        }

        /// <summary>
        /// Finds an teacher from the MySQL Database through an id. Non-Deterministic.
        /// </summary>
        /// <param name="id">The Teacher ID</param>
        /// <returns>Teacher object containing information about the teacher with a matching ID. Empty Teacher Object if the ID does not match any teachers in the system.</returns>
        /// <example>api/TeacherData/FindTeacher/6 -> {Teacher Object with TeacherId 6}</example>
        /// <example>api/TeacherData/FindTeacher/10 -> {Teacher Object with TeacherId 10}</example>
        [HttpGet]
        [Route("api/TeacherData/FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher NewTeacher = new Teacher();

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "SELECT * FROM Teachers t "
                            + "LEFT JOIN Classes c ON t.teacherid = c.teacherid "
                            + "WHERE t.teacherid = @id";

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                NewTeacher.TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                NewTeacher.TeacherFname = (string)ResultSet["teacherfname"];
                NewTeacher.TeacherLname = (string)ResultSet["teacherlname"];
                NewTeacher.TeacherEmployeeNumber = (string)ResultSet["employeenumber"];
                NewTeacher.TeacherHireDate = (DateTime)ResultSet["hiredate"];
                NewTeacher.TeacherSalary = (decimal)ResultSet["salary"];

                //If the value in column Classname is DBNull, set it to "null"
                if (ResultSet.IsDBNull(ResultSet.GetOrdinal("classname")))
                {
                    NewTeacher.ClassName = "null";
                } else
                {
                    NewTeacher.ClassName = (string)ResultSet["classname"];
                }
            }
            Conn.Close();

            return NewTeacher;
        }
    }
}
