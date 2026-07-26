using System;
using System.Collections.Generic;

namespace task2.CustomNameEF.Tables;

public partial class Donation
{
    public int DonationId { get; set; }

    public int DonorId { get; set; }

    public DateOnly DonationDate { get; set; }

    public int VolumeMl { get; set; }

    public string? CampName { get; set; }

    public virtual Donor Donor { get; set; } = null!;
}
