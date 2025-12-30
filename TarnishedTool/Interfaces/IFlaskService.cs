// 

using System.Threading;
using System.Threading.Tasks;

namespace TarnishedTool.Interfaces;

public interface IFlaskService
{
    Task TryUpgradeFlask(CancellationToken ct = default);
    Task TryIncreaseCharges(CancellationToken ct = default);
}