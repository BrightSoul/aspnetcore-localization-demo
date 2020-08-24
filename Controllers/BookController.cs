using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspnetcoreLocalizationDemo.Models;
using AspnetcoreLocalizationDemo.Models.ViewModels;
using AspnetcoreLocalizationDemo.Models.Services;
using System.Globalization;

namespace AspnetcoreLocalizationDemo.Controllers
{
    public class BookController : Controller
    {
        private readonly ILogger<BookController> logger;
        public BookController(ILogger<BookController> logger)
        {
            this.logger = logger;
        }
        public IActionResult Chapter1(int? comments)
        {
            // I commenti sono visualizzati dal view component /ViewComponents/Comments.cs
            if (comments.HasValue) logger.LogInformation("Chapter 1 will be shown with comments");
            return View();
        }
        public IActionResult Chapter2(int? comments)
        {
            // I commenti sono visualizzati dal view component /ViewComponents/Comments.cs
            if (comments.HasValue) logger.LogInformation("Chapter 2 will be shown with comments");
            return View();
        }

        [HttpPost]
        public IActionResult Comment(CommentsViewModel model, [FromServices] ICommentRepository commentRepository)
        {
            if (ModelState.IsValid)
            {
                commentRepository.AddComment(CultureInfo.CurrentCulture, model.Chapter, model.Comment);
            }
            else
            {
                //TODO: Visualizza tutti gli errori e non solo il primo
                TempData["Error"] = ModelState.SelectMany(m => m.Value.Errors).FirstOrDefault()?.ErrorMessage;
            }
            return RedirectToAction(model.Chapter, new { comments = 1 });
        }
    }
}
