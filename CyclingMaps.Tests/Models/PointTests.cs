namespace CyclingMaps.Tests.Models;

using CyclingMaps.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PointTests {


    [TestMethod]
    public void TestDistanceMeters() {
        var p1 = new Point(0, 0, 0);
        var p2 = new Point(0.001, 0.002, 0);
        var p3 = new Point(0.001, 0.002, 10);

        Assert.AreEqual(0, Point.DistanceMeters(p1, p1));
        Assert.AreEqual(0, Point.DistanceMeters(p1, p1));
        Assert.AreEqual(249, Point.DistanceMeters(p1, p2), 0.1);
        Assert.AreEqual(249, Point.DistanceMeters(p1, p2), 0.1);
        Assert.AreEqual(0, Point.DistanceMeters(p2, p3), 0.1, "Should not take elevation difference into account");
    }

    [TestMethod]
    public void TestCardanoRoot() {
        var res = Point.CardanoRoot(1, 2, 3, 4);
        Assert.AreEqual(-1.651, res, 0.01);
    }   
}