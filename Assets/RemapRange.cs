public struct RemapRange
{
    /// <summary>
    /// Returns a new value in a new range of numbers with the current value and old range.
    /// </summary>
    /// <param name="value">The input value</param>
    /// <param name="oldMin">The minimum of the old range of numbers</param>
    /// <param name="oldMax">The maximum of the old range of numbers</param>
    /// <param name="newMin">The minimum of the new range of numbers</param>
    /// <param name="newMax">The maximum of the new range of numbers</param>
    /// <returns></returns>
    public static float Remap(float value, float oldMin, float oldMax, float newMin, float newMax)
    {

        var oldValue = value - oldMin;

        var oldRange = oldMax - oldMin;
        var newRange = newMax - newMin;

        var range = newRange / oldRange;

        var newValue = (oldValue * range) + oldMin;

        return newValue;
    }
}
