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
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("/api/trips/{name}/stops")]
    [Authorize]
    public class StopsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<StopsController> _logger;
        private GeoCoordsService _coordsService;

        public StopsController(IWorldRepository repository, ILogger<StopsController> logger, GeoCoordsService coordsService)
        {
            _repository = repository;
            _logger = logger;
            _coordsService = coordsService;
        }

        [HttpGet("")]
        public IActionResult GetTripStopsByName([FromRoute] string name)
        {
            try
            {
                var trip = _repository.GetUserTripByName(name, User.Identity.Name);
                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.ToList()));
            }
            catch (Exception)
            {
                _logger.LogError($"Failed to get stops for trip: {name}");
            }
            return BadRequest("Failed to get stops");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromRoute] string name, [FromBody]StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);
                    var resultCoord = await _coordsService.GetCoordsAsync(newStop.Name);
                    if (!resultCoord.Success)
                    {
                        _logger.LogError(resultCoord.Message);
                    }
                    else
                    {
                        newStop.Latitude = resultCoord.Latitude;
                        newStop.Longitude = resultCoord.Longitude;
                        _repository.AddStop(name, newStop, User.Identity.Name);

                        if (await _repository.SaveChangesAsync())
                        {
                            return Created($"/api/trips/{name}/stops/{newStop.Name}", Mapper.Map<StopViewModel>(newStop));
                        }
                    }
                }
            }
            catch (Exception)
            {
                _logger.LogError($"Failed to post stop for trip: {name}");
            }
            return BadRequest("Failed to post stop");
        }
    }
}
