﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chsakell_SPA.Models
{
    public class TotalRentalHistoryViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int TotalRentals
        {
            get
            {
                return Rentals.Count;
            }
            set { }
        }
        public List<RentalHistoryPerDate> Rentals { get; set; }
    }

    public class RentalHistoryPerDate
    {
        public int TotalRentals { get; set; }
        public DateTime Date { get; set; }
    }
}