namespace TheaAdmin.Domain;

public enum DataStatus : byte
{
    None,
    Active,
    Disabled,
    Deleted
}
public enum Gender : byte
{
    Unknown,
    Male,
    Female
}
public enum MenuType : byte
{
    Unknown,
    Root,
    Menu,
    Page
}
