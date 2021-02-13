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

namespace IoTSuite.Client.Pages.PowerMeter
{
    public partial class EditPowerMeterComponent : ComponentBase
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

        PowerMeterInstantaneous _powerMeterInstantaneous;
        protected PowerMeterInstantaneous powerMeterInstantaneous
        {
            get
            {
                return _powerMeterInstantaneous;
            }
            set
            {
                if (!object.Equals(_powerMeterInstantaneous, value))
                {
                    _powerMeterInstantaneous = value;
                    InvokeAsync(() => { StateHasChanged(); });
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            powerMeterInstantaneous = (await IoTSuiteService.GetPowerMeter(Id)).Data;
        }

        protected async Task FormSubmit(PowerMeterInstantaneous args)
        {
            try
            {
                await IoTSuiteService.UpdatePowerMeter(args);
                DialogService.Close(args);
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Unable to update PowerMeter");
            }
        }

        protected async Task CloseButtonClick(MouseEventArgs args)
        {
            DialogService.Close();
        }
    }
}