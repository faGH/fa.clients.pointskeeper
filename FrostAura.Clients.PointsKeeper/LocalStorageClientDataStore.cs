using System;
using FrostAura.Clients.PointsKeeper.Interfaces;
using Microsoft.JSInterop;

namespace FrostAura.Clients.PointsKeeper
{
    /// <summary>
    /// Client local storage data store.
    /// </summary>
	public class LocalStorageClientDataStore : IClientDataStore
    {
        /// <summary>
        /// JS runtime to allow client-side access.
        /// </summary>
        private readonly IJSRuntime _jsRuntime;

        /// <summary>
        /// Overloaded constructr to allow for dependency injection.
        /// </summary>
        /// <param name="jsRuntime">JS runtime to allow client-side access.</param>
        public LocalStorageClientDataStore(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Get data by a key.
        /// </summary>
        /// <param name="key">Key that uniquely identifies the data to fetch.</param>
        /// <param name="token">A token to allow cancelling downstream operations.</param>
        /// <returns>The parsed data, if any.</returns>
        public async Task<string?> GetAsync(string key, CancellationToken token)
        {
            var data = await _jsRuntime.InvokeAsync<string>("eval", $"localStorage.getItem('{key}');");

            return data;
        }

        /// <summary>
        /// Get data by a key.
        /// </summary>
        /// <param name="key">Key that uniquely identifies the data to set.</param>
		/// <param name="data">Data to persist.</param>
        /// <param name="token">A token to allow cancelling downstream operations.</param>
        /// <returns>Void</returns>
        public async Task SetAsync(string key, string data, CancellationToken token)
        {
            await _jsRuntime.InvokeAsync<string>("eval", $"localStorage.setItem('{key}', '{data}')");
        }
    }
}

