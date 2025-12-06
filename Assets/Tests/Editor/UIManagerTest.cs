using NUnit.Framework;
using TMPro;

public class UIManager_EditMode_Tests
{
    private UIManager uiManager;

    [SetUp]
    public void SetUp()
    {
        uiManager = new UIManager();
    }

    [Test]
    public void DisplayWinScore_DoesNotCrash_WhenTextIsNull()
    {
        Assert.DoesNotThrow(() => uiManager.DisplayWinScore(10));
    }

    [Test]
    public void UpdateRound_DoesNotCrash_WhenTextIsNull()
    {
        Assert.DoesNotThrow(() => uiManager.UpdateRound(5));
    }

    [Test]
    public void UpdateScores_DoesNotCrash_WithNullArray()
    {
        Assert.DoesNotThrow(() => uiManager.UpdateScores(null));
    }

    [Test]
    public void UpdateActivePlayer_DoesNotCrash_WithNullPlayer()
    {
        Assert.DoesNotThrow(() => uiManager.UpdateActivePlayer(null));
    }

    [Test]
    public void SetWinner_DoesNotCrash_WithNullWinnerText()
    {
        Player dummy = new Player();
        dummy.PlayerName = "Test";

        Assert.DoesNotThrow(() => uiManager.SetWinner(dummy));
    }
}
