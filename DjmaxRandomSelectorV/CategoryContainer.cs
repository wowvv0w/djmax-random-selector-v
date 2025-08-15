using DjmaxRandomSelectorV.Models;
using System.Collections.Generic;
using System.Linq;

namespace DjmaxRandomSelectorV
{
    public class CategoryContainer
    {
        private List<Category> _categories;

        public List<Category> GetCategories()
        {
            return _categories.ConvertAll(x => x);
        }

        public void SetCategories(Dmrsv3AppData appData)
        {
            var categories = appData.Categories.Where(cat => cat.Type != 3);
            var plis = appData.PliCategories
                              .SelectMany(pli => pli.Minors, (pli, m) => new Category(m.Name, $"{pli.Major}:{m.Name}", null, 3));
            _categories = new List<Category>(categories.Union(plis));
        }
    }
}
