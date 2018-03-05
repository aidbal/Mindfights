using System.Threading.Tasks;
using Skautatinklis.Configuration.Dto;

namespace Skautatinklis.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
