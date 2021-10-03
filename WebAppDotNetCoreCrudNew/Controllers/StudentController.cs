using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppDotNetCoreCrudNew.Models;

namespace WebAppDotNetCoreCrudNew.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentContext _Db;

        public StudentController(StudentContext Db)
        {
            _Db = Db;
        }
        public IActionResult StudentList()
        {
            try
            {
                // for single var stdList = _Db.tbl_Student.ToList();

                var stdList = from a in _Db.tbl_Student
                              join b in _Db.tbl_Departments
                              on a.DepID equals b.ID
                              into Dep
                              from b in Dep.DefaultIfEmpty()

                              select new Student
                              {
                                  ID = a.ID,
                                  Name = a.Name,
                                  FName = a.FName,
                                  Mobile = a.Mobile,
                                  Email = a.Email,
                                  Description = a.Description,
                                  DepID = a.DepID,

                                  Department= b==null?"":b.Department
                              };
                return View(stdList);
            }
            catch(Exception ex)
            {

            }

            return View();
        }

        public IActionResult Create(Student obj)
        {
            loadDDL();
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(Student obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (obj.ID == 0)
                    {
                        _Db.tbl_Student.Add(obj);
                        await _Db.SaveChangesAsync();
                    }
                    else
                    {
                        _Db.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        await _Db.SaveChangesAsync();

                    }
                    
                    return RedirectToAction("StudentList");
                }

                return View(obj);

            }
            catch (Exception e)
            {
                return RedirectToAction("StudentList");

            }
           
        }

        public async Task<IActionResult> DeleteStd(int id)
        {
            try
            {
                var std = await _Db.tbl_Student.FindAsync(id);
                if(std!=null){

                    _Db.tbl_Student.Remove(std);
                    await _Db.SaveChangesAsync();

                }

                return RedirectToAction("StudentList");

            }
            catch (Exception ex)
            {

                return RedirectToAction("StudentList");
            }
        }

        private void loadDDL()
        {
            try
            {
                List<Departments> depList = new List<Departments>();
                depList = _Db.tbl_Departments.ToList();
                depList.Insert(0, new Departments { ID = 0, Department = "Please Select" });
                ViewBag.DepList = depList;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
