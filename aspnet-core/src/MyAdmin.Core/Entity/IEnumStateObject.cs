namespace MyAdmin.Core.Entity;

public interface IEnumStateObject<TEnum> where TEnum: System.Enum
{
    TEnum State { get; set; }
}