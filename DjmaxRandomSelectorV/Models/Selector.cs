using CsvHelper;
using DjmaxRandomSelectorV.DataTypes;
using DjmaxRandomSelectorV.DataTypes.Enums;
using DjmaxRandomSelectorV.DataTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DjmaxRandomSelectorV.Models
{
    public class Selector : IAddonObserver
    {
        #region Fields
        private List<Track> allTrackList;
        private List<Track> trackList;
        private List<Music> musicList;
        private ISifter sifter;
        private IProvider provider;
        private int titleCount;
        private bool isRunning;
        private bool isUpdated;
        #endregion

        #region Properties
        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }
        #endregion

        #region Constants
        private const string AllTrackListPath = "Data/AllTrackList.csv";
        private const string AllTrackListUrl = "https://raw.githubusercontent.com/wowvv0w/djmax-random-selector-v/main/DjmaxRandomSelectorV/Data/AllTrackList.csv";
        #endregion

        public Selector()
        {
            isRunning = false;
            isUpdated = true;
        }

        public Music Start(Filter filter, Setting setting)
        {
            isRunning = true;

            List<string> favorite = filter.IncludesFavorite ? setting.Favorite : new List<string>();
            List<string> recents = filter.Recents;

            if (filter.IsUpdated || this.isUpdated)
            {
                Sift(filter, favorite);
                SetTitleCount();
                recents.Clear();
                filter.IsUpdated = false;
                this.isUpdated = false;
            }
            else
            {
                filter.UpdateRecents(titleCount, setting.RecentsCount);
            }
            var musicList = from music in this.musicList
                            where !recents.Contains(music.Title)
                            select music;

            Music selectedMusic;
            try
            {
                selectedMusic = Pick(musicList.ToList());
            }
            catch (ArgumentOutOfRangeException e)
            {
                isRunning = false;
                throw e;
            }

            Provide(selectedMusic, trackList, setting.InputDelay);
            recents.Add(selectedMusic.Title);
            isRunning = false;
            return selectedMusic;
        }

        #region IAddonObserver Methods
        public void Update(IAddonObservable observable)
        {
            var setting = observable as Setting;

            SetSifter(setting.Mode, setting.Level);
            SetProvider(setting.Mode, setting.Aider);
            isUpdated = true;
        }
        private void SetSifter(Mode mode, Level level)
        {
            if (mode.Equals(Mode.Freestyle))
            {
                switch (level)
                {
                    case Level.Off:
                        sifter = new Freestyle();
                        break;
                    case Level.Beginner:
                        sifter = new FreestyleWithLevel("BEGINNER");
                        break;
                    case Level.Master:
                        sifter = new FreestyleWithLevel("MASTER");
                        break;
                }
            }
            else
            {
                sifter = new Online();
            }
        }
        private void SetProvider(Mode mode, Aider aider)
        {
            switch (aider)
            {
                case Aider.Off:
                    provider = new Locator(false);
                    break;
                case Aider.AutoStart:
                    if (mode.Equals(Mode.Freestyle))
                        provider = new Locator(true);
                    else
                        provider = new Locator(false);
                    break;
                case Aider.Observe:
                    provider = new Observer();
                    break;
            }
        }
        #endregion

        #region etc
        private void SetTitleCount()
        {
            var titleList = from music in musicList
                            select music.Title;
            titleCount = titleList.Distinct().Count();
        }
        #endregion

        #region Manage Track List
        public List<string> GetTitleList() => allTrackList.ConvertAll(x => x.Title).Distinct().ToList();
        public void DownloadAllTrackList()
        {
            string data;

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                data = client.DownloadString(AllTrackListUrl);
            }

            using (var writer = new StreamWriter(AllTrackListPath))
            {
                writer.Write(data);
            }
        }
        public void ReadAllTrackList()
        {
            using (var reader = new StreamReader(AllTrackListPath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<TrackMap>();
                var records = csv.GetRecords<Track>().ToList();
                allTrackList = records;
            }
        }
        public void UpdateTrackList(List<string> ownedDlcs)
        {
            var basicCategories = new List<string>() { "RP", "P1", "P2", "GG" };
            var titleFilter = CreateTitleFilter(ownedDlcs);
            var trackList = from track in allTrackList
                            where (ownedDlcs.Contains(track.Category) || basicCategories.Contains(track.Category))
                            && !titleFilter.Contains(track.Title)
                            select track;

            this.trackList = trackList.ToList();
            isUpdated = true;
        }
        private List<string> CreateTitleFilter(List<string> ownedDlcs)
        {
            var list = new List<string>();

            if (!ownedDlcs.Contains("P3"))
            {
                list.Add("glory day (Mintorment Remix)");
                list.Add("glory day -JHS Remix-");
            }
            if (!ownedDlcs.Contains("TR"))
                list.Add("Nevermind");
            if (!ownedDlcs.Contains("CE"))
                list.Add("Rising The Sonic");
            if (!ownedDlcs.Contains("BS"))
                list.Add("ANALYS");
            if (!ownedDlcs.Contains("T1"))
                list.Add("Do you want it");
            if (!ownedDlcs.Contains("T2"))
                list.Add("End of Mythology");
            if (!ownedDlcs.Contains("T3"))
                list.Add("ALiCE");
            if (ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Here in the Moment ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && ownedDlcs.Contains("BS") && !ownedDlcs.Contains("T1"))
                list.Add("Airwave ~Extended Mix~");
            if (!ownedDlcs.Contains("CE") && !ownedDlcs.Contains("BS") && ownedDlcs.Contains("T1"))
                list.Add("SON OF SUN ~Extended Mix~");
            if (!ownedDlcs.Contains("VE") && ownedDlcs.Contains("VE2"))
                list.Add("너로피어오라 ~Original Ver.~");

            return list;
        }
        #endregion

        #region Select Music
        public void Sift(Filter filter, List<string> favorite)
        {
            List<string> styles = new List<string>();
            foreach(string button in filter.ButtonTunes)
                foreach(string difficulty in filter.Difficulties)
                    styles.Add($"{button}{difficulty}");

            var trackList = from track in this.trackList
                            where filter.Categories.Contains(track.Category)
                                || favorite.Contains(track.Title)
                            select track;

            musicList =  sifter.Sift(trackList.ToList(), styles, filter.Levels, filter.ScLevels);
        }
        public Music Pick(List<Music> musicList)
        {
            var random = new Random();
            var index = random.Next(musicList.Count - 1);
            var selectedMusic = musicList[index];

            return selectedMusic;
        }
        private void Provide(Music selectedMusic, List<Track> trackList, int delay)
        {
            provider.Provide(selectedMusic, trackList, delay);
        }
        #endregion

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    }
}
