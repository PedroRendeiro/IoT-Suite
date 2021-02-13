using IoTSuite.Client.Services;
using IoTSuite.Shared;
using IoTSuite.Shared.Filters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTSuite.Client.Pages.Measures
{
    public partial class MeasuresComponent : ComponentBase
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
        public RadzenGrid<Measure> dataGrid;
        public IEnumerable<Measure> data;

        public IEnumerable<Measure> chartData;
        public DateTime dateTimeFrom = DateTime.Now.AddHours(-1);
        public DateTime dateTimeTo = DateTime.Now;

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
            paginationFilter.Type = ThingType.EnergyMonitor;

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

            hubConnection.On<Measure>("MeasureNotification", async (value) =>
            {
                if (value.ThingId.Equals(thingId))
                {
                    value.Date = value.Date.ToLocalTime();
                    chartData = chartData.Append(value).OrderByDescending(measure => measure.Date);

                    await InvokeAsync(StateHasChanged);
                }
            });

            hubConnection.Closed += Test;

            await hubConnection.StartAsync();
        }

        private async Task Test(Exception exception)
        {
            NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Connection to server lost");
        }

        public async Task ChangeThingsDropDown(object args)
        {
            thingId = Guid.Parse(args.ToString());

            if (dataGrid != null)
            {
                await UpdateGridData(new LoadDataArgs
                {
                    OrderBy = "Id Desc",
                    Skip = dataGrid.CurrentPage * dataGrid.PageSize,
                    Top = dataGrid.PageSize
                });

                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await UpdateGridData(new LoadDataArgs
                {
                    OrderBy = "Id Desc",
                    Skip = 0,
                    Top = RecordsPerPage.First()
                });
            }

            await UpdateChartData();

        }

        public async Task UpdateChartData()
        {
            MeasurePaginationFilter paginationFilter = new MeasurePaginationFilter()
            {
                MinDate = dateTimeFrom.ToUniversalTime(),
                MaxDate = dateTimeTo.ToUniversalTime(),
                ThingId = thingId,
                Order = Order.Asc,
                OrderBy = OrderByMeasure.Date,
                PageSize = int.MaxValue
            };
            var result = await IoTSuiteService.GetMeasures(paginationFilter);

            chartData = result.Data;
        }

        public void OnInvalidSubmit()
        {

        }

        public async Task ChangeRecordsDropDown(object args)
        {
            await dataGrid.Reload();
        }

        public async Task UpdateGridData(LoadDataArgs args)
        {
            Console.WriteLine(args.Filter);

            MeasurePaginationFilter paginationFilter = new MeasurePaginationFilter();

            var page = ((double)args.Skip / (double)args.Top);
            int roundedPage = Convert.ToInt32(Math.Ceiling(page)) + 1;

            paginationFilter.PageSize = args.Top.Value;
            paginationFilter.PageNumber = roundedPage;
            paginationFilter.ThingId = thingId;

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                var OrderByArray = args.OrderBy.Split(" ");
                paginationFilter.Order = (Order)Enum.Parse(typeof(Order), OrderByArray[1]);
                paginationFilter.OrderBy = (OrderByMeasure)Enum.Parse(typeof(OrderByMeasure), OrderByArray[0]);
            }
            else
            {
                paginationFilter.Order = Order.Desc;
                paginationFilter.OrderBy = OrderByMeasure.Date;
            }

            var result = await IoTSuiteService.GetMeasures(paginationFilter);

            data = result.Data;
            dataCount = result.TotalRecords;

            await InvokeAsync(StateHasChanged);
        }

        protected async Task RowSelect(Measure args)
        {
            var dialogResult = await DialogService.OpenAsync<EditMeasure>("Edit Measure", new Dictionary<string, object>() { { "Id", args.Id } }, new DialogOptions() { ShowClose = false });
            await dataGrid.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async Task DeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                await IoTSuiteService.DeleteMeasure(data.Id);
                await dataGrid.Reload();
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Unable to delete");
            }
        }
    }
}
