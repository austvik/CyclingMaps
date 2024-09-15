namespace CyclingMaps.Models;

using System.Collections.Generic;
using System.Linq;

public readonly record struct Track(string Name,
                                    string Type,
                                    List<Point> Positions)
{
    public double MinLatitude()
    {
        return Positions.Min(p => p.Latitude);
    }

    public double MaxLatitude()
    {
        return Positions.Max(p => p.Latitude);
    }

    public double MinLongitude()
    {
        return Positions.Min(p => p.Longitude);
    }

    public double MaxLongitude()
    {
        return Positions.Max(p => p.Longitude);
    }

    public double MinElevation()
    {
        return Positions.Min(p => p.Elevation);
    }

    public double MaxElevation()
    {
        return Positions.Max(p => p.Elevation);
    }

    public double Width()
    {
        return MaxLongitude() - MinLongitude();
    }

    public double Height()
    {
        return MaxLatitude() - MinLatitude();
    }
}