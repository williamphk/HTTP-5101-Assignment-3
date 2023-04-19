using HTTP_5101_Assignment_3.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HTTP_5101_Assignment_3.Controllers
{
    public class CourseDataController : ApiController
    {
        // The database context class which allows us to access our MySQL Database.
        private SchoolDbContext School = new SchoolDbContext();

        //This Controller will access the classs table of our school database. Non-Deterministic.
        /// <summary>
        /// Returns a list of Classes in the system matching the name
        /// </summary>
        /// <returns>
        /// A list of Class Objects with fields mapped to the database column values (first name, last name).
        /// </returns>
        /// <example>
        /// GET api/ClassData/ListClasses -> {Class Object 1, Class Object 2, Class Object 3...}
        /// </example>
        [HttpGet]
        [Route("api/CourseData/ListCourses/{SearchKey?}")]
        public IEnumerable<Course> ListCourses(string SearchKey = null)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "SELECT * FROM Classes "
                            + "WHERE LOWER(classname) LIKE LOWER(@key)";

            cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            //Create an empty list of Classes
            List<Course> Classes = new List<Course> { };

            //Loop Through Each Row the Result Set
            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                Course NewClass = new Course();
                NewClass.CourseId = Convert.ToInt32(ResultSet["classid"]);
                NewClass.CourseName = (string)ResultSet["classname"];

                //Add the Class to the List
                Classes.Add(NewClass);
            }

            //Close the connection between the MySQL Database and the WebServer
            Conn.Close();

            //Return the final list of class
            return Classes;
        }

        [HttpGet]
        [Route("api/CourseData/ListCoursesForTeacher/{teacherid}")]
        public IEnumerable<Course> ListCoursesForTeacher(int teacherid) 
        {
            //create MySqlconnection
            MySqlConnection Conn = School.AccessDatabase();

            //open connection
            Conn.Open();

            //MySqlCommand

            MySqlCommand Cmd = Conn.CreateCommand();

            string query = "SELECT classes.* FROM `classes` WHERE teacherid = @teacherid";

            Cmd.CommandText = query;
            Cmd.Parameters.AddWithValue("@teacherid", teacherid);
            Cmd.Prepare();

            MySqlDataReader ResultSet = Cmd.ExecuteReader();

            List<Course> Courses = new List<Course>();

            while (ResultSet.Read())
            {
                Course NewCourse = new Course();
                NewCourse.CourseId = Convert.ToInt32(ResultSet["classid"]);
                NewCourse.CourseCode = ResultSet["classcode"].ToString();
                NewCourse.CourseName = ResultSet["classname"].ToString();

                Courses.Add(NewCourse);
            }
            return Courses;
        }

        /// <summary>
        /// Finds an Class from the MySQL Database through an id. Non-Deterministic.
        /// </summary>
        /// <param name="id">The Class ID</param>
        /// <returns>Class object containing information about the Class with a matching ID. Empty Class Object if the ID does not match any Classes in the system.</returns>
        /// <example>api/ClassData/FindClass/6 -> {Class Object with ClassId 6}</example>
        /// <example>api/ClassData/FindClass/10 -> {Class Object with ClassId 10}</example>
        [HttpGet]
        [Route("api/ClassData/FindClass/{id}")]
        public Course FindClass(int id)
        {
            Course NewClass = new Course();

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "SELECT * FROM Classes c "
                            + "LEFT JOIN Teachers t ON c.teacherid = t.teacherid "
                            + "WHERE classid = @id";

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            //Gather Result Set of Query into a variable
            MySqlDataReader ResultSet = cmd.ExecuteReader();

            while (ResultSet.Read())
            {
                //Access Column information by the DB column name as an index
                NewClass.CourseId = Convert.ToInt32(ResultSet["classid"]);
                NewClass.CourseCode = (string)ResultSet["classcode"];
                NewClass.TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                NewClass.CourseStartDate = (DateTime)ResultSet["startdate"];
                NewClass.CourseFinishDate = (DateTime)ResultSet["finishdate"];
                NewClass.CourseName = (string)ResultSet["classname"];
                NewClass.TeacherName = (string)ResultSet["teacherfname"] + " " + (string)ResultSet["teacherlname"];
            }
            Conn.Close();

            return NewClass;
        }
    }
}
