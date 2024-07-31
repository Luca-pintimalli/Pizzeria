using System;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Models;
using Pizzeria.Services;

namespace Pizzeria.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;

        public AccountController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dataContext.Users.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (user == null)
                    TempData["User"] = "Anonimo";
                else
                    TempData["User"] = user.Email;
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }



    }
}