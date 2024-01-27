using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{

    public interface IStorage
    {
        ValueTask ClearAsync();

        ValueTask<bool> ContainsKeyAsync(string key);

        ValueTask<string> GetItemAsync(string key);

        ValueTask<IEnumerable<string>> GetKeysAsync();

        ValueTask<int> GetItemCountAsync();

        ValueTask RemoveItemAsync(string key);

        ValueTask RemoveItemsAsync(IEnumerable<string> keys);

        ValueTask SetItemAsync(string key, string value);
    }

    public interface ISessionStorage : IStorage
    {
    }

    public interface IPersistentStorage : IStorage
    {

    }
}
