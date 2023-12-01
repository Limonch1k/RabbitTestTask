using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class Repository
{
    private SqliteContext _context {get;set;}

    public Repository(SqliteContext context)
    {
        _context = context;
    }

    public void AddModuleCategoty(List<ModuleCategory> models)
    {
        var models_id_list = models.Select(x => x.ModuleCategoryID).ToList();

        var module_db = _context.ModuleCategories.Where(x => models_id_list.Contains(x.ModuleCategoryID)).ToList();

        var list_of_not_exist_entity = models.Except(module_db, new ModuleCategoryComparer()).ToList();

        foreach(var m in module_db)
        {
            string modelState = models.Where(x => x.ModuleCategoryID.Equals(m.ModuleCategoryID)).Select(x => x.ModuleState).FirstOrDefault();
            m.ModuleState = modelState;
        }

        _context.ModuleCategories.AddRangeAsync(list_of_not_exist_entity);
        
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}