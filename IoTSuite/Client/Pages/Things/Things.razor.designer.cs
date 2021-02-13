using IoTSuite.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using IoTSuite.Shared;
using System.Net.Http;
using Radzen.Blazor;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using IoTSuite.Shared.Filters;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;

namespace IoTSuite.Client.Pages.Things
{
    public partial class ThingsComponent : ComponentBase
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

        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected IAccessTokenProvider TokenProvider { get; set; }

        public int dataCount;
        public RadzenGrid<Thing> dataGrid;
        public IEnumerable<Thing> data;

        public static int[] RecordsPerPage = new int[]
        {
            12,
            24,
            48
        };
        public int PageSize = RecordsPerPage.First();

        private HubConnection hubConnection;

        protected override async Task OnInitializedAsync()
        {
            await LoadData(new LoadDataArgs
            {
                OrderBy = "ThingId Asc",
                Skip = 0,
                Top = RecordsPerPage.First()
            });
            
            hubConnection = new HubConnectionBuilder()
            .WithUrl(UriHelper.ToAbsoluteUri("/Hub/SignalR"), options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    var tokenResult = await TokenProvider.RequestAccessToken();

                    if (tokenResult.TryGetToken(out var token))
                    {
                        return await Task.FromResult(token.Value);
                    }
                    else
                    {
                        return await Task.FromResult(string.Empty);
                    }
                };

                options.Transports = HttpTransportType.LongPolling;
            })
            .WithAutomaticReconnect()
            .Build();

            await hubConnection.StartAsync();
        }

        public async Task ChangeRecordsDropDown(object args)
        {
            await dataGrid.Reload();
        }

        public async Task LoadData(LoadDataArgs args)
        {
            ThingPaginationFilter paginationFilter = new ThingPaginationFilter();

            var page = ((double)args.Skip / (double)args.Top);
            int roundedPage = Convert.ToInt32(Math.Ceiling(page)) + 1;

            paginationFilter.PageSize = args.Top.Value;
            paginationFilter.PageNumber = roundedPage;

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                var OrderByArray = args.OrderBy.Split(" ");
                paginationFilter.Order = (Order)Enum.Parse(typeof(Order), OrderByArray[1]);
                paginationFilter.OrderBy = (OrderByThing)Enum.Parse(typeof(OrderByThing), OrderByArray[0]);
            }
            else
            {
                paginationFilter.Order = Order.Desc;
                paginationFilter.OrderBy = OrderByThing.ThingId;
            }

            var result = await IoTSuiteService.GetThings(paginationFilter);

            data = result.Data;
            dataCount = result.TotalRecords;

            await InvokeAsync(StateHasChanged);
        }

        protected async Task RowSelect(Thing args)
        {
            var dialogResult = await DialogService.OpenAsync<EditThing>("Edit Thing", new Dictionary<string, object>() { { "Id", args.ThingId } }, new DialogOptions() { ShowClose = false });
            await dataGrid.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async Task DeleteButtonClick(MouseEventArgs args, Thing thing)
        {
            try
            {
                await IoTSuiteService.DeleteThing(thing.ThingId);
                await dataGrid.Reload();
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Unable to delete thing with Id {thing.ThingId}");
                return;
            }

            NotificationService.Notify(NotificationSeverity.Success, $"Success", $"Successfully deleted thing with Id {thing.ThingId}");
        }

        protected async Task ThingButtonClick(ThingActions thingAction, Thing thing)
        {            
            try
            {
                await hubConnection.InvokeAsync("SendMessageToThing", thing.ThingId, thingAction);
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Unable to {thingAction}");
                return;
            }

            NotificationService.Notify(NotificationSeverity.Success, $"Success", $"Message sent to thing with Id {thing.ThingId}");
        }
    }
}
