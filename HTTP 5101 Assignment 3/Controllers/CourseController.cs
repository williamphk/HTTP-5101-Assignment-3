using HTTP_5101_Assignment_3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTTP_5101_Assignment_3.Controllers
{
    public class CourseController : Controller
    {
        // GET: Class
        public ActionResult Index()
        {
            return View();
        }

        //GET : /Class/List
        public ActionResult List(string SearchKey = null)
        {
            CourseDataController controller = new CourseDataController();
            IEnumerable<Course> Classes = controller.ListCourses();

            return View(Classes);
        }

        //GET : /Class/Show/{id}
        public ActionResult Show(int id)
        {
            CourseDataController controller = new CourseDataController();
            Course NewClass = controller.FindClass(id);

            return View(NewClass);
        }
    }
}