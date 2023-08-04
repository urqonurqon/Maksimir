using System.Collections.Generic;
using UnityEngine;

namespace Novena.Utility
{
  public class ObjectPooling : MonoBehaviour
  {
    public static ObjectPooling self;

    public Transform objectPool;

    public GameObject[] pooledObject;
    public int[] pooledItemsAmount;
    public bool willGrow = true;

    public List<List<GameObject>> pooledObjects;


    private void Awake()
    {
      self = this;
    }

    // Use this for initialization
    void Start()
    {
      pooledObjects = new List<List<GameObject>>();

      for (int i = 0; i < pooledObject.Length; i++)
      {
        List<GameObject> pooledObjectsTypes = new List<GameObject>();

        for (int j = 0; j < pooledItemsAmount[i]; j++)
        {
          GameObject obj = (GameObject)Instantiate(pooledObject[i], objectPool, true);
          obj.SetActive(false);
          pooledObjectsTypes.Add(obj);
        }

        pooledObjects.Add(pooledObjectsTypes);
      }
    }


    public GameObject GetPooledObject(int type)
    {
      for (int i = 0; i < pooledObjects[type].Count; i++)
      {
        if (!pooledObjects[type][i].activeInHierarchy)
        {
          return pooledObjects[type][i];
        }
      }

      if (willGrow)
      {
        GameObject obj = (GameObject)Instantiate(pooledObject[type]);
        obj.transform.SetParent(objectPool);
        pooledObjects[type].Add(obj);

        return obj;
      }

      return null;
    }


    /// <summary>
    /// Return game object to poll
    /// </summary>
    /// <param name="tr"></param>
    public void ReturnToPool(Transform tr)
    {
      tr.gameObject.SetActive(false);
      tr.SetParent(objectPool);
    }

    /// <summary>
    /// Return game objects to poll
    /// </summary>
    /// <param name="gameObjects">List of GameObjects</param>
    public void ReturnToPool(List<GameObject> gameObjects)
    {
      for (int i = 0; i < gameObjects.Count; i++)
      {
        var go = gameObjects[i];
        
        go.SetActive(false);
        go.transform.SetParent(objectPool);
      }
    }
  }
}