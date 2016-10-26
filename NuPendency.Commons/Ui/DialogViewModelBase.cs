using System.Threading.Tasks;
using NuPendency.Commons.Extensions;

namespace NuPendency.Commons.Ui
{
    public abstract class DialogViewModelBase : ViewModelBase
    {
        public Task DialogCompletedTask = new Task(() => { });
    }
}