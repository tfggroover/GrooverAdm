using GrooverAdm.Entities.Application;
using GrooverAdmSPA.Business.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Test.Services
{
    [TestFixture]
    public class DistanceServiceTest
    {


        [TestCaseSource("valid")]
        public void ValidateGeohash_ValidInput_DoesntThowException(string geo)
        {
            DistanceService.ValidateGeohash(geo);
        }

        [TestCaseSource("invalid")]
        public void ValidateGeohash_InvalidInput_ThowsException(string geo)
        {
            Assert.Throws(typeof(ArgumentException), delegate
            {
                DistanceService.ValidateGeohash(geo);
            });
        }

        [TestCaseSource("queries")]
        public void GeohashQueries_ValidInput_NoException(Geolocation geolocation, double radius)
        {
            DistanceService.GeohashQueries(geolocation, radius);
        }


        private static List<string> valid = new List<string>(){
            "eyeub",
            "aese",
            "oeeab"
            };

        private static List<string> invalid = new List<string>(){
            "",
            "as5dads+1-",
            "asd?!ads"
            };

        private static IEnumerable<object[]> queries()
        {
            yield return
                new object[]
                {
                    new Geolocation(0,0), 500
                };
            yield return
                new object[]
                {
                    new Geolocation(0,0), 5000
                };
            yield return
                new object[]
                {
                    new Geolocation(0,0), 50000
                };
            yield return
                new object[]
                {
                    new Geolocation(50,15), 500
                };
            yield return
                new object[]
                {
                    new Geolocation(50,15), 5000
                };
            yield return
                new object[]
                {
                    new Geolocation(50,15), 50000
                };
            yield return
                new object[]
                {
                    new Geolocation(15,50), 5000
                };
            yield return
                new object[]
                {
                    new Geolocation(80,5), 500
                };
            yield return
                new object[]
                {
                    new Geolocation(80, 15), 5000000
                };


        }
    }
}
