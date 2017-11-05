using Mtn.Library.Enums;

namespace Mtn.Library.Web.Enums
{
    internal enum MtnAjaxCacheType
    {
        [Mtn.Library.Attributes.CacheContainer(CacheType.SortedHash,
            AbsoluteExpirationTime = 0,
            SlideExpiration = 0)]
        AjaxClass,
        [Mtn.Library.Attributes.CacheContainer(CacheType.SortedHash,
            AbsoluteExpirationTime = 0,
            SlideExpiration = 0)]
        AjaxMethodScript,
        [Mtn.Library.Attributes.CacheContainer(CacheType.SortedHash,
            AbsoluteExpirationTime = 0,
            SlideExpiration = 0)]
        AjaxScriptMethod,
        [Mtn.Library.Attributes.CacheContainer(CacheType.SortedHash,
            AbsoluteExpirationTime = 0,
            SlideExpiration = 0)]
        AjaxContructor
        
    }
}
