public class ModuleCategory
{
    public string ModuleCategoryID {get;set;}

    public string ModuleState {get;set;}
}

public class ModuleCategoryComparer : IEqualityComparer<ModuleCategory>
{
    public bool Equals(ModuleCategory x, ModuleCategory y)
    {
        return x.ModuleCategoryID == y.ModuleCategoryID;
    }

    public int GetHashCode(ModuleCategory obj)
    {
        return obj.ModuleCategoryID.GetHashCode();
    }
}