using FrostAura.Clients.PointsKeeper.Components.Abstractions;
using FrostAura.Clients.PointsKeeper.Components.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrostAura.Clients.PointsKeeper.Components.Background
{
  /// <summary>
  /// Component for allowing automatic client-initiated refreshes.
  /// </summary>
  public partial class Refresher : BaseComponent<Refresher>, IAsyncDisposable
  {
        /// <summary>
        /// JavaScript runtime engine.
        /// </summary>
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        /// <summary>
        /// The function to call on the refresh interval.
        /// </summary>
        [Parameter]
        public Action RefreshDelegate { get; set; }
        /// <summary>
        /// The interval at which to refresh.
        /// </summary>
        [Parameter]
        public TimeSpan RefreshInterval { get; set; }
        /// <summary>
        /// When true, refresh on repeat with the provided interval.
        /// </summary>
        [Parameter]
        public bool RefreshIndefinitely { get; set; }

        /// <summary>
        /// A cleanup process that clears client-side intervals.
        /// </summary>
        /// <returns>Void</returns>
        public async ValueTask DisposeAsync()
        {
            var cleanupCommand = @"
                (() => {
                    window.refreshers = window.refreshers || {};
                    window.refreshers['" + Id + @"'] = window.refreshers['" + Id + @"'] || {
                        reference: csharpObj,
                        initialized: false,
                        interval: null
                    }
                    const refresher = window.refreshers['" + Id + @"'];

                    if(!!refresher){
                        if(!!refresher.interval) clearInterval(refresher.interval);

                        delete window.refreshers['" + Id + @"'];
                    }
                })();";

            await JsRuntime.InvokeVoidAsync("eval", cleanupCommand);
        }

        /// <summary>
        /// The actual refresh function that would be called from the client-side.
        /// </summary>
        /// <returns></returns>
        [JSInvokable]
        public Task RefreshAsync()
        {
            RefreshDelegate?.Invoke();

            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var thisJsReference = DotNetObjectReference.Create(this);
            var bootstrapCommand = @"var boostrapDashboard = (csharpObj, id) => {
                if(!csharpObj) throw new Error('A valid C# reference object is required.');
                if(!id) throw new Error('A valid id is required.');

                window.refreshers = window.refreshers || {};
                window.refreshers[id] = window.refreshers[id] || {
                    reference: csharpObj,
                    initialized: false,
                    interval: null
                };
            }";
            var mainLoopCommand = @"(() => {
                const refresher = window.refreshers['" + Id + @"']

                if(refresher.initialized) return;

                refresher.interval = " + (RefreshIndefinitely ? "setInterval": "setTimeout") + @"(() => {
                    refresher.reference.invokeMethodAsync('" + nameof(RefreshAsync) + @"')
                }, " + RefreshInterval.TotalMilliseconds + @");

                refresher.initialized = true;
            })();";

            await JsRuntime.InvokeVoidAsync("eval", bootstrapCommand);
            await JsRuntime.InvokeVoidAsync("boostrapDashboard", thisJsReference, Id);
            await JsRuntime.InvokeVoidAsync("eval", mainLoopCommand);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
