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
        private Action<string> filterReloader;
        public BindableCollection<string> PresetItems { get; set; }

        public PresetViewModel(Action<string> filterReloader)
        {
            GetPresetList();
            this.filterReloader = filterReloader;
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

        public void LoadPreset(string presetName) => filterReloader.Invoke(presetName);

        public void RemoveItem(string presetName)
        {
            string path = $"{PRESET_PATH}/{presetName}.json";
            if (File.Exists(path)) File.Delete(path);
            PresetItems.Remove(presetName);
        }
    }
}
