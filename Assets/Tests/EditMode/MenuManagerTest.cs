using NUnit.Framework;
using UnityEngine;
using TMPro;
using System.Reflection;

namespace Tests.EditMode
{
    public class MenuManagerTests
    {
        private GameObject _menuGO;
        private MenuManager _menuManager;

        [SetUp]
        public void Setup()
        {
            _menuGO = new GameObject("MenuManager_Test");
            _menuManager = _menuGO.AddComponent<MenuManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_menuGO);
            PlayerPrefs.DeleteAll();
        }

        [Test]
        public void OnPlayerCountChanged_UpdatesStaticValue()
        {
            MethodInfo method = _menuManager.GetType()
                .GetMethod("OnPlayerCountChanged", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(_menuManager, new object[] { 1 });

            Assert.AreEqual(3, MenuManager.PlayerCountSelected);
        }

        [Test]
        public void OnContinueClicked_SetsPlayerPrefsFlag()
        {
            _menuManager.OnContinueClicked();
            Assert.AreEqual(1, PlayerPrefs.GetInt("ShouldLoadGame"));
        }
    }
}
