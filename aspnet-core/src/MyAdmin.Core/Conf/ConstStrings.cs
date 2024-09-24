namespace MyAdmin.Core.Conf;

public static class ConstStrings
{
    /// <summary>
    /// 默认使用滑动窗口方式
    /// </summary>
    public static string RateLimitingPolicyName = "sliding";
    /// <summary>
    /// 缓存所有表实体的缓存key
    /// </summary>
    public static string MACacheKeyAllEntities = "MA-AllEntities";

    public static class WhereConfitionType
    {
        public const string Contains = "contains";
        public const string In = "in";
        public const string LessThan = "lessThan";
        public const string GreaterThan = "greaterThan";
        public const string LessThanOrEqual = "lessThanOrEqual";
        public const string GreaterThanOrEqual = "greaterThanOrEqual";
        public const string Equal = "equal";
        public const string NotEqual = "notEqual";
    }

    public static class JoinType
    {
        // public const string Join = "join"; // 默认
        public const string LeftJoin = "left";
        public const string RightJoin = "right";
        public const string InnerJoin = "inner";
        public const string OuterJoin = "outer";
    }
}