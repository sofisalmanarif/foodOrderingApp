using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using foodOrderingApp.middlewares;
using foodOrderingApp.Models;
using foodOrderingApp.repositories.dashboard;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodOrderingApp.controllers.dashboard
{
    [ApiController]
    [Route("/api/dashboards")]
    public class Dashboard:ControllerBase
    {
        private readonly RestaurantDashboard _restaurantDashboard;
        public Dashboard(RestaurantDashboard restaurantDashboard)
        {
             _restaurantDashboard =  restaurantDashboard;
        }

        [HttpGet("restaurant")]
        [Authorize]
        public ActionResult RestaurantDashboard(){
            Guid ownerId = HttpContext.User.GetUserIdFromClaims();
            var data = _restaurantDashboard.GetRestaurntDashboard(ownerId);
            return Ok(new ApiResponse<object>(true,data));

        }


    }
}