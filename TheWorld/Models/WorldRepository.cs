using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddStop(string tripName, Stop newStop, string userName)
        {
            var trip = GetUserTripByName(tripName, userName);
            if (trip != null)
            {
                trip.Stops.Add(newStop);
                _context.Stops.Add(newStop);
            }
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting All trips from the Database");
            return _context.Trips.ToList();
        }

        public IEnumerable<Trip> GetAllTripsByUsername(string name)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .Where(t => t.UserName == name)
                .ToList();
        }

        public Trip GetTripByName(string name)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .FirstOrDefault(x => x.Name == name);
        }

        public Trip GetUserTripByName(string name, string userName)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .FirstOrDefault(x => x.Name == name && x.UserName == userName);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
