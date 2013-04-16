using System.Collections.Generic;

namespace Coordination
{
    public interface ISimpleBuilder
    {
        ISimpleBuilder Instance { get; }

        IEnumerable<BuiltComponents> Build(System.Configuration.Configuration configuration);
    }
}