using System;
using System.Collections.Generic;
using System.Text;

namespace GrooverAdm.Entities.Spotify
{
    public class PagingWrapper<T>
    {
        public PagingWrapper()
        {
            Items = new List<T>();
        }
        public List<T> Items { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }

    }
}
