# ObjectPool
A very simple object pooling script. <br/>
To use, create a new ObjectPool using `new ObjectPool<T>(GameObject prefab, int size);`. <br/>
Then, call `ObjectPool.Get();` to get a pooled object. <br/>
Then when you're done, call `ObjectPool.Release(GameObject);` to release it back into the pool. <br/>
When you don't need the object pool anymore, it might be a good idea to run `ObjectPool.Clear();`. <br/>
