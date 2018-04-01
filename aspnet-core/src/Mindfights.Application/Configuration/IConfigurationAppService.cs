using System.Threading.Tasks;
using Mindfights.Configuration.Dto;

namespace Mindfights.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
