using NUnit.Framework;

public class PositionTests
{
    [Test]
    public void Constructor_SetsCorrectValues()
    {
        Position pos = new Position(3, 5);

        Assert.AreEqual(3, pos.x);
        Assert.AreEqual(5, pos.y);
    }

    [Test]
    public void Constructor_AllowsNegativeValues()
    {
        Position pos = new Position(-1, -7);

        Assert.AreEqual(-1, pos.x);
        Assert.AreEqual(-7, pos.y);
    }

    [Test]
    public void DefaultStruct_IsZero()
    {
        Position pos = new Position();

        Assert.AreEqual(0, pos.x);
        Assert.AreEqual(0, pos.y);
    }

    [Test]
    public void StructCopy_IsValueType_NotReference()
    {
        Position a = new Position(2, 4);
        Position b = a;

        b.x = 10;

        Assert.AreEqual(2, a.x);
        Assert.AreEqual(10, b.x);
    }
}
