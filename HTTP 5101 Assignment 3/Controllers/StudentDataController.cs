using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HTTP_5101_Assignment_3.Models;
using MySql.Data.MySqlClient;

namespace HTTP_5101_Assignment_3.Controllers
{
    public class StudentDataController : ApiController
    {
        // The database context class which allows us to access our MySQL Database.
        private SchoolDbContext School = new SchoolDbContext();

        //This Controller will access the students table of our school database. Non-Deterministic.
        /// <summary>
        /// Returns a list of Students in the system
        /// </summary>
        /// <returns>
        /// A list of Student Objects with fields mapped to the database column values (first name, last name).
        /// </returns>
        /// <example>
        /// GET api/StudentData/ListStudents -> {Student Object 1, Student Object 2, Student Object 3...}
        /// </example>
        [HttpGet]
        [Route("api/StudentData/ListStudents/{SearchKey?}")]
        public IEnumerable<Student> ListStudents(string SearchKey = null)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "SELECT * FROM Students "
                            + "WHERE LOWER(studentfname) LIKE LOWER(@key) "
                            + "OR LOWER(studentlname) LIKE LOWER(@key) "
                            + "OR LOWER(CONCAT(studentfname, ' ', studentlname)) LIKE LOWER(@key)";

            cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            //Create an empty list of Teachers
            List<Student> Students = new List<Student> { };

            //Loop Through Each Row the Result Set
            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                Student NewStudent = new Student();
                NewStudent.StudentId = Convert.ToInt32(ResultSet["studentid"]);
                NewStudent.StudentFname = (string)ResultSet["studentfname"];
                NewStudent.StudentLname = (string)ResultSet["studentlname"];

                //Add the Teacher to the List
                Students.Add(NewStudent);
            }

            //Close the connection between the MySQL Database and the WebServer
            Conn.Close();

            //Return the final list of teacher
            return Students;
        }
        /// <summary>
        /// Finds an Student from the MySQL Database through an id. Non-Deterministic.
        /// </summary>
        /// <param name="id">The Student ID</param>
        /// <returns>Student object containing information about the Student with a matching ID. Empty Student Object if the ID does not match any Students in the system.</returns>
        /// <example>api/StudentData/FindStudent/6 -> {Student Object with StudentId 6}</example>
        /// <example>api/StudentData/FindStudent/10 -> {Student Object with StudentId 10}</example>
        [HttpGet]
        [Route("api/StudentData/FindStudent/{id}")]
        public Student FindStudent(int id)
        {
            Student NewStudent = new Student();

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "SELECT * FROM Students s "
                            + "LEFT JOIN StudentsXClasses sc ON s.studentid = sc.studentid "
                            + "LEFT JOIN Classes c ON sc.classid = c.classid "
                            + "WHERE s.studentid = @id";

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                NewStudent.StudentId = Convert.ToInt32(ResultSet["studentid"]);
                NewStudent.StudentFname = (string)ResultSet["studentfname"];
                NewStudent.StudentLname = (string)ResultSet["studentlname"];
                NewStudent.StudentNumber = (string)ResultSet["studentnumber"];
                NewStudent.StudentEnrolDate = (DateTime)ResultSet["enroldate"];
                //If the value in column Classname is DBNull, set it to "null"
                if (ResultSet.IsDBNull(ResultSet.GetOrdinal("classname")))
                {
                    NewStudent.ClassName = "null";
                }
                else
                {
                    NewStudent.ClassName = (string)ResultSet["classname"];
                }
            }
            Conn.Close();

            return NewStudent;
        }
    }
}
