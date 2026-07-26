using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task2.CustomNameEF;
using task2.CustomNameEF.Tables;

namespace task2.Controllers;

public class DonorController : Controller
{
    private readonly BloodBankDbContext db;

    public DonorController(BloodBankDbContext db)
    {
        this.db = db;
    }

    public IActionResult Index()
    {
        var data = db.Donors.ToList();
        return View(data);
    }

    public IActionResult Details(int id)
    {
        ViewBag.DonorId = id;
        var data = db.Donors.Find(id);
        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Donor());
    }

    [HttpPost]
    public IActionResult Create(Donor d)
    {
        if (ModelState.IsValid)
        {
            db.Donors.Add(d);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(d);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var data = db.Donors.Find(id);
        ViewBag.DonorId = id;
        return View(data);
    }

    [HttpPost]
    public IActionResult Edit(Donor d)
    {
        if (ModelState.IsValid == false)
        {
            return View(d);
        }

        var data = db.Donors.Find(d.DonorId);
        data.FullName = d.FullName;
        data.BloodGroup = d.BloodGroup;
        data.City = d.City;
        data.ContactNo = d.ContactNo;
        data.LastDonationDate = d.LastDonationDate;
        db.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var data = db.Donors.Find(id);
        ViewBag.DonorId = id;
        return View(data);
    }

    [HttpPost]
    public IActionResult Delete(string Decision, int DonorId)
    {
        if (Decision == "Yes")
        {
            var data = db.Donors.Find(DonorId);
            db.Donors.Remove(data);
            db.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    public IActionResult FilterByBloodGroup(string BloodGroup)
    {
        if (BloodGroup == null) BloodGroup = "A+";
        var data = (from item in db.Donors where item.BloodGroup.Trim() == BloodGroup.Trim() select item).ToList();
        ViewBag.BloodGroup = BloodGroup;
        return View(data);
    }

    public IActionResult SortedByDate()
    {
        var data = (from item in db.Donors orderby item.LastDonationDate descending select item).ToList();
        return View(data);
    }

    public IActionResult DonorDonationCount()
    {
        var data = db.Donors.Include(d => d.Donations).ToList();
        return View(data);
    }
}