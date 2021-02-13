using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using IoTSuite.Client.Pages;
using IoTSuite.Shared;
using IoTSuite.Client.Services;

namespace IoTSuite.Client.Pages.Things
{
    public partial class EditThingComponent : ComponentBase
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, dynamic> Attributes { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager UriHelper { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected IoTSuiteService IoTSuiteService { get; set; }

        [Parameter]
        public dynamic Id { get; set; }

        Thing _data;
        protected Thing data
        {
            get
            {
                return _data;
            }
            set
            {
                if (!object.Equals(_data, value))
                {
                    _data = value;
                    InvokeAsync(() => { StateHasChanged(); });
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            data = (await IoTSuiteService.GetThing(Id)).Data;
        }

        protected async Task FormSubmit(Thing thing)
        {
            try
            {
                Console.WriteLine(thing.Features);
                /*await IoTSuiteService.UpdateThing(thing);*/
                DialogService.Close(thing);
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Unable to update Thing");
            }
        }

        protected async Task CloseButtonClick(MouseEventArgs args)
        {
            DialogService.Close();
        }
    }
}