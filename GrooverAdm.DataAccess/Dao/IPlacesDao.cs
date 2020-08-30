using GrooverAdm.DataAccess.Model;
using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Place = GrooverAdm.DataAccess.Model.Place;

namespace GrooverAdm.DataAccess.Dao
{
    public interface IPlacesDao<T> where T : Place
    {
        /// <summary>
        /// Creates a Place
        /// </summary>
        /// <param name="place"></param>
        /// <returns>A place with the updated ID</returns>
        Task<T> CreatePlace(T place);
        /// <summary>
        /// Deletes a Place.
        /// </summary>
        /// <param name="id"></param>
        ///  <param name="user"></param>
        /// <returns>True if place was deleted</returns>
        Task<bool> DeletePlace(string id, string user);
        /// <summary>
        /// Updates a Place
        /// </summary>
        /// <param name="place"></param>
        /// <returns>The updated place</returns>
        Task<T> UpdatePlace(T place);
        /// <summary>
        /// Gets a Place by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The corresponding place</returns>
        Task<T> GetPlace(string id);
        /// <summary>
        /// Gets all places from the Db
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPlaces();
        /// <summary>
        /// Gets <paramref name="quantity"/> places from the db skipping <paramref name="offset"/>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPlaces(int offset, int quantity);
        /// <summary>
        /// Gets <paramref name="quantity"/> places from the db skipping <paramref name="offset"/>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPlaces(int offset, int quantity, string user);
        /// <summary>
        /// Gets all the places surrounding a location (Not going to implement this on firestore)
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="quantity"></param>
        /// <param name="location"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPlaces(int offset, int quantity, Geolocation location, double distance);
        /// <summary>
        /// Gets all the places between those geohashes
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="quantity"></param>
        /// <param name="geohashes"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPlaces(int offset, int quantity, List<Tuple<string,string>> geohashes);
        /// <summary>
        /// Reviews a place
        /// </summary>
        /// <param name="placeId"></param>
        /// <param name="approved"></param>
        /// <param name="reviewComment"></param>
        /// <returns></returns>
        Task<T> ReviewPlace(string placeId, bool approved, string reviewComment);
    }
}
