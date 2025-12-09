using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FinancialTrackr.Services;

namespace FinancialTrackr.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }
        #region properties

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        #endregion

        #region handlers
        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            if (!validate())
            {
                ModelState.AddModelError("", "Mindkét mezõ kitöltése kötelezõ!");
                return Page();
            }

            string enterdHash = HashPassword(Password);

            var user = _context.Users.FirstOrDefault(u => u.Username == Username);

            if (user == null) {
                ModelState.AddModelError("", "Nem létezõ felhasználónév!");
                return Page();
            }

            if (user.Password != enterdHash)
            {
                ModelState.AddModelError("", "Hibás jelszó!");
                return Page();
            }
            TempData["Success"] = $"Üdvözlünk, {user.FullName}!";
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Fullname", user.FullName);
            HttpContext.Session.SetInt32("IsAdmin", user.IsAdmin);

            if (user.IsAdmin == 1)
            {
                return RedirectToPage("AdminPage");

            }

            return RedirectToPage("Userpage"); 
        }
        #endregion

        #region functions
        public bool validate()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {

                return false;
            }
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
        #endregion
    }
}
