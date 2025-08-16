namespace Dmrsv.RandomSelector
{
    public readonly record struct PatternId : IComparable<PatternId>
    {
        public PatternId()
        {
            TrackId = 0;
            Button = ButtonTunes.Four;
            Difficulty = Difficulty.Normal;
        }

        public PatternId(int trackId, ButtonTunes button, Difficulty difficulty)
        {
            if (trackId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(trackId), "The value must be non-negative.");
            }
            if (!Enum.IsDefined(button))
            {
                throw new ArgumentException("The enum value you passed is not defined.", nameof(button));
            }
            if (!Enum.IsDefined(difficulty))
            {
                throw new ArgumentException("The enum value you passed is not defined.", nameof(difficulty));
            }

            TrackId = trackId;
            Button = button;
            Difficulty = difficulty;
        }

        public int TrackId { get; init; }
        public ButtonTunes Button { get; init; }
        public Difficulty Difficulty { get; init; }

        public int CompareTo(PatternId other)
        {
            if (TrackId != other.TrackId)
            {
                return TrackId.CompareTo(other.TrackId);
            }
            if (Button != other.Button)
            {
                return Button.CompareTo(other.Button);
            }
            return Difficulty.CompareTo(other.Difficulty);
        }

        public int ToInt32()
        {
            return 100 * TrackId + 10 * (int)Button + (int)Difficulty;
        }

        public static implicit operator int(PatternId value)
        {
            return value.ToInt32();
        }

        public static explicit operator PatternId(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The value must be non-negative.");
            }

            try
            {
                return new PatternId(value / 100, (ButtonTunes)(value / 10 % 10), (Difficulty)(value % 10));
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("The first digit on the right side of the value " +
                                            "must be one of the following numbers: 0, 1, 2, 3. " +
                                            "The second digit on the right side of the value " +
                                            "must be one of the following numbers: 4, 5, 6, 8.",
                                            nameof(value));
            }
        }
    }
}
