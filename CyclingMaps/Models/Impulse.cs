namespace CyclingMaps.Models;

using System;

public readonly record struct Impulse (double Power, TimeSpan Duration)
{
    // TODO: Support speed alternative?
}