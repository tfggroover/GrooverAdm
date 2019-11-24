using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    public class Rating : FirestoreEntity
    {
        public double Value { get; set; }
        public User User { get; set; }
    }
}
