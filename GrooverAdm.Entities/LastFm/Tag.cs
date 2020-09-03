using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GrooverAdm.Entities.LastFm
{
    public class Tag
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class TagsExtended
    {
        public List<Tag> Tag { get; set; }
    }

    public class TopTagsResponse
    {
        public TagsExtended Toptags { get; set; }
    }

    public class TagComparer : IComparer<Tag>
    {
        /// <summary>
        /// Descending order by default
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare([AllowNull] Tag x, [AllowNull] Tag y)
        {
            return y.Count.CompareTo(x.Count);
        }
    }
}
