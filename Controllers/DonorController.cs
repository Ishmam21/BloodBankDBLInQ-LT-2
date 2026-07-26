using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task2.CustomNameEF;
using task2.CustomNameEF.Tables;
using task2.Models;

namespace task2.Controllers;

public class DonorController : Controller
{
    private readonly BloodBankDbContext db;

    public DonorController(BloodBankDbContext db)
    {
        this.db = db;
    }

    // ===== CRUD: LIST =====
    public IActionResult Index()
    {
        var donors = db.Donors.ToList();
        return View(donors);
    }

    // ===== CRUD: CREATE (GET) =====
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Donor());
    }

    // ===== CRUD: CREATE (POST) =====
    [HttpPost]
    public IActionResult Create(string FullName, string BloodGroup, string ContactNo, string City, string LastDonationDate)
    {
        // Server-side validation
        if (string.IsNullOrWhiteSpace(FullName))
            ModelState.AddModelError("FullName", "Full Name is required.");
        if (string.IsNullOrWhiteSpace(BloodGroup))
            ModelState.AddModelError("BloodGroup", "Blood Group is required.");
        if (string.IsNullOrWhiteSpace(ContactNo))
            ModelState.AddModelError("ContactNo", "Contact No is required.");
        if (string.IsNullOrWhiteSpace(City))
            ModelState.AddModelError("City", "City is required.");

        DateOnly parsedDate = DateOnly.MinValue;
        if (string.IsNullOrWhiteSpace(LastDonationDate))
            ModelState.AddModelError("LastDonationDate", "Last Donation Date is required.");
        else if (!DateOnly.TryParse(LastDonationDate, out parsedDate))
            ModelState.AddModelError("LastDonationDate", "Invalid date format.");

        if (!ModelState.IsValid)
        {
            var donor = new Donor
            {
                FullName = FullName ?? "",
                BloodGroup = BloodGroup ?? "",
                ContactNo = ContactNo ?? "",
                City = City ?? "",
                LastDonationDate = parsedDate == DateOnly.MinValue ? null : parsedDate
            };
            return View(donor);
        }

        var newDonor = new Donor
        {
            FullName = FullName,
            BloodGroup = BloodGroup,
            ContactNo = ContactNo,
            City = City,
            LastDonationDate = parsedDate
        };

        db.Donors.Add(newDonor);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    // ===== CRUD: EDIT (GET) =====
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var donor = db.Donors.Find(id);
        if (donor == null)
            return NotFound();
        return View(donor);
    }

    // ===== CRUD: EDIT (POST) =====
    [HttpPost]
    public IActionResult Edit(int id, string FullName, string BloodGroup, string ContactNo, string City, string LastDonationDate)
    {
        var donor = db.Donors.Find(id);
        if (donor == null)
            return NotFound();

        // Server-side validation
        if (string.IsNullOrWhiteSpace(FullName))
            ModelState.AddModelError("FullName", "Full Name is required.");
        if (string.IsNullOrWhiteSpace(BloodGroup))
            ModelState.AddModelError("BloodGroup", "Blood Group is required.");
        if (string.IsNullOrWhiteSpace(ContactNo))
            ModelState.AddModelError("ContactNo", "Contact No is required.");
        if (string.IsNullOrWhiteSpace(City))
            ModelState.AddModelError("City", "City is required.");

        DateOnly parsedDate = DateOnly.MinValue;
        if (string.IsNullOrWhiteSpace(LastDonationDate))
            ModelState.AddModelError("LastDonationDate", "Last Donation Date is required.");
        else if (!DateOnly.TryParse(LastDonationDate, out parsedDate))
            ModelState.AddModelError("LastDonationDate", "Invalid date format.");

        if (!ModelState.IsValid)
        {
            donor.FullName = FullName ?? "";
            donor.BloodGroup = BloodGroup ?? "";
            donor.ContactNo = ContactNo ?? "";
            donor.City = City ?? "";
            return View(donor);
        }

        donor.FullName = FullName;
        donor.BloodGroup = BloodGroup;
        donor.ContactNo = ContactNo;
        donor.City = City;
        donor.LastDonationDate = parsedDate;

        db.SaveChanges();
        return RedirectToAction("Index");
    }

    // ===== CRUD: DELETE (GET) =====
    [HttpGet]
    public IActionResult Delete(int id)
    {
        var donor = db.Donors.Find(id);
        if (donor == null)
            return NotFound();
        return View(donor);
    }

    // ===== CRUD: DELETE (POST) =====
    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var donor = db.Donors.Include(d => d.Donations).FirstOrDefault(d => d.DonorId == id);
        if (donor == null)
            return NotFound();

        // Remove related donations first
        db.Donations.RemoveRange(donor.Donations);
        db.Donors.Remove(donor);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    // ===== LINQ QUERY 1: Filter by Blood Group =====
    [HttpGet]
    public IActionResult FilterByBloodGroup(string bloodGroup)
    {
        var allBloodGroups = db.Donors.Select(d => d.BloodGroup).Distinct().ToList();
        ViewBag.BloodGroups = allBloodGroups;
        ViewBag.SelectedBloodGroup = bloodGroup;

        List<Donor> donors;
        if (!string.IsNullOrEmpty(bloodGroup))
            donors = db.Donors.Where(d => d.BloodGroup == bloodGroup).ToList();
        else
            donors = new List<Donor>();

        return View(donors);
    }

    // ===== LINQ QUERY 2: Sorted by LastDonationDate Descending =====
    public IActionResult SortedByDate()
    {
        var donors = db.Donors
            .OrderByDescending(d => d.LastDonationDate)
            .ToList();
        return View(donors);
    }

    // ===== LINQ QUERY 3: Each donor with total donation count =====
    public IActionResult DonorDonationCount()
    {
        var result = db.Donors
            .Include(d => d.Donations)
            .Select(d => new DonorDonationCountViewModel
            {
                DonorId = d.DonorId,
                FullName = d.FullName,
                BloodGroup = d.BloodGroup,
                City = d.City,
                TotalDonations = d.Donations.Count
            })
            .ToList();

        return View(result);
    }

    // ===== LINQ QUERY 4: Total volume of blood collected =====
    public IActionResult TotalVolume()
    {
        var totalVolume = db.Donations.Sum(d => d.VolumeMl);
        ViewBag.TotalVolume = totalVolume;
        return View();
    }
}