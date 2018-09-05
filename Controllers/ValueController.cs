using System.Linq;
using System.Threading.Tasks;
using FinderApp.API.Model;
using FinderApp.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinderApp.API.Controllers
{
    [AllowAnonymous]
    // [Authorize]
    [Route("/api/value")]
    public class ValueController : Controller
    {
        private readonly FinderDbContext context;
        private readonly IUnitOfWork unitofwork;
        public ValueController(FinderDbContext context, IUnitOfWork unitofwork)
        {
            this.unitofwork = unitofwork;
            this.context = context;

        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var getall = await context.Values.ToListAsync();
            return Ok(getall);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await context.Values.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null)
            {
                return NotFound($"the id {id} cannot be found");
            }
            return Ok(result);

        }
        [HttpPost]
        public async Task<IActionResult> CreateValue([FromBody] Value model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var isexist = context.Values.FirstOrDefaultAsync(x => x.Name == model.Name.ToLower());
            if (isexist != null)
            {
                return BadRequest($"The Name with {model.Name} already exist");
            }
            await context.Values.AddAsync(model);
            await unitofwork.CompleteAsync();
            return Ok("successfully created");

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateValue(int id, [FromBody] Value value)
        {
            if (id != value.Id)
            {
                return NotFound($"The id {id} does not match any record");
            }
            if (ModelState.IsValid)
            {
                context.Values.Update(value);
                await unitofwork.CompleteAsync();

            }
            return Ok("Successfully updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromBody] Value value)
        {
            if (id != value.Id)
            {
                return NotFound();

            }
            context.Remove(value);
            await unitofwork.CompleteAsync();
            return Ok("Successfully deleted");

        }
    }
}