using UnityEngine;
using NUnit.Framework;
using UnityEngine;

public class PlayerTests
{
    private GameObject _go;
    private Player _player;
    private SpriteRenderer _spriteRenderer;

    [SetUp]
    public void SetUp()
    {
        // Minden teszt előtt új GameObject + komponensek
        _go = new GameObject("PlayerTestGO");
        _spriteRenderer = _go.AddComponent<SpriteRenderer>();
        _player = _go.AddComponent<Player>();
        // Awake automatikusan lefut az AddComponent után, de ha nagyon akarod:
        // _player.Awake();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_go);
    }

    [Test]
    public void DefaultValues_AreCorrect_WhenPlayerIsCreated()
    {
        // ARRANGE -> SetUp megcsinálta

        // ASSERT
        Assert.AreEqual(0, _player.Score, "Score default nem 0");
        Assert.AreEqual(1.0f, _player.BattleMultiplier, 0.0001f, "BattleMultiplier default nem 1.0");
        Assert.IsTrue(_player.CanReceiveSpecialBuff, "CanReceiveSpecialBuff default nem true");
        Assert.IsFalse(_player.HasBuff, "HasBuff default nem false");
    }

    [Test]
    public void Initialize_SetsBasicFieldsCorrectly()
    {
        // ARRANGE
        int expectedId = 3;
        string expectedName = "Player 3";
        Position startPos = new Position(1, 2);
        Color expectedColor = Color.green;
        Vector2 worldPos = new Vector2(5.0f, -1.5f);

        // ACT
        _player.Initialize(expectedId, expectedName, startPos, expectedColor, worldPos);

        // ASSERT
        Assert.AreEqual(expectedId, _player.ID, "ID nincs jól beállítva");
        Assert.AreEqual(expectedName, _player.PlayerName, "PlayerName nincs jól beállítva");
        Assert.AreEqual(startPos.x, _player.CurrentPosition.x, "CurrentPosition.x nem jó");
        Assert.AreEqual(startPos.y, _player.CurrentPosition.y, "CurrentPosition.y nem jó");
        Assert.AreEqual(expectedColor, _player.Color, "Color property nincs jól beállítva");
        Assert.AreEqual(worldPos, (Vector2)_player.transform.position, "Transform pozíció nincs jól beállítva");
    }

    [Test]
    public void Initialize_SetsSpriteRendererColor()
    {
        // ARRANGE
        Color expectedColor = new Color(0.2f, 0.5f, 0.9f);
        Position startPos = new Position(0, 0);
        Vector2 worldPos = Vector2.zero;

        // Biztonság kedvéért ellenőrizzük, hogy van SpriteRenderer
        Assert.IsNotNull(_spriteRenderer, "SpriteRenderer hiányzik a GameObject-ről!");

        // ACT
        _player.Initialize(1, "TestPlayer", startPos, expectedColor, worldPos);

        // ASSERT
        Assert.AreEqual(expectedColor, _spriteRenderer.color, "SpriteRenderer színe nem egyezik a Player színével");
    }
}

