using Blazorade.Id.Core.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazorade.Id.Core.Services
{
    /// <summary>
    /// A storage factory service implementation.
    /// </summary>
    /// <remarks>
    /// The storage factory is responsible for proving an application with
    /// the storage that is configured for each authority configured for the application.
    /// </remarks>
    public class StorageFactory
    {
        public StorageFactory(IOptionsFactory<AuthorityOptions> optionsFactory, ISessionStorage sessionStorage, IPersistentStorage persistentStorage)
        {
            this.OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
            this.SessionStorage = sessionStorage ?? throw new ArgumentNullException(nameof (sessionStorage));
            this.PersistentStorage = persistentStorage ?? throw new ArgumentNullException(nameof(persistentStorage));
        }

        private readonly IOptionsFactory<AuthorityOptions> OptionsFactory;


        /// <summary>
        /// Returns the session storage configured for the application.
        /// </summary>
        public ISessionStorage SessionStorage { get; private set; }

        /// <summary>
        /// Returns the persistent storage configured for the application.
        /// </summary>
        public IPersistentStorage PersistentStorage { get; private set; }



        public IStorage GetConfiguredStorage(string? authorityKey)
        {
            var options = this.OptionsFactory.Create(authorityKey ?? "");
            return this.GetConfiguredStorage(options);
        }

        public IStorage GetConfiguredStorage(AuthorityOptions options)
        {
            return options.CacheMode == TokenCacheMode.Session
                ? (IStorage)this.SessionStorage
                : (IStorage)this.PersistentStorage;
        }
    }
}
