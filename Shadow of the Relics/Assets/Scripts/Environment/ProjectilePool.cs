using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : Pool<Projectile>{}

public abstract class Pool<T> : MonoBehaviour where T : MonoBehaviour
{
    public T prefab;
    public int startAmount;
    List<T> pool = new List<T>();

    static Pool<T> instance;

    void Awake()
    {
        GenerateItems(startAmount);
        instance = this;
    }

    T GenerateItems(int amount)
    {
        T item = null;
        for(int i = 0; i < amount; i++)
        {
            item = Instantiate(prefab.gameObject, transform).GetComponent<T>();
            item.gameObject.SetActive(false);
            pool.Add(item);
        }
        return item;
    }

    public static T GetItem()
    {
        foreach(T item in instance.pool)
        {
            if(item.gameObject.activeInHierarchy)
                continue;
            
            return item;
        }
        return instance.GenerateItems(1);
    }
}