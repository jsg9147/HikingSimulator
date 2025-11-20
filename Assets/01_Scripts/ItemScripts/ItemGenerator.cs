using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : SingletonBase<ItemGenerator>
{
    public ItemPickup pickupPrefab;
    public ItemPickup itemBoxPrefab;
    public List<ItemData> dropItemList;
    public int numberOfObjects = 10;
    public Vector3 spawnAreaCenter;
    public Vector3 spawnAreaSize;

    void Start()
    {
        SpawnObjects();
    }

    public void DropItemGenerate(ItemData itemData, int count)
    {
        Debug.Log(itemData.itemName + " : " + count);
        Transform playerTransform = GameManager.instance.playerTransfrom; // 플레이어의 Transform 가져오기
        Vector3 throwDirection = (playerTransform.forward * 0.5f) + (playerTransform.up * 1f); // 플레이어가 쳐다보는 방향

        ItemPickup spawnedObject = Instantiate(pickupPrefab, playerTransform.position + throwDirection, Quaternion.identity);
        spawnedObject.SetItem(itemData, count);
        spawnedObject.Throw();
    }

    void SpawnObjects()
    {
        if (pickupPrefab != null && dropItemList.Count > 0)
        {
            for (int i = 0; i < numberOfObjects; i++)
            {
                Vector3 randomPosition = GetRandomPosition();
                ItemPickup spawnedObject = Instantiate(pickupPrefab, randomPosition, Quaternion.identity);
                spawnedObject.SetItem(dropItemList[Random.Range(0, dropItemList.Count)]);
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
        float y = spawnAreaCenter.y + spawnAreaSize.y / 2; // 공중에 생성 후 땅에 붙임
        float z = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2);

        return new Vector3(x, y, z);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}