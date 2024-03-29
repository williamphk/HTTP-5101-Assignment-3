﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http;
using HTTP_5101_Assignment_3.Models;
using MySql.Data.MySqlClient;
using System.Web.Http.Cors;

namespace HTTP_5101_Assignment_3.Controllers
{
    public class TeacherDataController : ApiController
    {
        // The database context class which allows us to access our MySQL Database.
        private SchoolDbContext School = new SchoolDbContext();

        //This Controller will access the teachers table of our school database. Non-Deterministic.
        /// <summary>
        /// Returns a list of Teachers in the system matching the search criteria, such as name, salary and hire date 
        /// </summary>
        /// <returns>
        /// A list of Teacher Objects with fields mapped to the database column values (first name, last name).
        /// </returns>
        /// <example>
        /// GET api/TeacherData/ListTeachers -> {Teacher Object 1, Teacher Object 2, Teacher Object 3...}
        /// </example>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{MinSalary}/{MaxSalary}/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(int? MinSalary, int? MaxSalary, DateTime? HireAfter, DateTime? HireBefore, string SearchKey = null)
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
            if (HireAfter.HasValue)
            {
                cmd.CommandText += " AND hiredate >= @HireAfter";
                cmd.Parameters.AddWithValue("@HireAfter", HireAfter);
            }
            if (HireBefore.HasValue)
            {
                cmd.CommandText += " AND hiredate <= @HireBefore";
                cmd.Parameters.AddWithValue("@HireBefore", HireBefore);
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
        [EnableCors(origins: "*", methods: "*", headers: "*")]
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

        /// <summary>
        /// Deletes an Teacher from the connected MySQL Database if the ID of that teacher exists. Does maintain relational integrity. Non-Deterministic.
        /// </summary>
        /// <param name="id">The ID of the teacher.</param>
        /// <example>POST /api/TeacherData/DeleteTeacher/3</example>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        public void DeleteTeacher(int id)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a command (query) for teacher table
            MySqlCommand DeleteTeacherCmd = Conn.CreateCommand();

            //Delete teacher
            DeleteTeacherCmd.CommandText = "Delete from teachers where teacherid=@id";
            DeleteTeacherCmd.Parameters.AddWithValue("@id", id);
            DeleteTeacherCmd.Prepare();
            DeleteTeacherCmd.ExecuteNonQuery();

            //Maintain referential integrity by deleting any courses in the classes pointing to the deleted teacher
            //Establish a command (query) for class table
            MySqlCommand DeleteClassCmd = Conn.CreateCommand();

            //Delete rows in the Classes table that reference the teacher
            DeleteClassCmd.CommandText = "DELETE FROM Classes WHERE teacherid = @id";
            DeleteClassCmd.Parameters.AddWithValue("@id", id);
            DeleteClassCmd.Prepare();
            DeleteClassCmd.ExecuteNonQuery();

            Conn.Close();
        }

        /// <summary>
        /// Adds an Teacher to the MySQL Database.
        /// </summary>
        /// <param name="NewTeacher">An object with fields that map to the columns of the teacher's table. Non-Deterministic.</param>
        /// <returns>
        /// Returns a object of ApiResult
        /// If the update was successful, the ApiResult object will include the created Teacher's TeacherId
        /// If the update was unsuccessful, the ApiResult object will include a Error Message
        /// </returns>
        /// <example>
        /// POST api/TeacherData/AddTeacher 
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        ///	"TeacherFname":"William",
        ///	"TeacherLname":"Poon",
        ///	"TeacherEmployeeNumber":"T567",
        ///	"TeacherHireDate":"2023-11-22",
        ///	"TeacherSalary":"52.22"
        /// }
        /// </example>
        [HttpPost]
        [EnableCors(origins:"*", methods:"*", headers:"*")]
        public ApiResult AddTeacher([FromBody] Teacher NewTeacher)
        {
            ApiResult ApiResult = new ApiResult();
            //Exit method if the input fields are invalid.
            if (!NewTeacher.IsValid())
            {
                ApiResult.Success = false;
                ApiResult.ErrorMsg = "The create was unsuccessful due to incomplete data";
                return ApiResult;
            }

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) "
                            + "values (@TeacherFname,@TeacherLname,@TeacherEmployeeNumber,CURRENT_DATE(),@TeacherSalary);"
                            + "SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@TeacherFname", NewTeacher.TeacherFname);
            cmd.Parameters.AddWithValue("@TeacherLname", NewTeacher.TeacherLname);
            cmd.Parameters.AddWithValue("@TeacherEmployeeNumber", NewTeacher.TeacherEmployeeNumber);
            cmd.Parameters.AddWithValue("@TeacherHireDate", NewTeacher.TeacherHireDate);
            cmd.Parameters.AddWithValue("@TeacherSalary", NewTeacher.TeacherSalary);
            cmd.ExecuteNonQuery();

            // Get the inserted ID
            cmd.CommandText = "SELECT LAST_INSERT_ID();";
            int newTeacherId = Convert.ToInt32(cmd.ExecuteScalar());

            Conn.Close();
            ApiResult.Success = true;
            ApiResult.TeacherId = newTeacherId;
            return ApiResult;
        }

        /// <summary>
        /// Updates an Teacher on the MySQL Database. Non-Deterministic.
        /// </summary>
        /// <param name="UpdatedTeacher">An object with fields that map to the columns of the teacher's table.</param>
        /// <returns>
        /// Returns a object of ApiResult
        /// If the update was successful, the ApiResult object will include the selected Teacher's TeacherId
        /// If the update was unsuccessful, the ApiResult object will include a Error Message
        /// </returns>
        /// <example>
        /// POST api/TeacherData/UpdateTeacher/2 
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        ///	"TeacherFname":"William",
        ///	"TeacherLname":"Poon",
        ///	"TeacherEmployeeNumber":"T567",
        ///	"TeacherHireDate":"2023-11-22",
        ///	"TeacherSalary":"52.22"
        /// }
        /// </example>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        public ApiResult UpdateTeacher(int id, [FromBody]Teacher UpdatedTeacher)
        {
            ApiResult ApiResult = new ApiResult();

            //Exit method if the input fields are invalid.
            if (!UpdatedTeacher.IsValid())
            {
                ApiResult.Success = false;
                ApiResult.ErrorMsg = "The update was unsuccessful due to incomplete data";
                return ApiResult;
            }

            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            MySqlCommand Cmd = Conn.CreateCommand();

            Cmd.CommandText = "UPDATE teachers SET teacherfname = @TeacherFname, "
                            + "teacherlname = @TeacherLname, "
                            + "employeenumber = @TeacherEmployeeNumber, "
                            + "hiredate = @TeacherHireDate, "
                            + "salary = @TeacherSalary "
                            + "WHERE teacherid = @TeacherId";

            Cmd.Parameters.AddWithValue("@TeacherFname", UpdatedTeacher.TeacherFname);
            Cmd.Parameters.AddWithValue("@TeacherLname", UpdatedTeacher.TeacherLname);
            Cmd.Parameters.AddWithValue("@TeacherEmployeeNumber", UpdatedTeacher.TeacherEmployeeNumber);
            Cmd.Parameters.AddWithValue("@TeacherHireDate", UpdatedTeacher.TeacherHireDate);
            Cmd.Parameters.AddWithValue("@TeacherSalary", UpdatedTeacher.TeacherSalary);
            Cmd.Parameters.AddWithValue("@TeacherId", id);
            Cmd.Prepare();
            Cmd.ExecuteNonQuery();

            Conn.Close();

            ApiResult.Success = true;
            ApiResult.TeacherId = id;
            return ApiResult;
        }
    }
}
