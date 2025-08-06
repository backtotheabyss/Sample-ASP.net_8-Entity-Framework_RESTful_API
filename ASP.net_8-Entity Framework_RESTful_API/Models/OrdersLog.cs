using System;
using System.Collections.Generic;

namespace Models;

public partial class OrdersLog
{
    public int OrderId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Action { get; set; }
}
