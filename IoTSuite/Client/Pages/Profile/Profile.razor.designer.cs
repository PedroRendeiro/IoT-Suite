using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Microsoft.Graph;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Client;
using Microsoft.Graph.Auth;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;

namespace IoTSuite.Pages
{
    public partial class ProfileComponent : ComponentBase
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
        protected IAccessTokenProvider TokenProvider { get; set; }

        [Inject]
        protected IHttpClientFactory ClientFactory { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        User _user;
        
        private HubConnection hubConnection;

        protected User user
        {
            get
            {
                return _user;
            }
            set
            {
                if (!object.Equals(_user, value))
                {
                    _user = value;
                    InvokeAsync(() => { StateHasChanged(); });
                }
            }
        }
        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
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

            hubConnection.On<User>("GraphAPIGetUser", (user) =>
            {
                _user = user;
                StateHasChanged();
            });

            await hubConnection.StartAsync();

            await hubConnection.SendAsync("GraphAPIGetUser");
        }

        protected async System.Threading.Tasks.Task TemplateForm0Submit(User args)
        {
            
        }

        public void ChangePassword(MouseEventArgs args)
        {
            var resetPasswordFlow = "B2C_1_PasswordReset";
            var clientId = "226df5b5-6a59-4324-a98d-1623a084e1b7";
            var redirectUri = $"{UriHelper.BaseUri}authentication/login-callback";

            var userFlowBuilder = $@"https://pedrorendeiro.b2clogin.com/pedrorendeiro.eu/oauth2/v2.0/authorize
                        ?p={resetPasswordFlow}
                        &client_id={clientId}
                        &redirect_uri={redirectUri}
                        &nonce=defaultNonce
                        &scope=openid
                        &response_type=id_token
                        &prompt=login";

            var userFlowUrl = String.Concat(userFlowBuilder.Where(c => !Char.IsWhiteSpace(c)));
            UriHelper.NavigateTo(userFlowUrl, forceLoad: true);
        }

        protected async System.Threading.Tasks.Task Button1Click(MouseEventArgs args)
        {
            DialogService.Close();
            await JSRuntime.InvokeAsync<string>("window.history.back");
        }
    }
}