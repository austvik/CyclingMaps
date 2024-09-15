namespace CyclingMaps.Models;

using System;

public readonly record struct Point (double Latitude,  // "height"
                                     double Longitude, // "width"
                                     double Elevation)
{
    const double EarthRadius = 6378.137;
    const double Gravity = 9.8067;

    public double DistanceSeconds(Point next, Impulse impulse, Rider rider, Bike bike, Surface surface, Weather weather) {
        double elevationDiff = next.Elevation - Elevation;
        double distMeters = DistanceMeters(this, next);
        if (distMeters == 0 && impulse.Power > 0) 
        {
            return 0;
        }

        double climbPercentage = (elevationDiff / distMeters);
        double distance = Math.Sqrt(distMeters * distMeters + elevationDiff * elevationDiff);
        
        var headwindSpeed = 0.1; // TODO: Calculate driving direction and use weather to calculate headwind

        // Math;: https://www.gribble.org/cycling/power_v_speed.html
        double cda = rider.DragCoefficience * rider.FrontalArea; // OK
        double totalWeight = bike.Weight + rider.Weight;         // OK

        double a = 0.5d * cda * weather.AirDensity;              // OK
        double b = headwindSpeed * cda * weather.AirDensity;     // OK
        double c = Gravity * totalWeight * (Math.Sin(Math.Atan(climbPercentage)) + surface.CoefficientRollingResistance * Math.Cos(Math.Atan(climbPercentage))) + (0.5 * b * headwindSpeed);
        double d = -1 * (1 - (bike.DrivetrainLossPercentage / 100d)) * impulse.Power; // OK

        double groundSpeed = CardanoRoot(a, b, c, d);

        double timeDiff = distance / groundSpeed;
        return timeDiff;
    }

    public static double CardanoRoot(double a, double b, double c, double d)
    {
        // https://brilliant.org/wiki/cardano-method/
        double a2 = a * a;
        double b2 = b * b;
        double q = ((3 * a * c) - b2) / (9 * a2);
        double r = ((9 * a * b * c) - (27 * a2 * d) - (2 * b * b2)) / (54 * a * a2);
        double qr = Math.Sqrt((q * q * q) + (r * r));
        double s = Math.Cbrt(r + qr);
        double t = Math.Cbrt(r - qr);

        return s + t - (b / (3 * a)); // m/s
    }

    public static double DistanceMeters(Point a, Point b)
    {
        var dLat = b.Latitude * Math.PI / 180 - a.Latitude * Math.PI / 180;
        var dLon = b.Longitude * Math.PI / 180 - a.Longitude * Math.PI / 180;
        var angle = Math.Sin(dLat/2) * Math.Sin(dLat/2) +
        Math.Cos(a.Latitude * Math.PI / 180) * Math.Cos(b.Latitude * Math.PI / 180) *
        Math.Sin(dLon/2) * Math.Sin(dLon/2);
        var c = 2 * Math.Atan2(Math.Sqrt(angle), Math.Sqrt(1-angle));
        return EarthRadius * c * 1000;
    }
}