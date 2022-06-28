using Catalog.API.Data;
using Catalog.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public interface ICourseRepository
    {
        IEnumerable<Course> GetCourses();
        CourseModel GetCourseById(string courseName);
        Task<Course> UpdateCourse(CourseModel model);
        Task<int> CreateCourse(CourseModel model);
        int DeleteCourse(int Id);


    }
}
