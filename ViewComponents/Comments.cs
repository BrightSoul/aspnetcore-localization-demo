using System.Globalization;
using System.Threading.Tasks;
using AspnetcoreLocalizationDemo.Models.Services;
using AspnetcoreLocalizationDemo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AspnetcoreLocalizationDemo.Models.Localization
{
    public class CommentsViewComponent : ViewComponent
    {
        private readonly ICommentRepository commentRepository;
        public CommentsViewComponent(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public IViewComponentResult Invoke()
        {
            string action = HttpContext.Request.RouteValues["action"] as string;
            var viewModel = new CommentsViewModel
            {
                Results = commentRepository.GetComments(CultureInfo.CurrentCulture, action),
                Shown = HttpContext.Request.Query.ContainsKey("comments"),
                Comment = string.Empty,
                Chapter = action
            };
            return View(viewModel);
        }
    }
}