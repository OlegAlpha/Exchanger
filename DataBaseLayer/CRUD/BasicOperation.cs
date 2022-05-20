using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseLayer.CRUD;
public class BasicOperation
{
    private static Context context = new Context();

    private void Add(object entity)
    {
        lock (context)
        {
            context.Add(entity);
            context.SaveChanges();
        }
    }

    public async Task AddAsync(object entity)
    {
        await Task.Run(()=> Add(entity));
    }
}
