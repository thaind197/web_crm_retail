using SalesCRM.Domain.Entities;

namespace SalesCRM.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateJwtToken(ApplicationUser user, IList<string> roles);
}
