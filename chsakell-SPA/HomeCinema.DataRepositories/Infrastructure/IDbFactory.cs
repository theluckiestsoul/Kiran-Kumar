﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.DataRepositories.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        HomeCinemaContext Init();
    }
}
