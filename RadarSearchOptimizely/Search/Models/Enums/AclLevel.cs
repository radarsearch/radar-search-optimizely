namespace RadarSearchOptimizely.Search.Models.Enums
{
    [Serializable]
    [Flags]
    public enum AclLevel
    {
        NoAccess = 0,
        Read = 1,
        Create = 2,
        Edit = 4,
        Delete = 8,
        Publish = 16,
        Administer = 32,
        FullAccess = 63,
        Undefined = 1073741824
    }
}
