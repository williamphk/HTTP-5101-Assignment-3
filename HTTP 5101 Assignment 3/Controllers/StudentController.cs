using HTTP_5101_Assignment_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTTP_5101_Assignment_3.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        //GET : /Student/List
        public ActionResult List(string SearchKey = null)
        {
            StudentDataController controller = new StudentDataController();
            IEnumerable<Student> Students = controller.ListStudents(SearchKey);

            return View(Students);
        }

        //GET : /Student/Show/{id}
        public ActionResult Show(int id)
        {
            StudentDataController controller = new StudentDataController();
            Student NewStudent = controller.FindStudent(id);

            return View(NewStudent);
        }
    }
}