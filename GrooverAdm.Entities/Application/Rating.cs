using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities.Application
{
    public class Rating : IApplicationEntity
    {
        public string Id { get; set; }
        public double Value { get; set; }
        public bool New { get; set; }
        public double? OldValue { get; set; }
    }
}
