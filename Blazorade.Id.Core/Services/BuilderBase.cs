using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blazorade.Id.Core.Services
{
    public abstract class BuilderBase<TResult>
    {

        public abstract TResult Build();



        protected Dictionary<string, string> Parameters { get; private set; } = new Dictionary<string, string>();

        protected virtual void AddParameterValue(string key, string value)
        {
            this.Parameters[key] = value;
        }

    }
}
