using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace AspnetcoreLocalizationDemo.Models.Services
{
    public interface ICommentRepository
    {
        IReadOnlyCollection<string> GetComments(CultureInfo culture, string chapter);
        void AddComment(CultureInfo culture, string chapter, string comment);
    }
}