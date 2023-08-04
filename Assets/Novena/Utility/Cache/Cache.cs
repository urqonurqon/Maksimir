using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Novena.Utility.Cache
{
  public static class Cache
  {
    private static List<CacheObject> _cachedObjects = new List<CacheObject>();
    
    /// <summary>
    /// Store object in cache.
    /// If object exist it will be updated.
    /// </summary>
    /// <param name="key">Uniq key</param>
    /// <param name="id">Uniq id</param>
    /// <param name="objToStore"></param>
    public static void Store(string key, int id, object objToStore)
    {
      CacheObject cache;

      cache = Get(key, id);
      
      if (cache != null)
      {
        cache.CachedObject = objToStore;
      }
      else
      {
        cache = new CacheObject();
        cache.Key = key;
        cache.Id = id;
        cache.CachedObject = objToStore;
        _cachedObjects.Add(cache);
      }
    }

    /// <summary>
    /// Get item from cache
    /// </summary>
    /// <param name="key"></param>
    /// <param name="id"></param>
    /// <returns>CacheObject</returns>
    public static CacheObject Get(string key, int id)
    {
      return _cachedObjects.FirstOrDefault(co => co.Key == key && co.Id == id);
    }

    public static bool Contains(string key, int id)
    {
      return _cachedObjects.Any(co => co.Key == key && co.Id == id);
    }

    /// <summary>
    /// Delete items from cache if KEY is not passed all cache will be deleted (cleared)!
    /// </summary>
    /// <param name="key">Default value 0</param>
    public static void ClearCache(string key = "0")
    {
     var tempList = _cachedObjects;

      if (key != "0")
      {
        tempList = _cachedObjects.Where(co => co.Key == key).ToList();
      }
      
      for (int i = 0; i < tempList.Count; i++)
      {
        var co = tempList[i];
        co.CachedObject = null;
      }
      _cachedObjects?.RemoveAll(co => co.Key == key);
    }

    /// <summary>
    /// Delete items from cache if KEY is not passed all items in cache will be deleted!
    /// </summary>
    /// <param name="key">Default value 0</param>
    public static async Task ClearCacheAsync(string key = "0")
    {
      await Task.Run(() => ClearCache(key));
    }
    
    public class CacheObject
    {
      public string Key { get; set; }
      public int Id { get; set; }
      public object CachedObject { get; set; }
    }
  }
  
}