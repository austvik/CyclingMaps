namespace CyclingMaps.Models;

public readonly record struct Rider (double Weight, double FrontalArea, double DragCoefficience)
{
    // TODO: Add air resistance, ...
}