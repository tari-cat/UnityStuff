using UnityEngine;

/// <summary>
/// Revision 1.00 //
/// Author: <see href="https://github.com/tari-cat/UnityStuff"/>
/// 
/// <para>A super simple object pool script.</para>
/// <para>Examples: </para>
/// <example>
/// To create an object pool, use the code block below with your component, prefab, and desired pool size.
/// <code>
///     ObjectPool myObjectPool = new ObjectPool&lt;T&gt;(GameObject prefab, int size); 
/// </code>
/// <code>
///     T pooledObject = myObjectPool.Get();
/// </code>
/// <code>
///     myObjectPool.Release(pooledObject);
/// </code>
/// <code>
///     myObjectPool.Clear();
/// </code>
/// </example>
/// 
/// </summary>
/// <typeparam name="T">"Component"</typeparam>
public class ObjectPool<T> where T : Component
{
    public int size;
    public GameObject[] objects;
    public GameObject prefab;

    public ObjectPool(GameObject prefab, int size)
    {
        this.prefab = prefab;
        if (prefab.GetComponent<T>() == null)
            throw new System.Exception("The prefab doesn't have the component type!");
        this.size = size;
        objects = new GameObject[size];
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < size; i++)
        {
            objects[i] = Object.Instantiate(prefab);
            objects[i].name = prefab.name + "(Pooled) ID:" + i;
            objects[i].SetActive(false);
        }
    }

    /// <summary>
    /// Activates an object. Remember to deactivate the object in the object pool instance when done.
    /// </summary>
    public T Get()
    {
        foreach (GameObject go in objects)
        {
            if (go.activeSelf == false)
            {
                go.SetActive(true);
                return go.GetComponent<T>();
            }
        }
        return default;
    }

    /// <summary>
    /// Deactivates the pooled object.
    /// </summary>
    public void Release(GameObject go)
    {
        go.SetActive(false);
    }

    /// <summary>
    /// Clears the object pool, deleting all objects and setting its contents to null.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < size; i++)
        {
            Object.Destroy(objects[i]);
        }

        objects = null;
        prefab = null;
        size = -1;
    }
}