using Semifinals.Guardian.Models;

namespace Semifinals.Guardian.Utils.Exceptions;

public class RecoveryCodeNotFoundException : NotFoundException
{
    public RecoveryCodeNotFoundException(string identityId, string type)
        : base(RecoveryCode.GetCompositeId(identityId, type)) { }
}
