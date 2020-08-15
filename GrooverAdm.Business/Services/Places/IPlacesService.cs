using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services.Places
{
    public interface IPlacesService
    {
        /// <summary>
        /// Creates a Place
        /// </summary>
        /// <param name="place"></param>
        /// <returns>A place with the updated ID</returns>
        Task<Place> CreatePlace(Place place);
        /// <summary>
        /// Deletes a Place.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if place was deleted</returns>
        Task<bool> DeletePlace(string id);
        /// <summary>
        /// Updates a Place
        /// </summary>
        /// <param name="place"></param>
        /// <returns>The updated place</returns>
        Task<Place> UpdatePlace(Place place);
        /// <summary>
        /// Gets a Place by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The corresponding place</returns>
        Task<Place> GetPlace(string id);
        /// <summary>
        /// Gets all places from the Db
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Place>> GetPlaces();
        /// <summary>
        /// Gets <paramref name="quantity"/> places from the db skipping <paramref name="offset"/>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<IEnumerable<Place>> GetPlaces(int offset, int quantity);
        /// <summary>
        /// Gets the places surrounding a location checking for the distance to each place at the end of the process
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="quantity"></param>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, Geolocation location, double distance, bool includePlaylist);
        /// <summary>
        /// Gets all the places between those geohashes
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="quantity"></param>
        /// <param name="geohashes"></param>
        /// <returns></returns>
        Task<IEnumerable<Place>> GetPlaces(int offset, int quantity, List<Tuple<string, string>> geohashes);

        Task<Entities.Application.Playlist> UpdatePlaylist(Place place, Dictionary<string, int> tags, Dictionary<string, int> genres);
    }
}
