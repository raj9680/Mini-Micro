using Catalog.API.Data;
using Catalog.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DBContext _context;
        public CourseRepository(DBContext context)
        {
            _context = context;
        }



        // Mains
        public async Task<int> CreateCourse(CourseModel model)
        {
            var data = new Course()
            {
                CourseName = model.CourseName,
                CourseDescription = model.CourseDescription,
                Price = model.Price
            };
            await _context.Courses.AddAsync(data);
            await _context.SaveChangesAsync();
            return data.Id;
        }



        public int DeleteCourse(int id)
        {
            var Id = new Course { Id = id };
            _context.Courses.Remove(Id);
            _context.SaveChanges();
            return Id.Id;
        }



        public CourseModel GetCourseById(string courseName)
        {
            var res = _context.Courses.Where(x => x.CourseName == courseName).FirstOrDefault();
            
            return new CourseModel()
            {
                Id = res.Id,
                CourseName = res.CourseName,
                CourseDescription = res.CourseDescription,
                Price = res.Price
            };    
        }



        public IEnumerable<Course> GetCourses()
        {
            var result = _context.Courses.ToList();
            return result;
        }



        public async Task<Course> UpdateCourse(CourseModel model)
        {
            var res = _context.Courses.FirstOrDefault(x => x.Id == model.Id);
            if(res != null)
            {
                res.CourseName = model.CourseName;
                res.CourseDescription = model.CourseDescription;
                res.Price = model.Price;
            }

            _context.Courses.Update(res);
            _context.SaveChangesAsync();
            return res;
        }
    }
}
