namespace Dmrsv.Data.DataTypes
{
    public struct PlaylistItem
    {
        public string Title;
        public string Style;

        public PlaylistItem(string title, string style)
        {
            Title = title;
            Style = style;
        }
    }
}
