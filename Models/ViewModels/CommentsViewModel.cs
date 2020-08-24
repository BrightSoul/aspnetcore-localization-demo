using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspnetcoreLocalizationDemo.Models.ViewModels
{
    public class CommentsViewModel
    {
        public IReadOnlyCollection<string> Results { get; set; }
        public bool Shown { get; set; }

        [Required(ErrorMessage = "Il capitolo non può essere vuoto")]
        public string Chapter { get; set; }

        [Required(ErrorMessage = "Devi inserire un commento"),
        StringLength(100, MinimumLength = 3, ErrorMessage = "Il commento deve essere di almeno {2} e di al massimo {1} caratteri")]
        public string Comment { get; set; }
    }
}