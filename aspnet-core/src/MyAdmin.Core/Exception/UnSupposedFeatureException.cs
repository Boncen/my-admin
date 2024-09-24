namespace MyAdmin.Core.Exception;

/// <summary>
/// 未支持功能
/// </summary>
public class UnSupposedFeatureException: MAException
{
    public UnSupposedFeatureException():base("尚未被支持的功能")
    {
        
    }

    public UnSupposedFeatureException(string message):base(message)
    {
        
    }
}