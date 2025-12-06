using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TilePlayModeTests
{
    private GameObject tileGO;
    private Tile tile;
    private GameObject playerGO;
    private Player player;

    [SetUp]
    public void Setup()
    {
        tileGO = new GameObject("Tile");
        tileGO.AddComponent<SpriteRenderer>();
        tile = tileGO.AddComponent<Tile>();

        tile.Initialize(1, 2);

        playerGO = new GameObject("Player");
        player = playerGO.AddComponent<Player>();
        player.Initialize(1, "TestPlayer", new Position(0, 0), Color.red, Vector2.zero);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(tileGO);
        Object.DestroyImmediate(playerGO);
    }

    [Test]
    public void SetBoostTile_SetsBoostFlag()
    {
        tile.SetBoostTile();

        Assert.IsTrue(tile.IsBoostTile);
    }

    [Test]
    public void ApplyBoost_IncreasesBattleMultiplier()
    {
        tile.SetBoostTile();
        float before = player.BattleMultiplier;

        tile.ApplyBoost(player);

        Assert.AreEqual(before + 0.3f, player.BattleMultiplier, 0.001f);
        Assert.IsTrue(tile.BoostUsed);
    }

    [Test]
    public void SetOwner_SetsOwnerAndOccupiesTile()
    {
        tile.SetOwner(player);

        Assert.AreEqual(player, tile.Owner);
        Assert.IsTrue(tile.IsOccupied());
    }

    [Test]
    public void SetSpecial_SetsSpecialOwner()
    {
        tile.SetSpecial(Color.blue, player);

        Assert.IsTrue(tile.IsSpecialTile);
        Assert.AreEqual(player, tile.SpecialOwner);
    }

    [Test]
    public void RestoreSpecial_WithOwner_SetsBackSpecialOwner()
    {
        tile.RestoreSpecial(player);

        Assert.AreEqual(player, tile.SpecialOwner);
    }

    [Test]
    public void ResetAppearanceToBase_ResetsColor()
    {
        tile.BaseColor = Color.green;
        tile.ResetAppearanceToBase();

        Assert.AreEqual(Color.green, tile.CurrentColor);
    }
}
