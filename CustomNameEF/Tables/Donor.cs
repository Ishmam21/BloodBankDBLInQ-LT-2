using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace task2.CustomNameEF.Tables;

public partial class Donor
{
    public int DonorId { get; set; }

    [Required(ErrorMessage = "Full Name is required.")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Blood Group is required.")]
    public string BloodGroup { get; set; } = null!;

    [Required(ErrorMessage = "Contact No is required.")]
    public string ContactNo { get; set; } = null!;

    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "Last Donation Date is required.")]
    public DateOnly? LastDonationDate { get; set; }

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();
}
