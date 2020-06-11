using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrooverAdm.Entities  .Application
{
    public class Song 
    {
        public Song(){
            Tags = new List<string>();
        }
        public string Id { get; set; }
        public List<string> Tags { get; set; }
        public dynamic Data { get; set; }
    }
}
