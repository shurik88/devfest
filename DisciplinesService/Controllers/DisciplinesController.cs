using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DisciplinesService.Contracts;
using DisciplinesService.Models.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisciplinesService.Controllers
{
    /// <summary>
    ///     Контроллер дисциплин
    /// </summary>
    [Route("api/[controller]")]
    public class DisciplinesController : Controller
    {
        private readonly DisciplinesDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Создание экземпляра класса <see cref="DisciplinesController"/>
        /// </summary>
        /// <param name="context">Контекст бд</param>
        /// <param name="mapper">Маппер</param>
        public DisciplinesController(DisciplinesDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("")]
        [Produces(typeof(IEnumerable<DisciplineDTO>))]
        public async Task<ActionResult> GetAsync()
        {
            return Ok(await _context.Disciplines.Where(x => !x.IsDeleted).ProjectTo<DisciplineDTO>(_mapper.ConfigurationProvider).ToListAsync());
        }


        [HttpGet("{id}")]
        [Produces(typeof(DisciplineDetailsDTO))]
        public async Task<ActionResult> GetAsync(long id)
        {
            var res = await _context.Disciplines.Where(x => x.Id == id && !x.IsDeleted).ProjectTo<DisciplineDetailsDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            if (res != null)
                return Ok(res);
            return NotFound($"discipline:{id} not found");
        }

        // POST api/values
        [HttpPost("")]
        public async Task<IActionResult> PostAsync([FromBody] DisciplineIM data)
        {
            if (data == null)
                return BadRequest();

            var discipline = new Models.Discipline { IsDeleted = false, Code = data.Code, Name = data.Name, Annotion = data.Annotion };
            _context.Disciplines.Add(discipline);
            await _context.SaveChangesAsync();

            return Ok(discipline.Id);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(long id, [FromBody] DisciplineIM data)
        {
            if (data == null)
                return BadRequest();

            var res = await _context.Disciplines.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if(res == null)
                return NotFound($"discipline:{id} not found");

            res.Code = data.Code;
            res.Annotion = data.Annotion;
            res.Name = data.Name;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var res = await _context.Disciplines.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if(res == null)
                return NotFound($"discipline:{id} not found");
            res.IsDeleted = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
