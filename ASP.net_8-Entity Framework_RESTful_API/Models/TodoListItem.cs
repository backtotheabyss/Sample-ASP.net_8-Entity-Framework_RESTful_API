using System;
using System.Collections.Generic;

namespace ASP.net_8-Entity_Framework_RESTful_API.Models;

public partial class TodoListItem
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public bool? ItemStatus { get; set; }
}
