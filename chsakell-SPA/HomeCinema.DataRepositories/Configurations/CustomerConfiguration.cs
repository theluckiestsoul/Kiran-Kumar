﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Entities.Entities;

namespace HomeCinema.DataRepositories.Configurations
{
    public class CustomerConfiguration : EntityBaseConfiguration<Customer>
    {
        public CustomerConfiguration()
        {
            Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            Property(u => u.LastName).IsRequired().HasMaxLength(100);
            Property(u => u.IdentityCard).IsRequired().HasMaxLength(50);
            Property(u => u.UniqueKey).IsRequired();
            Property(c => c.Mobile).HasMaxLength(10);
            Property(c => c.Email).IsRequired().HasMaxLength(200);
            Property(c => c.DateOfBirth).IsRequired();
        }
    }
}
