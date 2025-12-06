using NUnit.Framework;
using UnityEngine;
using TMPro;

public class MenuManagerTest
{
    private GameObject go;
    private MenuManager menu;

    [SetUp]
    public void Setup()
    {
        go = new GameObject("MenuManager");
        menu = go.AddComponent<MenuManager>();

        // statikus változók reset
        MenuManager.PlayerCountSelected = 2;
        MenuManager.BoardSizeSelected = 4;
        MenuManager.BoostTileCount = 2;
        MenuManager.SelectedSaveFile = null;

        PlayerPrefs.DeleteAll();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    // ------------------------------
    // PLAYER COUNT TEST
    // ------------------------------
    [Test]
    public void OnPlayerCountChanged_UpdatesStaticValue()
    {
        menu.SendMessage("OnPlayerCountChanged", 1);
        // value=1 → 1+2 = 3 player

        Assert.AreEqual(3, MenuManager.PlayerCountSelected);
    }

    // ------------------------------
    // BOARD SIZE TEST
    // ------------------------------
    [Test]
    public void OnBoardSizeChanged_UpdatesStaticValue()
    {
        menu.SendMessage("OnBoardSizeChanged", 2);
        // value=2 → 2+3 = 5 board size

        Assert.AreEqual(5, MenuManager.BoardSizeSelected);
    }

    // ------------------------------
    // BOOST COUNT TEST – valid szám
    // ------------------------------
    [Test]
    public void OnBoostCountChanged_ValidNumber_UpdatesValue()
    {
        menu.SendMessage("OnBoostCountChanged", "7");

        Assert.AreEqual(7, MenuManager.BoostTileCount);
    }

    // ------------------------------
    // BOOST COUNT TEST – negatív szám
    // ------------------------------
    [Test]
    public void OnBoostCountChanged_NegativeNumber_SetsZero()
    {
        menu.SendMessage("OnBoostCountChanged", "-5");

        Assert.AreEqual(0, MenuManager.BoostTileCount);
    }

    // ------------------------------
    // CONTINUE CLICKED TEST
    // ------------------------------
    [Test]
    public void OnContinueClicked_SetsShouldLoadGameFlag()
    {
        menu.OnContinueClicked();

        Assert.AreEqual(1, PlayerPrefs.GetInt("ShouldLoadGame"));
    }
}
