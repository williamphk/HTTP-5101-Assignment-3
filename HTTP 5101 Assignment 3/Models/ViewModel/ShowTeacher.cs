using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTTP_5101_Assignment_3.Models.ViewModel
{
    public class ShowTeacher
    {
        //represent the information a webpage needs to show an teacher
        //article itself

        public Teacher SelectedTeacher { get; set; }
        //the courses associated withe the teacher
        public IEnumerable<Course> AssociatedCourses { get; set; }

        //public Author ArticleAuthor { get; set; }

        //public IEnumerable<Comment> ArticleComments {get; set; }
    }
}