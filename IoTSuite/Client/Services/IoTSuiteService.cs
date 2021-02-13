using IoTSuite.Shared;
using IoTSuite.Shared.Filters;
using IoTSuite.Shared.Wrappers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IoTSuite.Client.Services
{
    public class IoTSuiteService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public IoTSuiteService(NavigationManager navigationManager, HttpClient httpClient)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}api/");

            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<PagedResponse<List<Thing>>> GetThings(ThingPaginationFilter paginationFilter)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<PagedResponse<List<Thing>>>($"Things?{paginationFilter.ToQueryString()}", jsonSerializerOptions);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return null;
            }
        }

        public async Task<PagedResponse<Thing>> GetThing(Guid Id)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<PagedResponse<Thing>>($"Things/{Id}", jsonSerializerOptions);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return null;
            }
        }

        public async Task UpdateThing(Thing thing)
        {
            try
            {
                await httpClient.PutAsJsonAsync($"Things/{thing.ThingId}", thing, jsonSerializerOptions);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public async Task DeleteThing(Guid Id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"Things/{Id}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception();
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public async Task<PagedResponse<List<Measure>>> GetMeasures(MeasurePaginationFilter paginationFilter)
        {
            try
            {
                var data = await httpClient.GetFromJsonAsync<PagedResponse<List<Measure>>>($"Measures?{paginationFilter.ToQueryString()}", jsonSerializerOptions);
                data.Data.ForEach(value =>
                {
                    value.Date = value.Date.ToLocalTime();
                });

                return data;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return null;
            }
        }

        public async Task<PagedResponse<Measure>> GetMeasure(long Id)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<PagedResponse<Measure>>($"Measures/{Id}", jsonSerializerOptions);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return null;
            }
        }

        public async Task UpdateMeasure(Measure measure)
        {
            try
            {
                await httpClient.PutAsJsonAsync($"Measures/{measure.Id}", measure, jsonSerializerOptions);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public async Task DeleteMeasure(long Id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"Measures/{Id}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception();
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public async Task<PagedResponse<List<Position>>> GetPositions(PositionPaginationFilter paginationFilter)
        {
            try
            {
                var data = await httpClient.GetFromJsonAsync<PagedResponse<List<Position>>>($"Positions?{paginationFilter.ToQueryString()}", jsonSerializerOptions);
                data.Data.ForEach(value =>
                {
                    value.Date = value.Date.ToLocalTime();
                });

                return data;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return null;
            }
        }

        public async Task<PagedResponse<Position>> GetPosition(long Id)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<PagedResponse<Position>>($"Positions/{Id}", jsonSerializerOptions);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return null;
            }
        }

        public async Task UpdatePosition(Position position)
        {
            try
            {
                await httpClient.PutAsJsonAsync($"Position/{position.Id}", position, jsonSerializerOptions);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public async Task DeletePosition(long Id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"Position/{Id}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception();
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        public async Task<PagedResponse<List<PowerMeterInstantaneous>>> GetPowerMeters(PowerMeterInstantaneousPaginationFilter paginationFilter)
        {
            var data = await httpClient.GetFromJsonAsync<PagedResponse<List<PowerMeterInstantaneous>>>($"PowerMeter/Instantaneous?{paginationFilter.ToQueryString()}", jsonSerializerOptions);
            data.Data.ForEach(value => value.Date = value.Date.ToLocalTime());

            return data;
        }

        public async Task<PagedResponse<PowerMeterInstantaneous>> GetPowerMeter(long Id)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<PagedResponse<PowerMeterInstantaneous>>($"PowerMeter/Instantaneous/{Id}", jsonSerializerOptions);
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return null;
            }
        }

        public async Task UpdatePowerMeter(PowerMeterInstantaneous powerMeterInstantaneous)
        {
            await httpClient.PutAsJsonAsync($"PowerMeter/Instantaneous/{powerMeterInstantaneous.Id}", powerMeterInstantaneous, jsonSerializerOptions);
        }

        public async Task DeletePowerMeter(long Id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"PowerMeter/Instantaneous/{Id}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception();
                }
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }
    }
}
