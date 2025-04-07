using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.Models;
using foodOrderingApp.repositories;
using Microsoft.AspNetCore.Mvc;

namespace foodOrderingApp.controllers
{
    [ApiController]
    [Route("/api/search")]
    public class FiltersController:ControllerBase
    {
        private readonly Filters _filters;
        public FiltersController(Filters filters)
        {
            _filters = filters;
        }

        [HttpGet]
        public ActionResult Search([FromQuery] string query){
            if(string.IsNullOrWhiteSpace(query)){
                throw new NullReferenceException("Querry Can't be empty");
            }

            var res = _filters.Search(query.ToLower());

            return Ok(new ApiResponse<object>(true,res));


        }
    }
}