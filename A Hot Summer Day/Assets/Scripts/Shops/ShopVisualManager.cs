using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopVisualManager : MonoBehaviour
{
    private Shop _shop;
    private int _currentIndex = 0;
    private bool _isSpawnVFXPlaying;

    [SerializeField] private ParticleSystem _onSpawnVFX;
    [SerializeField] private List<GameObject> _gameObjects;

    private void Awake()
    {
        _shop = GetComponent<Shop>();
    }

    private void Start()
    {
        foreach (GameObject obj in _gameObjects)
        {
            obj.SetActive(false);
        }

        Shop.OnUpgraded += Shop_OnUpgraded;
    }

    private void Shop_OnUpgraded(object sender, Shop.OnUpgradeEventArgs e)
    {
        TrySpawnNextVisual();
    }

    private void TrySpawnNextVisual()
    {
        if (_gameObjects.Count > _currentIndex && _gameObjects[_currentIndex] != null)
        {
            _gameObjects[_currentIndex].SetActive(true);

            StartCoroutine(SpawnUpgradeVFX());
            RunSpecialInstructions();

            _currentIndex++;
        }
    }

    private IEnumerator SpawnUpgradeVFX()
    {
        if (!_isSpawnVFXPlaying)
        {
            _isSpawnVFXPlaying = true;
            _onSpawnVFX.Play();

            yield return new WaitForSeconds(_onSpawnVFX.main.duration);

            _isSpawnVFXPlaying = false;
        }
    }

    private void RunSpecialInstructions()
    {
        if (_shop is LemonadeStand && _currentIndex == 2)
        {
            _gameObjects[1].SetActive(false);
        }
    }
}
