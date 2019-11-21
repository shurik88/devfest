using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurveyService.Contracts;
using SurveyService.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyService.Controllers
{
    /// <summary>
    ///     Контроллер дисциплин
    /// </summary>
    [Route("api/[controller]")]
    public class TestsController : Controller
    {
        private readonly SurveysDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Создание экземпляра класса <see cref="TestsController"/>
        /// </summary>
        /// <param name="context">Контекст бд</param>
        /// <param name="mapper">Маппер</param>
        public TestsController(SurveysDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("")]
        [Produces(typeof(IEnumerable<TestDTO>))]
        public async Task<ActionResult> GetAsync()
        {
            return Ok(await _context.Tests.Where(x => !x.IsDeleted).ProjectTo<TestDTO>(_mapper.ConfigurationProvider).ToListAsync());
        }
    }
}
