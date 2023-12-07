﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ImageSharingWithServerless.DAL;
using Microsoft.AspNetCore.Identity;
using ImageSharingWithServerless.Models;
using ImageSharingModels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ImageSharingWithServerless.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext db;

        protected UserManager<ApplicationUser> userManager;

        protected IImageStorage imageStorage;

        protected BaseController(UserManager<ApplicationUser> userManager, 
                                 IImageStorage imageStorage,
                                 ApplicationDbContext db)
        {
            this.db = db;
            this.userManager = userManager;
            this.imageStorage = imageStorage;
        }


        protected void CheckAda()
        {
            ViewBag.isADA = GetADAFlag();
        }

        protected bool GetADAFlag()
        {
            var cookie = Request.Cookies["ADA"];
            return (cookie != null && "true".Equals(cookie));
        }

        protected async Task<ApplicationUser> GetLoggedInUser()
        {
            var user = HttpContext.User;
            if (user == null || user.Identity == null || user.Identity.Name == null)
            {
                return null;
            }
            return await userManager.FindByNameAsync(user.Identity.Name);
        }

        protected ActionResult ForceLogin()
        {
            return RedirectToAction("Login", "Account");
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        protected IQueryable<Image> ApprovedImages(IQueryable<Image> images)
        {
            return images.Where(im => im.Valid && im.Approved);
        }


        protected IQueryable<ApplicationUser> ActiveUsers()
        {
            return userManager.Users.Where(u => u.Active);
        }
    }
}