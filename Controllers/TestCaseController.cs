using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DACS2.Models;
using System;
using DACS2.Data;

namespace DACS2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestCasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestCasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/testcases/baihoc/5
        [HttpGet("baihoc/{baiHocId}")]
        public async Task<ActionResult<IEnumerable<TestCase>>> GetByBaiHoc(int baiHocId)
        {
            var testCases = await _context.TestCase
                .Where(tc => tc.BaiHocId == baiHocId)
                .ToListAsync();

            return Ok(testCases);
        }

        // GET: api/testcases/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestCase>> GetTestCase(int id)
        {
            var testCase = await _context.TestCase.FindAsync(id);
            if (testCase == null)
                return NotFound();

            return testCase;
        }

        // POST: api/testcases
        [HttpPost]
        public async Task<ActionResult<TestCase>> CreateTestCase(TestCase testCase)
        {
            _context.TestCase.Add(testCase);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTestCase), new { id = testCase.Id }, testCase);
        }

        // PUT: api/testcases/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTestCase(int id, TestCase testCase)
        {
            if (id != testCase.Id)
                return BadRequest();

            _context.Entry(testCase).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TestCase.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/testcases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestCase(int id)
        {
            var testCase = await _context.TestCase.FindAsync(id);
            if (testCase == null)
                return NotFound();

            _context.TestCase.Remove(testCase);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
