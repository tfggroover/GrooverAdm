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
            Assert.Throws(typeof(ArgumentException), delegate {
                DistanceService.ValidateGeohash(geo);
            });
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
    }
}
