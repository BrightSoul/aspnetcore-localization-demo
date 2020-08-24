using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace AspnetcoreLocalizationDemo.Models.Services
{
    public class InMemoryCommentRepository : ICommentRepository
    {
        private readonly ConcurrentDictionary<string, List<string>> allComments = new ConcurrentDictionary<string, List<string>>();
        private readonly IReadOnlyCollection<string> emptyCommentList = new List<string>().AsReadOnly();

        public IReadOnlyCollection<string> GetComments(CultureInfo culture, string chapter)
        {
            if (culture == null || string.IsNullOrEmpty(chapter))
            {
                throw new ArgumentNullException();
            }
            var key = GetKey(culture, chapter);
            if (allComments.TryGetValue(key, out List<string> comments))
            {
                return comments.AsReadOnly();
            }
            return emptyCommentList;
        }

        public void AddComment(CultureInfo culture, string chapter, string comment)
        {   
            if (culture == null || string.IsNullOrEmpty(chapter))
            {
                throw new ArgumentNullException();
            }
            var key = GetKey(culture, chapter);
            var comments = allComments.GetOrAdd(key, entry => new List<string>());
            comments.Add(comment);
        }

        private string GetKey(CultureInfo culture, string chapter)
        {
            return $"{culture.TwoLetterISOLanguageName}.{chapter}";
        }
    }
}