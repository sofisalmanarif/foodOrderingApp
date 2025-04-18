using System.Net;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace foodOrderingApp.controllers
{
    [ApiController]
    [Route("/api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryReopsitory _categoryRepository;
        public CategoryController(ICategoryReopsitory categoryRepository)
        {
            _categoryRepository = categoryRepository;

        }

        [HttpPost]
        public ActionResult Create([FromBody] Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Category data", errors = ModelState });
            }
            string msg = _categoryRepository.Add(newCategory);

            return Ok(new ApiResponse(true, msg));

        }
        [HttpGet]
        public ActionResult GetAllCategories()
        {

            var categories = _categoryRepository.AllCategories();

            return Ok(new ApiResponse<IEnumerable<Category>>(true, categories));

        }
        [HttpPut]
        public ActionResult Update([FromBody] Category newCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Category data", errors = ModelState });
            }
            string msg = _categoryRepository.Update(newCategory);

            return Ok(new ApiResponse(true, msg));

        }
        [HttpGet("{id}")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new AppException("Please Provide id in params", HttpStatusCode.BadRequest);
            }
            if (!Guid.TryParse(id, out Guid categoryId))
            {
                throw new AppException("Please Provide Valid id", HttpStatusCode.BadRequest);
            }
            string msg = _categoryRepository.Delete(categoryId);

            return Ok(new ApiResponse(true, msg));

        }
    }
}