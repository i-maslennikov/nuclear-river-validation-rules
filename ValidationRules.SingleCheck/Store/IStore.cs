﻿using System.Collections.Generic;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public interface IStore
    {
        void Add<T>(T entity) where T: class ;
        void AddRange<T>(IEnumerable<T> entities) where T : class;
    }
}