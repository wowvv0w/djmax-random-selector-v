using DjmaxRandomSelectorV.Models;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV
{
    public class CategoryContainer
    {
        private readonly List<Category> _categories;

        public CategoryContainer(Dmrsv3AppData appData)
        {
            _categories = new List<Category>(appData.Categories);
        }

        public List<Category> GetCategories()
        {
            return _categories.ConvertAll(x => x);
        }
    }
}
