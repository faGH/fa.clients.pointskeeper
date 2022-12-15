using FrostAura.Clients.PointsKeeper.Components.Abstractions;
using FrostAura.Clients.PointsKeeper.Components.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrostAura.Clients.PointsKeeper.Components.Banners
{
  /// <summary>
  /// Component for generating an input form automatically based on a given type and instance of the type.
  /// </summary>
  public partial class Carousel : BaseComponent<Carousel>
  {
        [Parameter]
        public TimeSpan Delay { get; set; } = TimeSpan.FromSeconds(5);
        [Parameter]
        public List<CarouselScene> Scenes { get; set; }
        /// <summary>
        /// JavaScript runtime engine.
        /// </summary>
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JsRuntime.InvokeVoidAsync("eval", GetJsBoostrappingCode());
            await base.OnAfterRenderAsync(firstRender);
        }

        private string GetJsBoostrappingCode()
        {
            return @"
                (() => {
                    window.carouselsLoaded = window.carouselsLoaded || {};
                    const currentCarouselIsLoaded = window.carouselsLoaded['" + Id + @"'] || false;

                    if(currentCarouselIsLoaded) return;

                    setInterval(() => {
                        const track = document.getElementById('" + Id + @"').querySelector(':scope > .track');
                        const currentScene = track.querySelector('.active');
                        let nextScene = currentScene.nextElementSibling;

                        if(nextScene == null){
                            nextScene = currentScene.parentElement.querySelector('.scene')
                        }

                        const lengthToMove = nextScene.style.left;
                        const transform = 'translateX(-' + lengthToMove + ')';

                        track.style.transform = transform;

                        currentScene.classList.remove('active');
                        nextScene.classList.add('active');
                    }, " + Delay.TotalMilliseconds + @");

                    window.carouselsLoaded['" + Id + @"'] = true;
                })();
            ";
        }

        private bool IsSceneActive(CarouselScene scene)
        {
            return Scenes.First().Id == scene.Id;
        }
    }
}
