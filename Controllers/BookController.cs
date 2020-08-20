using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspnetcoreLocalizationDemo.Models;

namespace AspnetcoreLocalizationDemo.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Chapter1()
        {
            return View();
        }
        public IActionResult Chapter2()
        {
            return View();
        }
    }
}
