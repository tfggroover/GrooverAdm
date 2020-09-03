using GrooverAdm.Entities.Application;
using GrooverAdmSPA.Business.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Test.Services
{

    [TestFixture]

    public class GeopointServiceTest
    {

        [TestCaseSource("ValidGeopoints")]
        public void ValidateGeopoint_ValidInput_NoException(Geolocation location)
        {
            GeopointService.ValidateGeopoint(location);
        }

        [TestCaseSource("InvalidGeopoints")]
        public void ValidateGeopoint_InvalidInput_Exceptions(Geolocation location)
        {
            Assert.Throws(typeof(ArgumentException), delegate { GeopointService.ValidateGeopoint(location); });
            

        }

        private static List<Geolocation> ValidGeopoints = new List<Geolocation>(){
            new Geolocation(0, 0),
            new Geolocation(90, 0),
            new Geolocation(-90, 0),
            new Geolocation(0, 180),
            new Geolocation(0, -180),
            new Geolocation(90, 180),
            new Geolocation(-90, -180),

            };

        private static List<Geolocation> InvalidGeopoints = new List<Geolocation>(){
            new Geolocation(-91, 0),
            new Geolocation(91, 0),
            new Geolocation(-200, 0),
            new Geolocation(200, 0),
            new Geolocation(5, 181),
            new Geolocation(100, -181),
            new Geolocation(200, 900),

            };
    }
}
