namespace MyAdmin.Core.Framework.Attribute;

/// <summary>
/// 标志接口不进行请求记录
/// </summary>
[AttributeUsage(AttributeTargets.Class| AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class IgnoreRequestLog:System.Attribute
{
}