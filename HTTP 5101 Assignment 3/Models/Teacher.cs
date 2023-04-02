using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTTP_5101_Assignment_3.Models
{
    public class Teacher
    {
        public int TeacherId;
        public string TeacherFname;
        public string TeacherLname;
        public string TeacherEmployeeNumber;
        public DateTime? TeacherHireDate;
        public decimal? TeacherSalary;
        public string ClassName;

        //Server side validation
        public bool IsValid()
        {
            if (TeacherFname == null || TeacherLname == null || TeacherEmployeeNumber == null || TeacherHireDate == null || TeacherSalary == null)
            {
                //Base validation to check if the fields are entered.
                return false;
            }
            return true;
        }

    }
}