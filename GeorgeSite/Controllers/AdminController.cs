﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GeorgeSite.Models.Data;
using GeorgeSite.Models.Entities;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace GeorgeSite.Controllers
{
    //[Authorize(Roles = "Admins")]
    public class AdminController : Controller
    {
        IRepositoryWrapper Repository;

        public AdminController(IRepositoryWrapper repository)
        {
            Repository = repository;
        }

        public IActionResult Index()
        {
            return View(Repository.ItemRepository.FindAll());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Item item, IFormFile file)
        {
            //if (file == null || file.Length == 0)
            //    return Content("file not selected");
            //else if (!file.FileName.ToLower().Contains(".jpg"))
            //{
            //        return Content("only picture(jpg) please");
            //}

            var arr = file.FileName.Split('.');
            string  ext = arr[arr.Length - 1];

            //Define image name (with a single instance of GUID)
            var ImageName = item.Id + Guid.NewGuid().ToString() +"."+ ext;
            
            //Set image path
            var path = Path.Combine(
                       Directory.GetCurrentDirectory(), @"wwwroot\ClientDocuments",
                       ImageName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                 await file.CopyToAsync(stream);
            }
            
            //Get image url
            item.ImageUrl = "/ClientDocuments/" + ImageName;
            Repository.ItemRepository.Create(item);
            Repository.ItemRepository.Save();
            return RedirectToAction("Index","Admin");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var v = Repository.ItemRepository.GetById(id);
            return View(v);
        }

        [HttpPost]
        public IActionResult Delete(Item item)
        {
            Repository.ItemRepository.Delete(item);
            Repository.ItemRepository.Save();
            return RedirectToAction("Index", "Admin");
        }
    }
}
