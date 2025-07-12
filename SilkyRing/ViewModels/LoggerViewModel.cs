using System.Threading.Tasks;
using System.Windows;
using SilkyRing.Memory.DLLShared;

namespace SilkyRing.ViewModels
{
    public class LoggerViewModel : BaseViewModel
    {
        
        private readonly DllManager _dllManager;
        private bool _isInitializing;
        
        private string _logText = string.Empty;
        
        private bool _isSetEventLogging;
        private bool _isApplySpEffectLogging; 
        private bool _isAiGoalsLogging;
        private bool _isApplySpEffectUniqueLogging;
        private bool _isSetEventUniqueLogging;
        
        public bool IsInitializing 
        { 
            get => _isInitializing; 
            set { _isInitializing = value; OnPropertyChanged(); }
        }
        
        public LoggerViewModel(DllManager dllManager)
        {
            _dllManager = dllManager;
            _dllManager.LogReceived += OnLogReceived;
        }

        public async Task InitializeAsync()
        {
            IsInitializing = true;
            
            await Task.Run(() =>
            {
                _dllManager.EnsureInjectedDll();
                _dllManager.StartLogReading();
            });
        
            IsInitializing = false;
        }
        
        public bool IsSetEventLogging
        {
            get => _isSetEventLogging;
            set 
            { 
                if (SetProperty(ref _isSetEventLogging, value))
                    _dllManager.SetLogCommand(LogCommand.LogSetEvent, value);
            }
        }
    
        public bool IsApplySpEffectLogging
        {
            get => _isApplySpEffectLogging;
            set 
            { 
                if (SetProperty(ref _isApplySpEffectLogging, value))
                    _dllManager.SetLogCommand(LogCommand.LogApplySpeffect, value);
            }
        }
        
        public bool IsAiGoalsLogging
        {
            get => _isAiGoalsLogging;
            set 
            { 
                if (SetProperty(ref _isAiGoalsLogging, value))
                    _dllManager.SetLogCommand(LogCommand.LogAiGoals, value);
            }
        }
    
        public bool IsApplySpEffectUniqueLogging
        {
            get => _isApplySpEffectUniqueLogging;
            set 
            { 
                if (SetProperty(ref _isApplySpEffectUniqueLogging, value))
                    _dllManager.SetLogCommand(LogCommand.LogUniqueSpeffect, value);
            }
        }
    
        public bool IsSetEventUniqueLogging
        {
            get => _isSetEventUniqueLogging;
            set 
            { 
                if (SetProperty(ref _isSetEventUniqueLogging, value))
                    _dllManager.SetLogCommand(LogCommand.LogUniqueSetEvent, value);
            }
        }
        
        public string LogText 
        { 
            get => _logText; 
            set => SetProperty(ref _logText, value);
        }
        
        public void ClearUniqueSetEvents()
        {
            _dllManager.SetLogCommand(LogCommand.ClearUniqueSetEvent, true);
        }
    
        public void ClearUniqueSpEffects()
        {
            _dllManager.SetLogCommand(LogCommand.ClearUniqueSpeffect, true);
        }
    
        public void ClearConsole()
        {
            LogText = string.Empty;
        }
        public void PauseAllLogging()
        {
            IsSetEventLogging = false;
            IsApplySpEffectLogging = false;
            IsAiGoalsLogging = false;
            IsApplySpEffectUniqueLogging = false;
            IsSetEventUniqueLogging = false;
        }
    
        private void OnLogReceived(object sender, string logs)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogText += logs;
            });
        }
    }
}