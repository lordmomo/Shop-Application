using DemoWebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebApplication.Controllers;
public class StudentController : Controller
{

    static IList<Student> studentList = new List<Student>{
            new Student() { StudentId = 1, StudentName = "John",StudentEmail="abc@gmail.com", Age = 18 } ,
            new Student() { StudentId = 2, StudentName = "Steve",StudentEmail="des@gmail.com",  Age = 21 } ,
            new Student() { StudentId = 3, StudentName = "Bill", StudentEmail="btgf@gmail.com", Age = 25 } ,
            new Student() { StudentId = 4, StudentName = "Ram" ,StudentEmail="wer@gmail.com", Age = 20 } ,
            new Student() { StudentId = 5, StudentName = "Ron" , StudentEmail="ggd@gmail.com",Age = 31 }
        };

    [Route("student/index")]
    public ActionResult Index()
    {
        return View(studentList);
    }



    [Route("student/list")]
    public string List()
    {
        return "this is list[] of student controller";
    }

    [Route("student/studentId/{studentId}")]
    public ActionResult GetStudentById(int studentId)
    {
        Student? student = studentList.FirstOrDefault(s => s.StudentId == studentId);
        if (student != null)
        {
            return View(student);

        }
        return View(studentList);
    }

    [Route("student/total")]
    public ActionResult GetTotalStudents()
    {
        ViewBag.TotalStudents = studentList.Count;
        return View();
    }
    async Task<int> GetTaskOfResultAsync()
    {
        int hours = 1;
        await Task.Delay(2000);
        return hours;
    }


}
