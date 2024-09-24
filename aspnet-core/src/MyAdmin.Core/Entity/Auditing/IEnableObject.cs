namespace MyAdmin.Core.Entity.Auditing;

public interface IEnableObject
{
    bool IsEnabled { get; set; }
}