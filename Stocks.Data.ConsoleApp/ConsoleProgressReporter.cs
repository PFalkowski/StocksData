using Konsole;
using ProgressReporting;

namespace Stocks.Data.ConsoleApp
{
    public class ConsoleProgressReporter : ProgressReporter
    {
        private ProgressBar _progressBar;

        public override void Start(double targetValue)
        {
            _progressBar = new ProgressBar((int)targetValue);
            base.Start(targetValue);
        }

        public override void Restart(double targetValue)
        {
            _progressBar = new ProgressBar((int)targetValue);
            base.Restart(targetValue);
        }

        public override void ReportProgress(double rawProgressValue)
        {
            _progressBar.Refresh((int)rawProgressValue, "Working");
            base.ReportProgress(rawProgressValue);
        }
    }
}
