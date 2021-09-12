using System.ServiceProcess;

namespace OnOffPCService
{
    public partial class OnOffService : ServiceBase
    {
        /// <summary>
        /// Установка параметров сервиса
        /// </summary>
        private void SetServiceSettings()
        {
            ServiceName = "Сервис отслеживания состояния ПК";
            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = true;
        }
        
        public OnOffService()
        {
            InitializeComponent();
            SetServiceSettings();
        }
        
        protected override void OnStart(string[] args)
        {
            OnOff.GetData();
        }

        protected override void OnStop()
        {
            OnOff.GetData();
        }

        protected override void OnPause()
        {
            OnOff.GetData();
        }

        protected override void OnContinue()
        {
            OnOff.GetData();
        }
    }
}