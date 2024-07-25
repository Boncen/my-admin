namespace MyAdmin.Core;

public class Check
{
    public static bool HasValue(string input){
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
}
