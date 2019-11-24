using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    public class User : FirestoreEntity
    {
        public int Born { get; set; }
        public string DisplayName { get; set; }
        public List<Establishment> Establishments { get; set; }

    }
}
