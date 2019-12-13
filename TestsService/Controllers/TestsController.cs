using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TestsService.Contracts;
using TestsService.DataAccess;
using TestsService.Models;

namespace TestsService.Controllers
{
    [Route("api/[controller]")]
    public class TestsController : Controller
    {
        private readonly MongoTestRepository _tests;
        private readonly IMapper _mapper;
        public TestsController(MongoTestRepository tests, IMapper mapper)
        {
            _tests = tests ?? throw new ArgumentNullException(nameof(tests));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("")]
        [Produces(typeof(IEnumerable<TestDTO>))]
        public IActionResult Get(bool? isApproved = true)
        {
            return Ok(_tests.Where(x => !x.IsDeleted && (isApproved == null || x.IsApproved == isApproved)).Select(x => new TestDTO { Id = x.Id, Name = x.Name, Created = x.Created }).ToList());
        }

        [HttpGet("{id}")]
        [Produces(typeof(TestDetailsDTO))]
        public IActionResult GetById(string id)
        {
            var test = _tests.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (test == null)
                return NotFound($"test:{id} not found");

            return Ok(_mapper.Map<TestDetailsDTO>(test));

        }

        [HttpPost("")]
        public async Task<IActionResult> PostAsync([FromBody] TestIM data)
        {
            if (data == null)
                return BadRequest();

            var test = new Test { Id = Guid.NewGuid().ToString("N"), Created = DateTime.UtcNow, IsDeleted = false, IsApproved = false };
            ReplaceTestValues(test, data);
            await _tests.AddAsync(test);

            return Ok(test.Id);
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PostConfirm(string id)
        {
            var test = _tests.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (test == null)
                return NotFound($"test:{id} not found");

            test.IsApproved = true;
            await _tests.SaveAsync(test);
            return Ok();
        }

        [HttpPost("{id}/questions")]
        public async Task<IActionResult> PostQuestionAsync(string id, [FromBody] QuestionIM data)
        {
            if (data == null)
                return BadRequest();

            var test = _tests.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (test == null)
                return NotFound($"test:{id} not found");
            if (test.IsApproved)
                return StatusCode(409, "test already published. Modification denied");

            test.Questions.Add(MapToQuestion(data));
            await _tests.SaveAsync(test);
            return Ok(test.Id);
        }

        private static Question MapToQuestion(QuestionIM question) => new Question
        {
            Name = question.Name,
            Choices = question.Choices,
            HasMany = question.HasMany,
            IsRequired = question.IsRequired
        };

        private static void ReplaceTestValues(Test test, TestIM data)
        {
            test.Author = data.Author;
            test.HasCreativePart = data.HasCreativePart;
            test.Name = data.Name;
            test.Questions = data.Questions.Select(MapToQuestion).ToList();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(string id, [FromBody] TestIM data)
        {
            if (data == null)
                return BadRequest();

            var test = _tests.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (test == null)
                return NotFound($"test:{id} not found");
            if (test.IsApproved)
                return StatusCode(409, "test already published. Modification denied");

            ReplaceTestValues(test, data);
            await _tests.SaveAsync(test);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var test = _tests.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (test == null)
                return NotFound($"test:{id} not found");

            await _tests.DeleteAsync(test);
            return Ok();
        }


    }
}
