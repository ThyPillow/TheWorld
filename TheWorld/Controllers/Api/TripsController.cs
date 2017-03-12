using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("api/trips")]
    [Authorize]
    public class TripsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<TripsController> _logger;

        public TripsController(IWorldRepository repository, ILogger<TripsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult GetAllTrips()
        {
            try
            {
                var result = _repository.GetAllTripsByUsername(User.Identity.Name);
                return Ok(Mapper.Map<IEnumerable<TripViewModel>>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get All Trips: {ex.Message}");
                return BadRequest("Error occurred");
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateNewTrip([FromBody]TripViewModel trip)
        {
            if (ModelState.IsValid)
            {
                var newTrip = Mapper.Map<Trip>(trip);
                newTrip.UserName = User.Identity.Name;
                _repository.AddTrip(newTrip);

                if (await _repository.SaveChangesAsync())
                {
                    return Created($"api/trips/{trip.Name}", Mapper.Map<TripViewModel>(newTrip));
                }

            }
            return BadRequest("Failed to save the trip");
        }
    }
}
