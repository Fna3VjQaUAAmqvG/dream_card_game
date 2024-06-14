//通用型，使用+对象池
using UnityEngine;
using UnityEngine.Pool;

[DefaultExecutionOrder(-100)] //数字越小，这里代码越优先执行
public class PoolTool : MonoBehaviour
{
    public GameObject objPrefab;
    private ObjectPool<GameObject> pool;
    private void Start()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(objPrefab, transform),
            actionOnGet: (obj) => obj.SetActive(value: true),
            actionOnRelease: (obj) => obj.SetActive(value: false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false, //不需要检查对象池集合
            defaultCapacity: 10,
            maxSize: 20
        );
        PreFillPool(7);
    }
    private void PreFillPool(int count) //预先填充对象池
    {
        var preFillArray = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            preFillArray[i] = pool.Get();
        }
        foreach(var item in preFillArray)
        {
            pool.Release(item);
        }
    }
    
    public GameObject GetObjectFromPool() //对外方法
    {
        return pool.Get();
    }
    public void ReturnObjectToPool(GameObject obj) //对外方法
    {
        pool.Release(obj);
    }
}
