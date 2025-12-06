using NUnit.Framework;
using UnityEngine;

public class PlayerTests
{
    [Test]
    public void Initialize_SetsBasicProperties()
    {
        var go = new GameObject();
        var player = go.AddComponent<Player>();

        var pos = new Position { x = 2, y = 3 };
        var color = Color.red;

        player.Initialize(5, "TestName", pos, color);

        Assert.AreEqual(5, player.ID);
        Assert.AreEqual("TestName", player.PlayerName);

        Assert.AreEqual(2, player.CurrentPosition.x);
        Assert.AreEqual(3, player.CurrentPosition.y);

        Assert.AreEqual(2f, player.transform.position.x);
        Assert.AreEqual(3f, player.transform.position.y);
    }

    [Test]
    public void Initialize_SetsRendererColor_IfRendererExists()
    {
        var go = new GameObject();
        var renderer = go.AddComponent<SpriteRenderer>();
        var player = go.AddComponent<Player>();

        var color = Color.blue;

        player.Initialize(1, "Test", new Position(), color);

        Assert.AreEqual(color, renderer.color);
    }

    [Test]
    public void DefaultScore_IsZero()
    {
        var go = new GameObject();
        var player = go.AddComponent<Player>();

        Assert.AreEqual(0, player.Score);
    }
}
