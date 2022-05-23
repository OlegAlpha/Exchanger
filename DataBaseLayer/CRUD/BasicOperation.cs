﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseLayer.CRUD;
public class BasicOperation
{
    private readonly Context _context;
    public BasicOperation(Context context)
    {
        _context = context;
    }

    private void Add(object entity)
    {
        _context.Add(entity);
        _context.SaveChanges();
    }

    public async Task AddAsync(object entity)
    {
        await Task.Run(()=> Add(entity));
    }
}
