using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Login.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Login.Controllers
{
    public class HomeController : Controller
    {
        private Context _context;

        public HomeController(Context context)
        {
            _context = context;
        }
        private Users GetUserFromDB()
        {
            return _context.Users.FirstOrDefault(i => i.UserId == HttpContext.Session.GetInt32("LoggedInID"));
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(Users user)
        {
            // Check initial ModelState
            if (ModelState.IsValid)
            {
                // If a User exists with provided email
                if (_context.Users.Any(i => i.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");

                    return View("Index", user);
                }
                PasswordHasher<Users> Hasher = new PasswordHasher<Users>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("LoggedInID", (int)user.UserId);
                return Redirect($"/Dashboard");
            }
            else
            {
                return View("Index", user);
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(i => i.Email == userSubmission.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                if (result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("LoggedInID", (int)userInDb.UserId);
                return Redirect($"Dashboard");
            }
            return View("Index");
        }
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            Users LoggedInID = GetUserFromDB();
            if (LoggedInID == null)
            {
                return RedirectToAction("logout");
            }
            ViewBag.User = LoggedInID;
            List<Weddings> Weddings = _context.Weddings.Include(i => i.Planner).Include(i => i.WeddingGuests).ThenInclude(i => i.User).ToList();
            return View(Weddings);
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet("new")]
        public IActionResult New()
        {
            Users LoggedInID = GetUserFromDB();
            if (LoggedInID == null)
            {
                return RedirectToAction("logout");
            }
            return View();
        }
        [HttpPost("Create")]
        public IActionResult Create(Weddings wedding)
        {
            Users LoggedInID = GetUserFromDB();
            if (ModelState.IsValid)
            {
                if (DateTime.Now > (DateTime)wedding.WeddingDate)
                {
                    ModelState.AddModelError("WeddingDate", "Wedding Must be in the Future");
                    return View("New", wedding);
                }
                else
                {
                    wedding.Planner = LoggedInID;
                    wedding.UserId = LoggedInID.UserId;
                    _context.Add(wedding);
                    _context.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
            }
            return View("New", wedding);
        }
        [HttpGet("View/{id}")]
        public IActionResult View(int id)
        {
            Users LoggedInID = GetUserFromDB();
            if (LoggedInID == null)
            {
                return RedirectToAction("logout");
            }
            Weddings wedding = _context.Weddings.Include(i => i.WeddingGuests).ThenInclude(i => i.User).Include(i => i.Planner).FirstOrDefault(i => i.WeddingId == id);
            ViewBag.Wedding = wedding;
            return View("View", wedding);
        }
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            Weddings toRemove = _context.Weddings.SingleOrDefault(i => i.WeddingId == id);
            _context.Weddings.Remove(toRemove);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet("UnRSVP/{WeddingId}/{GuestId}")]
        public IActionResult UnRSVP(int WeddingId, int GuestId)
        {
            // Weddings wedding = _context.Weddings.FirstOrDefault(i => i.WeddingId == WeddingId);
            // Users user = _context.Users.FirstOrDefault(j => j.UserId == GuestId);
            Guests UnRSVP = _context.Guests.FirstOrDefault(u => u.UserId == GuestId && u.WeddingId == WeddingId);
            _context.Guests.Remove(UnRSVP);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet("RSVP/{WId}/{GId}")]
        public IActionResult RSVP(int WId, int GId)
        {
            Weddings wedding = _context.Weddings.FirstOrDefault(i => i.WeddingId == WId);
            Users user = _context.Users.FirstOrDefault(j => j.UserId == GId);
            Guests RSVP = new Guests();
            RSVP.UserId = user.UserId;
            RSVP.User = user;
            RSVP.WeddingId = wedding.WeddingId;
            RSVP.Wedding = wedding;
            _context.Add(RSVP);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
