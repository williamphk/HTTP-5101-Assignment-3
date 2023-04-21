using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTTP_5101_Assignment_3.Models.ViewModel
{
    public class ShowTeacher
    {
        //represent the information a webpage needs to show an teacher
        //teacher itself

        public Teacher SelectedTeacher { get; set; }
        //the courses associated withe the teacher
        public IEnumerable<Course> AssociatedCourses { get; set; }

        public IEnumerable<Course> UnassignedCourses { get; set; }
    }
}