using Caliburn.Micro;
using DjmaxRandomSelectorV.Models;
using System.Collections.Generic;

namespace DjmaxRandomSelectorV.ViewModels
{
    public abstract class CategoryContainer : Screen
    {
        protected readonly List<Category> _categories = new()
        {
            new Category("RESPECT", "RP", null),
            new Category("PORTABLE 1", "P1", null),
            new Category("PORTABLE 2", "P2", null),
            new Category("PORTABLE 3", "P3", "1568680"),
            new Category("TRILOGY", "TR", "1271670"),
            new Category("CLAZZIQUAI", "CE", "1300210"),
            new Category("BLACK SQUARE", "BS", "1300211"),
            new Category("V EXTENSION", "VE", "1080550"),
            new Category("V EXTENSION 2", "VE2", "1843020"),
            new Category("V EXTENSION 3", "VE3", "2164540"),
            new Category("EMOTIONAL S.", "ES", "1238760"),
            new Category("TECHNIKA 1", "T1", "1386610"),
            new Category("TECHNIKA 2", "T2", "1386611"),
            new Category("TECHNIKA 3", "T3", "1386612"),
            new Category("TECHNIKA T&Q", "TQ", "1958171"),
            new Category("GUILTY GEAR", "GG", null),
            new Category("CHUNITHM", "CHU", "1472190"),
            new Category("CYTUS", "CY", "1356221"),
            new Category("DEEMO", "DM", "1356220"),
            new Category("ESTIMATE", "ESTI", "1664550"),
            new Category("GROOVE COASTER", "GC", "1271671"),
            new Category("GIRLS' FRONTLINE", "GF", "1472191"),
            new Category("MUSE DASH", "MD", "1782170"),
            new Category("NEXON", "NXN", "1958170"),
        };

        public BindableCollection<ListUpdater> CategoryUpdaters { get; private set; }

        protected void InitializeCategoryUpdaters(ICollection<string> target)
        {
            var updaters = _categories.ConvertAll(x => new ListUpdater(x.Name, x.Id, target));
            CategoryUpdaters = new BindableCollection<ListUpdater>(updaters);
        }
    }
}
