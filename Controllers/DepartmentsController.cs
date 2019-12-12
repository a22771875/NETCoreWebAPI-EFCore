using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosouniversityContext _context;

        // GET: api/Departments/RowQueryvwDepartmentCourseCount
        [HttpGet("RowQueryvwDepartmentCourseCount")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> GetvwDepartmentCourseCount()
        {
            var vwDepartmentCourseCount = await _context.VwDepartmentCourseCount
                    .FromSqlRaw("SELECT * FROM dbo.VwDepartmentCourseCount")
                    .ToListAsync();
            return vwDepartmentCourseCount;
        }
        public DepartmentsController(ContosouniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            //return await _context.Department.ToListAsync();
            return await _context.Department.Where(x => x.IsDeleted == false).ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            var count = await _context.Database.ExecuteSqlInterpolatedAsync($"EXECUTE Department_Update {department.DepartmentId},{department.Name},{department.Budget},{department.StartDate},{department.InstructorId},{department.RowVersion}");

            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            //_context.Department.Add(department);
            //await _context.SaveChangesAsync();

            department.DepartmentId = (await _context.Department.FromSqlInterpolated(
                                               $"EXEC [dbo].[Department_Insert] {department.Name}, {department.Budget}, {department.StartDate}, {department.InstructorId}; ")
                                           .Select(x => x.DepartmentId)
                                           .ToListAsync()).Single();

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            //_context.Department.Remove(department);
            //department.IsDeleted = true;
            //_context.Update(department);
            //await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC [dbo].[Department_Delete] {department.DepartmentId}, {department.RowVersion}");

            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
