using System;
using System.Collections.Generic;

namespace RepairTracking.Data.Models;

public partial class Mail
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Subject { get; set; }
    public string Template { get; set; }
}
