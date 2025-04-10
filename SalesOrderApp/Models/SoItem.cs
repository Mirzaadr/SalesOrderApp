﻿using System;
using System.Collections.Generic;

namespace SalesOrderApp.Models
{
    public partial class SoItem
    {
        public long SoItemId { get; set; }

        public long SoOrderId { get; set; }

        public string ItemName { get; set; } = null!;

        public int Quantity { get; set; }

        public double Price { get; set; }

        public SoOrder SoOrder { get; set; }
    }
};
