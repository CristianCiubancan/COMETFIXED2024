using Comet.Shared;
using Comet.Shared.Comet.Shared;
using System.Threading.Tasks;

namespace Comet.Game.World.Threading
{
    public sealed class GeneratorProcessing : TimerBase
    {
        public GeneratorProcessing()
            : base(1000, "Generator Thread")
        {
        }

        protected override async Task<bool> OnElapseAsync()
        {
            var task = Kernel.GeneratorManager.OnTimerAsync();
            if (await Task.WhenAny(task, Task.Delay(2000)) == task)
            {
                // Task completed within 2 seconds.
                return true;
            }
            else
            {
                // Task took longer than 2 seconds.
                await Log.WriteLogAsync(LogLevel.Warning, "GeneratorProcessing thread maybe got deadlocked");
                // Consider adding additional logging or handling here.
                return false; // Or handle accordingly.
            }
        }
    }
}