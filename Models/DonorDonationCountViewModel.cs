namespace task2.Models
{
    public class DonorDonationCountViewModel
    {
        public int DonorId { get; set; }
        public string FullName { get; set; } = "";
        public string BloodGroup { get; set; } = "";
        public string City { get; set; } = "";
        public int TotalDonations { get; set; }
    }
}
