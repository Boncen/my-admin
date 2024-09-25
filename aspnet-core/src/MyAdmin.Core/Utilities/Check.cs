using System.Text.Json.Nodes;

namespace MyAdmin.Core;

public class Check
{
    public static bool HasValue(string? input){
        return !string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input);
    }

    public static bool GreaterThanZero(dynamic input){
        return input > 0;
    }
    public static bool LessThanZero(dynamic input){
        return input < 0;
    }

    public static bool Between(dynamic target, dynamic a, dynamic b){
        return target > a && target < b;
    }

    public static bool In<T>(T target, IEnumerable<T> collection){
        return collection.Contains(target);
    }

    /// <summary>
    /// 简单判断是否有sql注入风险
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public static bool IfSqlFragmentSafe(string? sql)
    {
        if (!HasValue(sql))
        {
            return true;
        }

        if (sql!.Contains("--") || sql.Contains("//") || sql.Contains("truncate"))// || sql.Contains('(')|| sql.Contains(')'))
        {
            return false;
        }
        return true;
    }
}
