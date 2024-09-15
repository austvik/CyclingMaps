namespace CyclingMaps.Models;

public readonly record struct Bike (double Weight, double RollingResistancePercentage, double DrivetrainLossPercentage)
{
    // TODO: Add air resistance, ...
}