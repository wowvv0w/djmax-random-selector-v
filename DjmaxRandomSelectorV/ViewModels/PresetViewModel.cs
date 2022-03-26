using Caliburn.Micro;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjmaxRandomSelectorV.ViewModels
{
    public class PresetViewModel : Screen
    {
        private const string PRESET_PATH = "Data/Preset";
        public BindableCollection<string> PresetItems { get; set; }

        public PresetViewModel()
        {
            GetPresetList();
        }

        public void GetPresetList()
        {
            DirectoryInfo di = new DirectoryInfo(PRESET_PATH);
            FileInfo[] files = di.GetFiles("*.json");

            string presetName;
            int startIndex;
            PresetItems = new BindableCollection<string>();
            foreach (FileInfo file in files)
            {
                presetName = file.Name;
                startIndex = presetName.Length - 5;
                PresetItems.Add(presetName.Remove(startIndex, 5));
            }
        }

        public void LoadPreset(string name)
        {
        }

        public void RemoveItem(string name)
        {
            string path = $"{PRESET_PATH}/{name}.json";
            if (File.Exists(path)) File.Delete(path);
            PresetItems.Remove(name);
        }
    }
}
