using GoogleMapsComponents;
using GoogleMapsComponents.Maps;
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

namespace IoTSuite.Client.Pages.Locations
{
    public partial class LocationsComponent : ComponentBase
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
        public RadzenGrid<Position> dataGrid;
        public IEnumerable<Position> data;

        public IEnumerable<Position> mapData;
        public DateTime dateTimeFrom = DateTime.Now.AddMonths(-2);
        public DateTime dateTimeTo = DateTime.Now;

        public List<RadzenGoogleMapMarker> locationsMarkers;
        public GoogleMap map;
        public MapOptions mapOptions = new MapOptions()
        {
            Zoom = 13,
            Center = new LatLngLiteral()
            {
                Lat = 0,
                Lng = 0
            },
            MapTypeId = MapTypeId.Roadmap
        };

        private Marker marker;
        private List<LatLngLiteral> path = new List<LatLngLiteral>();
        private List<Polyline> polylines = new List<Polyline>();
        private Polyline polyline;

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
            paginationFilter.Type = ThingType.Vehicle;

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

            hubConnection.On<Position>("PositionNotification", async (value) =>
            {
                if (value.ThingId.Equals(thingId) & mapData.Select(position => position.Id).Max() >= value.Id - 5)
                {
                    value.Date = value.Date.ToLocalTime();
                    mapData = mapData.Append(value).OrderByDescending(position => position.Date);

                    await map.InteropObject.SetCenter(new LatLngLiteral
                    {
                        Lat = value.Latitude,
                        Lng = value.Longitude
                    });

                    await marker.SetPosition(new LatLngLiteral
                    {
                        Lat = value.Latitude,
                        Lng = value.Longitude
                    });

                    path.Add(new LatLngLiteral
                    {
                        Lat = value.Latitude,
                        Lng = value.Longitude
                    });

                    await polyline.SetPath(path);

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

            await map.InteropObject.SetCenter(new LatLngLiteral
            {
                Lat = 0,
                Lng = 0
            });

            marker = await Marker.CreateAsync(map.JsRuntime, new MarkerOptions()
            {
                Position = await map.InteropObject.GetCenter(),
                Map = map.InteropObject
            });

            polyline = await Polyline.CreateAsync(map.JsRuntime, new PolylineOptions()
            {
                Draggable = false,
                Editable = false,
                Map = map.InteropObject,
                Geodesic = true
            });

            await UpdateMapData();

        }

        public async Task UpdateMapData()
        {
            PositionPaginationFilter paginationFilter = new PositionPaginationFilter()
            {
                MinDate = dateTimeFrom.ToUniversalTime(),
                MaxDate = dateTimeTo.ToUniversalTime(),
                ThingId = thingId,
                Order = Order.Asc,
                OrderBy = OrderByPosition.Date,
                PageSize = int.MaxValue
            };
            var result = await IoTSuiteService.GetPositions(paginationFilter);

            mapData = result.Data;

            await map.InteropObject.SetCenter(new LatLngLiteral
            {
                Lat = mapData.First().Latitude,
                Lng = mapData.First().Longitude
            });

            await marker.SetPosition(new LatLngLiteral
            {
                Lat = mapData.First().Latitude,
                Lng = mapData.First().Longitude
            });

            path.Clear();

            path.AddRange(mapData.Select(location => new LatLngLiteral
            {
                Lat = location.Latitude,
                Lng = location.Longitude
            }));

            await polyline.SetPath(path);

            await InvokeAsync(StateHasChanged);
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

            PositionPaginationFilter paginationFilter = new PositionPaginationFilter();

            var page = ((double)args.Skip / (double)args.Top);
            int roundedPage = Convert.ToInt32(Math.Ceiling(page)) + 1;

            paginationFilter.PageSize = args.Top.Value;
            paginationFilter.PageNumber = roundedPage;
            paginationFilter.ThingId = thingId;

            if (!string.IsNullOrEmpty(args.OrderBy))
            {
                var OrderByArray = args.OrderBy.Split(" ");
                paginationFilter.Order = (Order)Enum.Parse(typeof(Order), OrderByArray[1]);
                paginationFilter.OrderBy = (OrderByPosition)Enum.Parse(typeof(OrderByPosition), OrderByArray[0]);
            }
            else
            {
                paginationFilter.Order = Order.Desc;
                paginationFilter.OrderBy = OrderByPosition.Date;
            }

            var result = await IoTSuiteService.GetPositions(paginationFilter);

            data = result.Data;
            dataCount = result.TotalRecords;

            await InvokeAsync(StateHasChanged);
        }

        protected async Task RowSelect(Position args)
        {
            var dialogResult = await DialogService.OpenAsync<EditLocation>("Edit Position", new Dictionary<string, object>() { { "Id", args.Id } }, new DialogOptions() { ShowClose = false });
            await dataGrid.Reload();

            await InvokeAsync(() => { StateHasChanged(); });
        }

        protected async Task DeleteButtonClick(MouseEventArgs args, dynamic data)
        {
            try
            {
                await IoTSuiteService.DeletePosition(data.Id);
                await dataGrid.Reload();
            }
            catch
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Unable to delete");
            }
        }
    }
}
