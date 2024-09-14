namespace MyAdmin.Core.Conf;

public static class ConstSettingValue
{
    /// <summary>
    /// 默认使用滑动窗口方式
    /// </summary>
    public static string RateLimitingPolicyName = "sliding";
    /// <summary>
    /// 缓存所有表实体的缓存key
    /// </summary>
    public static string MACacheKeyAllEntities = "MA-AllEntities";
}