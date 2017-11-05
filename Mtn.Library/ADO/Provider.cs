using System;
using System.Linq;
using System.Reflection;
using Mtn.Library.Attributes;
using Mtn.Library.Configuration;
using Mtn.Library.Enums;
using Mtn.Library.Extensions;
using Mtn.Library.Service;

namespace Mtn.Library.ADO
{
    /// <summary>
    /// ADO Provider
    /// </summary>
    public static class Provider
    {
        private enum CacheProvider
        {
            [CacheContainer(CacheType.Hash, AbsoluteExpirationTime = 0)] Provider
        }
        /// <summary>
        /// GetProvider
        /// </summary>
        /// <param name="providerInfo"></param>
        /// <returns></returns>
        public static IDataProvider GetProvider(String providerInfo = null)
        {
            if (providerInfo.IsNullOrWhiteSpaceMtn())
                providerInfo = Config.GetString("Mtn.Library.Data.Provider");

            var provider = Cache.Instance[CacheProvider.Provider, providerInfo];
            if (provider != null) return (IDataProvider) provider;
            var assemblyName = providerInfo.SplitMtn(",").First();
            var iDataProvider = Assembly.Load(assemblyName);
            if (iDataProvider != null)
            {
                var instanceName = providerInfo.SplitMtn(",").Last();
                provider = iDataProvider.CreateInstance(instanceName, true, BindingFlags.Default, null,
                    new object[0], null, new object[0]);
            }
            Cache.Instance.Add(CacheProvider.Provider, provider, providerInfo);

            return (IDataProvider) provider;
        }
    }
}