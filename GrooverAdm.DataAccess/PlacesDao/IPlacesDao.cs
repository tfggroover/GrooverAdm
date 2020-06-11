using GrooverAdm.Entities.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.DataAccess.PlacesDao
{
    interface IPlacesDao
    {
        Place CreatePlace(Place place);
        bool DeletePlace(string id);
        Place UpdatePlace(Place place);
        Place GetPlace(string id);
        IEnumerable<Place> GetPlaces();
        IEnumerable<Place> GetPlaces(int offset, int quantity);
        IEnumerable<Place> GetPlaces(int offset, int quantity, Geolocation location, double distance);
        IEnumerable<Place> GetPlaces(int offset, int quantity, List<string> geohashes);
    }
}
