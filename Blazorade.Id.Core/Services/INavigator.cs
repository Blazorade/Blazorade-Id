using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Core.Services
{
    public interface INavigator
    {

        string? CurrentUri { get; }

        ValueTask NavigateToAsync(string uri);


    }
}
