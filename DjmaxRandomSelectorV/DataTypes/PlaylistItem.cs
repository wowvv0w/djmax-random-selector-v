using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.DataTypes
{
    public struct PlaylistItem : INullable
    {
        public string Title { get; init; }
        public string Style { get; init; }

        public bool IsNull => Title == null || Style == null;

        public PlaylistItem(string title, string style)
        {
            Title = title;
            Style = style;
        }
    }
}
