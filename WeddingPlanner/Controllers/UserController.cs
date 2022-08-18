using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;

public class UserController : Controller
{
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

    // db is just a variable name, can be called anything (e.g. DATABASE, db, _db, etc)
    private DatabaseContext db;
     
    // here we can "inject" our context service into the constructor
    public UserController(DatabaseContext context)
    {
        db = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        if (loggedIn)
        {
            return RedirectToAction("Success");
        }
        return View("Index");
    }

    [HttpGet("/success")]
    public IActionResult Success()
    {
        if (loggedIn)
        {
            return View("Success");
        }
        return Index();
    }

    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (db.Users.Any(user => user.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "is taken");
            }
        }

        if (ModelState.IsValid == false)
        {
            return Index();
        }

        // now we hash our passwords
        PasswordHasher<User> hashBrowns = new PasswordHasher<User>();
        newUser.Password = hashBrowns.HashPassword(newUser, newUser.Password);

        db.Users.Add(newUser);
        db.SaveChanges();

        // now that we've run SaveChanges() we have access to the UserId from our SQL db
        HttpContext.Session.SetInt32("UUID", newUser.UserId);
        HttpContext.Session.SetString("Name", newUser.FullName());
        return RedirectToAction("Dashboard", "Wedding");

    }

    [HttpPost("/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if (ModelState.IsValid == false)
        {
            return Index();
        }

        User? dbUser = db.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);

        if (dbUser == null)
        {
            // normally login validations should be more generic to avoid phishing
            // but we're using specific error messages for testing
            ModelState.AddModelError("LoginEmail", "not found");
            return Index();
        }

        PasswordHasher<LoginUser> hashBrowns = new PasswordHasher<LoginUser>();
        PasswordVerificationResult pwCompareResult = hashBrowns.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

        if (pwCompareResult == 0)
        {
            ModelState.AddModelError("LoginPassword", "is not correct");
            return Index();
        }

        // no returns, therefore no errors
        HttpContext.Session.SetInt32("UUID", dbUser.UserId);
        HttpContext.Session.SetString("Name", dbUser.FullName());
        return RedirectToAction("Dashboard", "Wedding");
    }

    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    // [HttpGet("/profile")]
    // public IActionResult Profile()
    // {
    //     if(!loggedIn)
    //     {
    //         return RedirectToAction("Index");
    //     }

    //     User? oneUser = db.Users.FirstOrDefault(user => user.UserId == uid);

    //     if (oneUser != null)
    //     {
    //         return View("Profile", oneUser);
    //     }

    //     return RedirectToAction("Logout");
    // }
}