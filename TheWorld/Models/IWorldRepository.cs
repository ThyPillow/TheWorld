using System.Collections.Generic;
using System.Threading.Tasks;
using TheWorld.ViewModels;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        Trip GetTripByName(string name);
        Trip GetUserTripByName(string name, string userName);
        IEnumerable<Trip> GetAllTripsByUsername(string name);

        void AddTrip(Trip trip);
        void AddStop(string tripName, Stop newStop, string userName);

        Task<bool> SaveChangesAsync();
    }
}