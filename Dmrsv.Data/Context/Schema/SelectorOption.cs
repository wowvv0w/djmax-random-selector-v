﻿namespace Dmrsv.Data.Context.Schema
{
    public class SelectorOption
    {
        public string FilterType { get; set; } = nameof(ConditionalFilter);
        public int InputInterval { get; set; } = 30;
        public bool SavesExclusion { get; set; } = false;
        public List<string> OwnedDlcs { get; set; } = new();
    }
}
