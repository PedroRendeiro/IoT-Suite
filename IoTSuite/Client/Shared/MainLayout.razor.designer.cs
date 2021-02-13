using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace IoTSuite.Layouts
{
    public partial class MainLayoutComponent : LayoutComponentBase
    {
        [Inject]
        protected NavigationManager UriHelper { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SignOutSessionStateManager SignOutManager { get; set; }

        protected RadzenBody body0;
        protected RadzenSidebar sidebar0;

        protected async Task SidebarToggle0Click(dynamic args)
        {
            await InvokeAsync(() => { sidebar0.Toggle(); });
            await InvokeAsync(() => { body0.Toggle(); });
        }

        protected async Task Profilemenu0Click(dynamic args)
        {
            if (args.Value == "Logout")
            {
                await SignOutManager.SetSignOutState();
                UriHelper.NavigateTo("authentication/logout");
            }
            else if (args.Value == "Login")
            {
                UriHelper.NavigateTo("authentication/login");
            }
        }
    }
}
