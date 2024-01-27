using Blazorade.Id.Core.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazorade.Id.Services
{
    public class BlazorNavigator : INavigator
    {
        public BlazorNavigator(NavigationManager navMan)
        {
            this.NavMan = navMan ?? throw new ArgumentNullException(nameof(navMan));
        }

        private readonly NavigationManager NavMan;

        public ValueTask NavigateToAsync(string uri)
        {
            this.NavMan.NavigateTo(uri);
            return ValueTask.CompletedTask;
        }
    }
}
