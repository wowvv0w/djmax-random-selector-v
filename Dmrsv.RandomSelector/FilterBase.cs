﻿using System.Text.Json.Serialization;

namespace Dmrsv.RandomSelector
{
    public abstract class FilterBase : IFilter
    {
        [JsonIgnore]
        public bool IsUpdated { get; protected set; }

        public FilterBase()
        {
            IsUpdated = true;
        }

        public abstract IEnumerable<Pattern> Filter(IEnumerable<Track> trackList);
    }
}
