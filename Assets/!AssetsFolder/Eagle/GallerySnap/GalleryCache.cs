using Novena.Helpers;
using Novena.Networking.Image;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GalleryCache
{
    private List<CacheObject> cachedImages = new List<CacheObject>();

    /// <summary>
    /// Save image in cache!
    /// </summary>
    /// <param name="path"></param>
    /// <param name="texture"></param>
    public void Save(string path, Texture texture)
    {

        Debug.Log("Saving to cache");
        if (string.IsNullOrEmpty(path) == false && texture != null)
        {
            CacheObject cacheObject = new CacheObject();
            cacheObject.path = path;
            cacheObject.texture = texture;
            cachedImages.Add(cacheObject);
            Debug.Log(cacheObject.path);
            Debug.Log(cacheObject.texture);
        }
    }

    /// <summary>
    /// Load image from cache!
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
#nullable enable
    public Texture? Load(string path)
    {
        Debug.Log("Loading from cache");
        var cachedImage = GetImageInCache(path);
        if (cachedImage != null)
        {
            Debug.Log("Loading from cache succeeded");
            return cachedImage.texture;
        } 

        return null;
    }
#nullable disable
#nullable enable
  private CacheObject? GetImageInCache(string path)
    {
       return cachedImages.FirstOrDefault(ci => ci.path == path);        
    }
#nullable disable

    /// <summary>
    /// Clean cached textures!
    /// </summary>
    public void CleanCache() {
        DestroyTextures();
        cachedImages.Clear();
    }

    /// <summary>
    /// Destroy texture. Clear from memory.
    /// </summary>
    private void DestroyTextures()
    {
        for (int i = 0; i < cachedImages.Count; i++)
        {
            var texture = cachedImages[i].texture;
            UnityHelper.Destroy(texture);
        }
    }

    private class CacheObject {
        public string path;
        public Texture texture;
    }
}
