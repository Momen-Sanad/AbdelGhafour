// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using SuperMarket.Models;

// namespace SuperMarket.Pages.Account
// {
//     public class LoginModel : PageModel
//     {
//         private readonly SignInManager<ApplicationUser> _signInManager;
//         private readonly UserManager<ApplicationUser> _userManager;

//         public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
//         {
//             _signInManager = signInManager;
//             _userManager = userManager;
//         }

//         [BindProperty]
//         public InputModel Input { get; set; }

//         public class InputModel
//         {
//             public string Email { get; set; }
//             public string Password { get; set; }
//         }

//         public async Task<IActionResult> OnPostAsync()
//         {
//             if (!ModelState.IsValid)
//             {
//                 return Page();
//             }

//             var user = await _userManager.FindByEmailAsync(Input.Email);
//             if (user == null)
//             {
//                 ModelState.AddModelError(string.Empty, "Invalid login attempt.");
//                 return Page();
//             }

//             var result = await _signInManager.PasswordSignInAsync(user, Input.Password, false, false);
//             if (result.Succeeded)
//             {
//                 return RedirectToPage("/Index");
//             }
//             else
//             {
//                 ModelState.AddModelError(string.Empty, "Invalid login attempt.");
//                 return Page();
//             }
//         }
//     }
// }
