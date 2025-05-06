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
        private readonly AdminDashboard _adminDashboard;
        public Dashboard(RestaurantDashboard restaurantDashboard ,AdminDashboard adminDashboard)
        {
             _restaurantDashboard =  restaurantDashboard;
             _adminDashboard =adminDashboard;
        }

        [HttpGet("restaurant")]
        [Authorize(Roles ="Owner")]
        public ActionResult RestaurantDashboard([FromQuery] foodOrderingApp.repositories.dashboard.SatsOf statsOf)
        {
           
            Guid ownerId = HttpContext.User.GetUserIdFromClaims();
            var data = _restaurantDashboard.GetRestaurntDashboard(ownerId,statsOf);
            return Ok(new ApiResponse<object>(true,data));

        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public ActionResult AdminDashboard([FromQuery] foodOrderingApp.repositories.dashboard.SatsOf statsOf)
        {

            Guid adminId = HttpContext.User.GetUserIdFromClaims();
            var data = _adminDashboard.GetAdminDashboard(statsOf);
            return Ok(new ApiResponse<object>(true, data));

        }


    }
}