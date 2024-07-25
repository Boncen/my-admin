namespace MyAdmin.Core.Logger;

public class LoggerOption{
    public StorageType? LogStorageType { get; set; }
    public bool? SplitFileViaLevel { get; set; }
    public bool? DatebasedDirectoryStructure { get; set; }
}