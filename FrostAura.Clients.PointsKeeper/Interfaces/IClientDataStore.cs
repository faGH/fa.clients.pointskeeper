namespace FrostAura.Clients.PointsKeeper.Interfaces
{
	/// <summary>
	/// Get and set data async.
	/// </summary>
	public interface IClientDataStore
	{
		/// <summary>
		/// Get data by a key.
		/// </summary>
		/// <param name="key">Key that uniquely identifies the data to fetch.</param>
		/// <param name="token">A token to allow cancelling downstream operations.</param>
		/// <returns>The parsed data, if any.</returns>
		Task<string?> GetAsync(string key, CancellationToken token);
        /// <summary>
        /// Get data by a key.
        /// </summary>
        /// <param name="key">Key that uniquely identifies the data to set.</param>
		/// <param name="data">Data to persist.</param>
        /// <param name="token">A token to allow cancelling downstream operations.</param>
        /// <returns>Void</returns>
        Task SetAsync(string key, string data, CancellationToken token);
    }
}

