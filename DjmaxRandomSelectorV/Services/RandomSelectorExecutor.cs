using System;
using Dmrsv.RandomSelector;

namespace DjmaxRandomSelectorV.Services
{
    public class RandomSelectorExecutor : IExecutable
    {
        public event Action<bool> OnExecutionCompleted;

        private readonly IRandomSelector _rs;
        private readonly ITrackDB _db;
        private readonly IConditionBuilder _condBuild;
        private readonly IGroupwiseExtractorBuilder _extrBuild;
        private readonly ILocator _loc;

        private bool _isStateChanged = true;
        private bool _isRunning = false;

        public bool IsRunning { get { return _isRunning; } }

        public RandomSelectorExecutor(IRandomSelector rs, ITrackDB db, ILocator loc,
            IConditionBuilder condBuild, IGroupwiseExtractorBuilder extrBuild)
        {
            _rs = rs;
            _db = db;
            _loc = loc;
            _condBuild = condBuild;
            _extrBuild = extrBuild;
        }

        public void SetStateChanged()
        {
            _isStateChanged = true;
        }

        public void Start()
        {
            _isRunning = true;
            if (_isStateChanged)
            {
                _rs.SetCandidates(_db.Playable, _condBuild.Build(), _extrBuild.Build());
                _isStateChanged = false;
            }
            Pattern selected = _rs.Select();
            _loc.Locate(selected);
            OnExecutionCompleted?.Invoke(selected is not null);
            _isRunning = false;
        }

        public void Restart()
        {
            _isRunning = true;
            Pattern selected = _rs.Reselect();
            _loc.Locate(selected);
            OnExecutionCompleted?.Invoke(selected is not null);
            _isRunning = false;
        }
    }
}
