using System.Net;
using foodOrderingApp.interfaces;
using foodOrderingApp.models;
using foodOrderingApp.Models;
using foodOrderingApp.services;
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

        [Consumes("multipart/form-data")]
        [HttpPost]
        public ActionResult Create([FromForm] CategoryRequistModel newCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Category data", errors = ModelState });
            }
            if (newCategory.Photo == null)
            {
                return BadRequest("Please Select photos");
            }
            string ImageUrl = UploadFiles.Photo(newCategory.Photo);
            Category category = new Category()
            {
                Name = newCategory.Name,
                Photo = ImageUrl,
            };
            string msg = _categoryRepository.Add(category);

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
        [HttpDelete("{id}")]
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