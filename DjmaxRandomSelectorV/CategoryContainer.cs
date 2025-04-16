using DjmaxRandomSelectorV.Models;
using System.Collections.Generic;

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
            _categories = new List<Category>(appData.Categories);
        }
    }
}
