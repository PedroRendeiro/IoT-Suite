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

namespace IoTSuite.Client.Pages.PowerMeter
{
    public partial class PowerMeterComponent : ComponentBase
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
        public RadzenGrid<PowerMeterInstantaneous> dataGrid;
        public IEnumerable<PowerMeterInstantaneous> data;

        public IEnumerable<PowerMeterInstantaneous> chartData;

        public RadzenDropDown<Guid> thingsDropDown;
        public List<Thing> things;
        public Guid thingId;
        public string thingsPlaceholder = "Loading...";

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
            ThingPaginationFilter paginationFilter = new ThingPaginationFilter();
            paginationFilter.Type = ThingType.PowerMeter;

            var result = await IoTSuiteService.GetThings(paginationFilter);
            things = result.Data;
            thingsPlaceholder = "Choose a Thing";
            
            await InvokeAsync(StateHasChanged);

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

            hubConnection.On<PowerMeterInstantaneous>("PowerMeterInstantaneousNotification", async (value) =>
            {
                if (value.ThingId.Equals(thingId))
                {
                    value.Date = value.Date.ToLocalTime();
                    chartData = chartData.Append(value).OrderByDescending(measure => measure.Date);

                    await InvokeAsync(StateHasChanged);
                }
            });

            await hubConnection.StartAsync();
        }

        public async Task ChangeThingsDropDown(object args)
        {
            thingId = Guid.Parse(args.ToString());

            if (dataGrid != null)
            {
                await LoadData(new LoadDataArgs
                {
                    OrderBy = "Id Desc",
                    Skip = dataGrid.CurrentPage * dataGrid.PageSize,
                    Top = dataGrid.PageSize
                });

                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await LoadData(new LoadDataArgs
                {
                    OrderBy = "Id Desc",
                    Skip = 0,
                    Top = RecordsPerPage.First()
                });
            }

            PowerMeterInstantaneousPaginationFilter paginationFilter = new PowerMeterInstantaneousPaginationFilter()
            {
                MinDate = DateTime.UtcNow.AddHours(-1),
                ThingId = thingId,
                Order = Order.Desc,
                OrderBy = OrderByPowerMeterInstantaneous.Date,
                PageSize = int.MaxValue
            };
            var result = await IoTSuiteService.GetPowerMeters(paginationFilter);

            chartData = result.Data;

        }

        public async Task ChangeRecordsDropDown(object args)
        {
            await dataGrid.Reload();
        }

        public async Task LoadData(LoadDataArgs args)
        {
            Console.WriteLine(args.Filter);

            PowerMeterInstantaneousPaginationFilter paginationFilter = new PowerMeterInstantaneousPaginationFilter();

            var page = ((double)args.Skip / (double)args.Top);
            int roundedPage = Convert.ToInt32(Math.Ceiling(page)) + 1;

            paginationFilter.PageSize = args.Top.Value;
            paginationFilter.PageNumber = roundedPage;
            paginationFilter.ThingId = thingId;

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                var OrderByArray = args.OrderBy.Split(" ");
                paginationFilter.Order = (Order)Enum.Parse(typeof(Order), OrderByArray[1]);
                paginationFilter.OrderBy = (OrderByPowerMeterInstantaneous)Enum.Parse(typeof(OrderByPowerMeterInstantaneous), OrderByArray[0]);
            }
            else
            {
                paginationFilter.Order = Order.Desc;
                paginationFilter.OrderBy = OrderByPowerMeterInstantaneous.Date;
            }

            var result = await IoTSuiteService.GetPowerMeters(paginationFilter);

            data = result.Data;
            dataCount = result.TotalRecords;

            await InvokeAsync(StateHasChanged);
        }

        protected async Task RowSelect(PowerMeterInstantaneous args)
        {
            var dialogResult = await DialogService.OpenAsync<EditPowerMeter>("Edit PowerMeter", new Dictionary<string, object>() { { "Id", args.Id } }, new DialogOptions() { ShowClose = false });
            await dataGrid.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async Task DeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                await IoTSuiteService.DeletePowerMeter(data.Id);
                await dataGrid.Reload();
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Unable to delete");
            }
        }
    }
}
