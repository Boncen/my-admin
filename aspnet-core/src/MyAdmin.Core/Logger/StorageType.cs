namespace MyAdmin.Core.Logger;

public enum StorageType {
    None = 0,
    File = 1,
    Database,
    MongoDB // todo
}

public enum LogType
{
    Default = 1,
    Request = 2,
}