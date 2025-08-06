using System;
using System.Collections.Generic;

namespace Models;

public partial class TodoList
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;
}
