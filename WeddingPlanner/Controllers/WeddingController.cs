using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;

public class WeddingController : Controller
{
    private DatabaseContext _context;

    public WeddingController(DatabaseContext context)
    {
        _context = context;
    }

    private int? uid
    {
        get
        {
            return HttpContext.Session.GetInt32("UUID");
        }
    }

     private bool loggedIn
    {
        get
        {
            return uid != null;
        }
    }




    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        List<Wedding> allWeddings = _context.Weddings
        .Include(wedding => wedding.Creator)
        .Include(wedding => wedding.Attendees)
        .ThenInclude(attendee => attendee.User)
        .ToList();

        return View("Dashboard", allWeddings);
    }

    [HttpGet("/new")]
    public IActionResult New()
    {
        int? userId = HttpContext.Session.GetInt32("UUID");
        if (userId == null)
        {
            return RedirectToAction("Index", "User");
        }

        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        return View("New");
    }

    [HttpPost("/create")]
    public IActionResult Create(Wedding newWedding)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        if (ModelState.IsValid == false)
        {
            return New();
        }

        if (uid != null)
        {
            newWedding.UserId = (int)uid;
        }

        _context.Weddings.Add(newWedding);
        _context.SaveChanges();

        return Dashboard();

    }


    [HttpGet("/weddings/{weddingId}")]
    public IActionResult ViewWedding(int weddingId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        Wedding? wedding = _context.Weddings
            .Include(w => w.Creator)
            .Include(w => w.Attendees)
            .ThenInclude(attendee => attendee.User)
            .FirstOrDefault(w => w.WeddingId == weddingId);

        if (wedding == null)
        {
            return Dashboard();
        }

        return View("ViewWedding", wedding);


    }

    [HttpPost("/weddings/{deleteId}/delete")]
    public IActionResult Delete(int deleteId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        Wedding? weddingToBeDeleted = _context.Weddings.FirstOrDefault(wedding => wedding.WeddingId == deleteId);

        if (weddingToBeDeleted != null)
        {
            if (weddingToBeDeleted.UserId == uid)
            {
                _context.Weddings.Remove(weddingToBeDeleted);
                _context.SaveChanges();
            }

        }
        return Dashboard();
    }

    [HttpGet("/weddings/{weddingId}/edit")]
    public IActionResult EditWedding(int weddingId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        Wedding? wedding = _context.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);

        if (wedding == null || wedding.UserId != uid)
        {
            return Dashboard();

        }

        return View("Edit", wedding);

    }

    [HttpPost("/weddings/{updatedWeddingId}/update")]
    public IActionResult UpdateWedding(int updatedWeddingId, Wedding updatedWedding)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        if (ModelState.IsValid == false)
        {
            return EditWedding(updatedWeddingId);
        }

        Wedding? dbWedding = _context.Weddings.FirstOrDefault(w => w.WeddingId == updatedWeddingId);

        if (dbWedding == null || dbWedding.UserId != uid)
        {
            return Dashboard();
        }

        dbWedding.WedderOne = updatedWedding.WedderOne;
        dbWedding.WedderTwo = updatedWedding.WedderTwo;
        dbWedding.WeddingAddress = updatedWedding.WeddingAddress;
        dbWedding.Date = updatedWedding.Date;
        dbWedding.UpdatedAt = DateTime.Now;

        _context.Weddings.Update(dbWedding);
        _context.SaveChanges();

        return RedirectToAction("ViewWedding", new { weddingId = dbWedding.WeddingId});
    }

    [HttpPost("/weddings/{weddingId}/RSVP")]
    public IActionResult RSVP(int weddingId)
    {
        if(!loggedIn || uid == null)
        {
            return RedirectToAction("Index", "User");
        }

        RSVP? existingRSVP = _context.RSVPS.FirstOrDefault(rsvp => rsvp.WeddingId == weddingId && rsvp.UserId == uid);

        if (existingRSVP == null)
        {
            RSVP newRSVP = new RSVP(){
                WeddingId = weddingId,
                UserId = (int)uid
            };
            _context.RSVPS.Add(newRSVP);
        }
        else {
            _context.Remove(existingRSVP);
        }

        _context.SaveChanges();
        return Dashboard();
    }



}