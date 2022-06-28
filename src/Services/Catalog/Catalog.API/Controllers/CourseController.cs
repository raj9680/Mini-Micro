using Catalog.API.Data;
using Catalog.API.Models;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CourseController: ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        public CourseController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        }


        // Mains
        [HttpPost]
        public async Task<ActionResult> CreateCourse(CourseModel model)
        {
            var result = await _courseRepository.CreateCourse(model);
            if (result > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpDelete("{Id}")]
        public ActionResult DeleteCourse(int Id)
        {
            _courseRepository.DeleteCourse(Id);
            return Ok("Success");
        }



        [HttpGet("{courseName}")]
        public CourseModel GetCourseById(string courseName)
        {
            var res = _courseRepository.GetCourseById(courseName);
            return res;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Course>> GetCourse()
        {
            var data =  _courseRepository.GetCourses();
            return Ok(data);
        }


        [HttpPut]
        public ActionResult<Course> UpdateCourse([FromBody] CourseModel model)
        {
            var data = _courseRepository.UpdateCourse(model);
            return Ok(data);
        }
    }
}
