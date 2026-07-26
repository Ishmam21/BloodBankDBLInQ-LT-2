using Microsoft.AspNetCore.Mvc;
using task2.CustomNameEF;

namespace task2.Controllers;

public class DonationController : Controller
{
    private protected BloodBankDbContext db;

    public DonationController(BloodBankDbContext db)
    {
        this.db = db;
    }

    public IActionResult TotalVolume()
    {
        int total = (from item in db.Donations select item.VolumeMl).Sum();
        return View(total);
    }
}
