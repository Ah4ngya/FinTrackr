using FinancialTrackr.Data;
using FinancialTrackr.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FinancialTrackr.Pages
{
    
    
    public class RegPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegPageModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string Fullname { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfPassword { get; set; }
        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            if (!validate())
            {
                return Page();
            }
            string hashPw = HashPassword(Password);

            var user = new User
            {
                FullName = Fullname,
                Username = Username,
                Email = Email,
                Password = hashPw,
                IsAdmin = 0
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Sikeres regisztráció!";
            return RedirectToPage("Login");
        }
        public bool validate()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Fullname) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfPassword))
            {
                ModelState.AddModelError("", "Minden mezõ kötelezõ!");
                return false;

            }
            if (!Regex.IsMatch(Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$"))
            {
                ModelState.AddModelError("", "A jelszónak legalább 8 karakterbõl kell állnia, tartalmaznia kell kis-, nagybetût és számot!");
            }
            if (Password != ConfPassword)
            {
                ModelState.AddModelError("", "A két jelszó nem egyezik meg!");

                return false;
            }
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("", "Helytelen email formátum!");

                return false;
            }
            return ModelState.IsValid;



        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2")); // hex
                return builder.ToString();
            }
        }

       
    }
    
}
