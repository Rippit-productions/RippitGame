using System;

public enum TrickDirection
{
    Up,
    Down,
    Left,
    Right
}

public static class TrickDirectionExtensions
{
    public static string ToInputLabel(this TrickDirection direction)
    {
        switch (direction)
        {
            case TrickDirection.Up:
                return "^";
            case TrickDirection.Down:
                return "v";
            case TrickDirection.Left:
                return "<";
            case TrickDirection.Right:
                return ">";
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}
