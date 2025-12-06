using NUnit.Framework;
using UnityEngine;

public class TileTests
{
    private GameObject tileGO;
    private Tile tile;

    [SetUp]
    public void Setup()
    {
        tileGO = new GameObject("TileTestGO");
        tileGO.AddComponent<SpriteRenderer>();
        tile = tileGO.AddComponent<Tile>();

    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(tileGO);
    }

    [Test]
    public void Initialize_SetsGridPos_AndName()
    {
        tile.Initialize(4, 7);

        Assert.AreEqual(4, tile.GridPos.x);
        Assert.AreEqual(7, tile.GridPos.y);
        Assert.AreEqual("Tile (4,7)", tileGO.name);
    }

    [Test]
    public void BaseColor_Setter_Updates_Renderer_And_CurrentColor()
    {
        Color newColor = Color.green;
        tile.BaseColor = newColor;

        var renderer = tileGO.GetComponent<SpriteRenderer>();

        Assert.AreEqual(newColor, renderer.color);
        Assert.AreEqual(newColor, tile.CurrentColor);
    }

    [Test]
    public void HighlightUnderPlayer_Creates_FadedColor()
    {
        tile.BaseColor = Color.red;
        Color playerColor = Color.blue;

        tile.HighlightUnderPlayer(playerColor);

        Color expected = new Color(0.5f, 0f, 0.5f, 1f);

        Assert.AreEqual(expected, tile.CurrentColor);
    }

    [Test]
    public void SetPlayerColor_Sets_FadedAlpha()
    {
        Color playerColor = new Color(1f, 1f, 0f, 1f);

        tile.SetPlayerColor(playerColor);

        Color expected = new Color(1f, 1f, 0f, 0.5f);

        Assert.AreEqual(expected, tile.CurrentColor);
    }


    [Test]
    public void SetOwner_SetsOwner_AndColor()
    {
        var playerGO = new GameObject("PlayerGO");
        var player = playerGO.AddComponent<Player>();

        player.Initialize(1, "P", new Position { x = 0, y = 0 }, Color.cyan);

        tile.SetOwner(player);

        Assert.AreEqual(player, tile.Owner);
        Assert.AreEqual(new Color(0f, 1f, 1f, 0.5f), tile.CurrentColor);

        Object.DestroyImmediate(playerGO);
    }

    [Test]
    public void IsOccupied_ReturnsTrue_WhenOwnerSet()
    {
        Assert.IsFalse(tile.IsOccupied());

        var playerGO = new GameObject("PlayerGO2");
        var player = playerGO.AddComponent<Player>();
        player.Initialize(2, "Q", new Position { x = 0, y = 0 }, Color.black);

        tile.SetOwner(player);
        Assert.IsTrue(tile.IsOccupied());

        Object.DestroyImmediate(playerGO);
    }

    [Test]
    public void ResetAppearanceToBase_RestoresColor()
    {
        tile.BaseColor = Color.magenta;
        tile.SetPlayerColor(Color.yellow);

        tile.ResetAppearanceToBase();

        Assert.AreEqual(Color.magenta, tile.CurrentColor);
    }

    [Test]
    public void SetSpecial_SetsSpecialOwner_AndBaseColor()
    {
        var playerGO = new GameObject("OwnerGO");
        var player = playerGO.AddComponent<Player>();
        player.Initialize(3, "Owner", new Position { x = 0, y = 0 }, Color.green);

        tile.SetSpecial(Color.green, player);

        Assert.AreEqual(player, tile.SpecialOwner);
        Assert.AreEqual(Color.green, tile.BaseColor);

        Object.DestroyImmediate(playerGO);
    }

    [Test]
    public void RestoreSpecial_WithCapturedBy_SetsOwner()
    {
        var ownerGO = new GameObject("OwnerGO2");
        var owner = ownerGO.AddComponent<Player>();
        owner.Initialize(4, "Owner2", new Position { x = 0, y = 0 }, Color.red);

        var captGO = new GameObject("CaptGO");
        var capturedBy = captGO.AddComponent<Player>();
        capturedBy.Initialize(5, "Capt", new Position { x = 0, y = 0 }, Color.blue);

        tile.RestoreSpecial(owner, capturedBy);

        Assert.AreEqual(owner, tile.SpecialOwner);
        Assert.AreEqual(capturedBy, tile.SpecialCapturedBy);
        Assert.AreEqual(new Color(0f, 0f, 1f, 0.5f), tile.CurrentColor);

        Object.DestroyImmediate(ownerGO);
        Object.DestroyImmediate(captGO);
    }

    [Test]
    public void RestoreSpecial_WithOwnerOnly_UpdatesBaseColor()
    {
        var ownerGO = new GameObject("OwnerGO3");
        var owner = ownerGO.AddComponent<Player>();
        owner.Initialize(6, "Owner3", new Position { x = 0, y = 0 }, Color.red);

        tile.RestoreSpecial(owner);

        Assert.AreEqual(owner, tile.SpecialOwner);
        Assert.AreEqual(Color.red, tile.BaseColor);

        Object.DestroyImmediate(ownerGO);
    }

    [Test]
    public void RestoreSpecial_NoOwner_ResetsAppearance()
    {
        tile.BaseColor = Color.gray;
        tile.SetPlayerColor(Color.yellow);

        tile.RestoreSpecial(null);

        Assert.AreEqual(Color.gray, tile.CurrentColor);
    }
}
