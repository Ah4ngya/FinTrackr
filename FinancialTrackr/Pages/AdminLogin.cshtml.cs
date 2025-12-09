using FinancialTrackr.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FinancialTrackr.Pages
{

    public class AdminLoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public AdminLoginModel(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
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

            var user = _context.Users.FirstOrDefault(u => u.Username == Username && u.IsAdmin == 1);

            if (user == null)
            {
                ModelState.AddModelError("", "Nem létezõ Admin felhasználónév!");
                return Page();
            }

            if (user.Password != Password)
            {
                ModelState.AddModelError("", "Hibás jelszó!");
                return Page();
            }
            TempData["Success"] = $"Üdvözlünk, {user.FullName}!";
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Fullname", user.FullName);
            return RedirectToPage("AdminPage");
        }
        public bool validate()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {

                return false;
            }
            return true;
            
        }


     
    }
}
