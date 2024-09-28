using System;
using Google.Protobuf.WellKnownTypes;
using MyAdmin.Core.Utilities;

namespace MyAdmin.Core.Identity;

/// <summary>
/// token 黑名单
/// </summary>
public class TokenBlackList
{
    const string CACHEKEY = "TOKEN_BLACKLIST";
    private readonly ICacheManager<Dictionary<string, long>> _cacheManager; // token, expireHour
    public TokenBlackList(ICacheManager<Dictionary<string, long>> cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public void AddToken(string token, int hour = 48)
    {
        if (!Check.HasValue(token))
        {
            return;
        }
        var list = _cacheManager.Get(CACHEKEY);
        if (list == null)
        {
            list = new Dictionary<string, long>();
        }
        list.Add(token, DateTime.Now.AddHours(hour).Ticks);
        _cacheManager.Save(CACHEKEY, list);
    }

    public bool IsBlackToken(string token)
    {
        var list = _cacheManager.Get(CACHEKEY);
        return list != null && list.ContainsKey(token);
    }

    // todo 定期任务
    public void ClearExpireToken()
    {
        var list = _cacheManager.Get(CACHEKEY);
        if (list == null)
        {
            return;
        }
        var tmp = new Dictionary<string, long>();
        var now = DateTime.Now.Ticks;
        foreach (var key in list.Keys)
        {
            if (list[key] > now)
            {
                tmp.Add(key, list[key]);
            }
        }
        _cacheManager.Save(CACHEKEY, tmp);
    }

}
