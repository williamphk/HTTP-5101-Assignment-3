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

        /// <summary>
        /// Returns the courses that is associated with the teacher
        /// </summary>
        /// <param name="teacherid">The Teacher id</param>
        ///  <returns>
        /// A list of Course Objects with teacherid field mapped to the teacherid parameter.
        /// </returns>
        /// <example>
        /// GET api/ClassData/ListCoursesForTeacher -> {Course Object 1, Course Object 2, Course Object 3...}
        /// </example>
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
        /// Returns the courses that is not associated with any teachers
        /// </summary>
        /// <param name="teacherid">The Teacher id</param>
        ///  <returns>
        /// A list of Course Objects with teacherid field is null
        /// </returns>
        /// <example>
        /// GET api/ClassData/ListUnassignedCoursesForTeacher -> {Course Object 1, Course Object 2, Course Object 3...}
        /// </example>
        [HttpGet]
        [Route("api/CourseData/ListUnassignedCoursesForTeacher/")]
        public IEnumerable<Course> ListUnassignedCoursesForTeacher()
        {
            //create MySqlconnection
            MySqlConnection Conn = School.AccessDatabase();

            //open connection
            Conn.Open();

            //MySqlCommand

            MySqlCommand Cmd = Conn.CreateCommand();

            string query = "SELECT classes.* FROM `classes` WHERE teacherid IS NULL";

            Cmd.CommandText = query;

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
        /// <summary>
        /// Set the teacherid column to null on table Class from the MySQL Database through an id. Non-Deterministic.
        /// </summary>
        /// <param name="id">The ID of the teacher.</param>
        /// <example>
        /// POST api/TeacherData/RemoveCourseForTeacher/2 
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        /// "teacherid":"12",
        /// "classid":"2"
        /// }
        /// -> the values of teacherid will be set to null where teacherid = 12 and classid = 2
        /// </example>
        [HttpPost]
        public void RemoveCourseForTeacher(int teacherid, int classid)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand command = Conn.CreateCommand();

            string query = "UPDATE classes SET teacherid = NULL WHERE teacherid = @teacherid AND classid = @classid";
            command.CommandText = query;
            command.Parameters.AddWithValue("@teacherid", teacherid);
            command.Parameters.AddWithValue("@classid", classid);
            command.Prepare();
            command.ExecuteNonQuery();

            Conn.Close();
        }

        /// <summary>
        /// Set the Null teacherid column to selected teacherid on table Class from the MySQL Database through an id. Non-Deterministic.
        /// </summary>
        /// <param name="teacherid">The ID of the teacher.</param>
        /// <example>
        /// POST api/TeacherData/AddCourseForTeacher/2 
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        /// "teacherid":"12",
        /// "classid":"2"
        /// }
        /// -> the values of teacherid will be set to 12 where classid = 2
        /// </example>
        [HttpPost]
        public void AddCourseForTeacher(int teacherid, int classid)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand command = Conn.CreateCommand();

            string query = "UPDATE classes SET teacherid = @teacherid WHERE classid = @classid AND teacherid IS NULL";
            command.CommandText = query;
            command.Parameters.AddWithValue("@teacherid", teacherid);
            command.Parameters.AddWithValue("@classid", classid);
            command.Prepare();
            command.ExecuteNonQuery();

            Conn.Close();
        }
    }
}
