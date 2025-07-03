﻿using System;
using System.Collections.Generic;

namespace BusinessObjects.Domains;

public partial class PaymentMethod
{
    public int MethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public string? IconUrl { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
