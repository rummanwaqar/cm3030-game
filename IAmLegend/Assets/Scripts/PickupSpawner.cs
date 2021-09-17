using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    #region Variables
    [Header("Weapons Pickup Spawner")]
    [SerializeField] private GameObject[] weaponPickups;
    [SerializeField] private int minWeaponsEach;
    [SerializeField] private int maxWeaponsEach;

    [Header("Health Pickup Spawner")]
    [SerializeField] private GameObject[] healthPickups;
    [SerializeField] private int minHealthItems;
    [SerializeField] private int maxHealthItems;

    [Header("Terrain Size")]
    // Max position of the terrain for spawning locations
    [SerializeField] private float maxPosAxisX;
    [SerializeField] private float maxPosAxisZ;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ItemSpawner(weaponPickups, minWeaponsEach, maxWeaponsEach, maxPosAxisX, maxPosAxisZ);
        ItemSpawner(healthPickups, minHealthItems, maxHealthItems, maxPosAxisX, maxPosAxisZ);
    }

    private void ItemSpawner(GameObject[] _items, int _minItemSpawns, int _maxItemSpawns, float _maxPosAxisX, float _maxPosAxisZ)
    {
        // Random amount of spawns for each item
        int _numSpawns = Random.Range(_minItemSpawns, _maxItemSpawns);
        for(int i = 0; i < _numSpawns; i++)
        {
            foreach(GameObject _item in _items)
            {
                // Generate random position for the item
                Vector3 _spawnPos = new Vector3(Random.Range(0, _maxPosAxisX), _item.transform.position.y, Random.Range(0, _maxPosAxisZ));

                Instantiate(_item, _spawnPos, transform.rotation);
            }
        }
    }
}
