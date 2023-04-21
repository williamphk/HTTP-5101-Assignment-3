using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HTTP_5101_Assignment_3.Models;
using HTTP_5101_Assignment_3.Models.ViewModel;

namespace HTTP_5101_Assignment_3.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        //GET : /Teacher/List
        public ActionResult List(int? MinSalary, int? MaxSalary, DateTime? HireAfter, DateTime? HireBefore, string SearchKey = null)
        {
            TeacherDataController controller = new TeacherDataController();
            IEnumerable<Teacher> Teachers = controller.ListTeachers(MinSalary, MaxSalary, HireAfter, HireBefore, SearchKey);

            return View(Teachers);
        }

        //GET : /Teacher/Show/{id}
        public ActionResult Show(int id)
        {
            ShowTeacher ViewModel = new ShowTeacher();
            TeacherDataController controller = new TeacherDataController();
            Teacher SelectedTeacher = controller.FindTeacher(id);

            CourseDataController courseController = new CourseDataController();

            ViewModel.SelectedTeacher = SelectedTeacher;
            ViewModel.AssociatedCourses = courseController.ListCoursesForTeacher(id);
            ViewModel.UnassignedCourses = courseController.ListUnassignedCoursesForTeacher();

            return View(ViewModel);
        }

        //GET : /Teacher/DeleteConfirm/{id}
        public ActionResult DeleteConfirm(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher = controller.FindTeacher(id);
            return View(NewTeacher);
        }

        //POST : /Teacher/Delete/{id}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            controller.DeleteTeacher(id);
            return RedirectToAction("List");
        }

        //GET : /Teacher/New
        public ActionResult New()
        {
            return View();
        }

        //POST : /Teacher/Create
        [HttpPost]
        public ActionResult Create(string TeacherFname, string TeacherLname, string TeacherEmployeeNumber, DateTime? TeacherHireDate, decimal? TeacherSalary)
        {
            Teacher NewTeacher = new Teacher();
            NewTeacher.TeacherFname = TeacherFname;
            NewTeacher.TeacherLname = TeacherLname;
            NewTeacher.TeacherEmployeeNumber = TeacherEmployeeNumber;
            NewTeacher.TeacherHireDate = TeacherHireDate;
            NewTeacher.TeacherSalary = TeacherSalary;

            TeacherDataController controller = new TeacherDataController();

            //return RedirectToAction("List");

            ApiResult apiResult = controller.AddTeacher(NewTeacher);
            if (apiResult.Success == true)
            {
                return RedirectToAction("Show/" + apiResult.TeacherId);
            }
            else
            {
                ViewBag.ErrorMsg = apiResult.ErrorMsg;
                return View("New");
            }
        }

        //GET: /Teacher/Update
        public ActionResult Update(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher SelectedTeacher = controller.FindTeacher(id);
            return View(SelectedTeacher);
        }

        //POST : /Teacher/Edit
        [HttpPost]
        public ActionResult Edit(int id, string TeacherFname, string TeacherLname, string TeacherEmployeeNumber, DateTime? TeacherHireDate, decimal? TeacherSalary)
        {
            Teacher UpdatedTeacher = new Teacher();
            UpdatedTeacher.TeacherId = id;
            UpdatedTeacher.TeacherFname = TeacherFname;
            UpdatedTeacher.TeacherLname = TeacherLname;
            UpdatedTeacher.TeacherEmployeeNumber = TeacherEmployeeNumber;
            UpdatedTeacher.TeacherHireDate = TeacherHireDate;
            UpdatedTeacher.TeacherSalary = TeacherSalary;

            TeacherDataController controller = new TeacherDataController();
            ApiResult apiResult = controller.UpdateTeacher(id, UpdatedTeacher);
            if (apiResult.Success == true)
            {
                return RedirectToAction("Show/" + apiResult.TeacherId);
            }
            else
            {
                ViewBag.ErrorMsg = apiResult.ErrorMsg;
                return View("Update", UpdatedTeacher);
            }
        }

        //POST : /Teacher/RemoveCourse
        [HttpPost]
        public ActionResult RemoveCourse(int teacherid, int classid)
        {
            CourseDataController controller = new CourseDataController();
            controller.RemoveCourseForTeacher(teacherid, classid);
            return RedirectToAction("Show/" + teacherid);
        }

        //POST : /Teacher/AddCourse
        [HttpPost]
        public ActionResult AddCourse(int teacherid, int classid)
        {
            CourseDataController controller = new CourseDataController();
            controller.AddCourseForTeacher(teacherid, classid);
            return RedirectToAction("Show/" + teacherid);
        }
    }
}