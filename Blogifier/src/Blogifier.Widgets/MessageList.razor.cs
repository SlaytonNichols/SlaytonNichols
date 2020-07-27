using Blogifier.Core.Services;
using Microsoft.AspNetCore.Components;
using Sotsera.Blazor.Toaster;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Microsoft.Extensions.Configuration;
using Blogifier.Core;
using Microsoft.FeatureManagement;
using Blogifier.Core.Data;
using System.Collections.Generic;

namespace Blogifier.Widgets
{
    public partial class MessageList : ComponentBase
    {
        protected List<Message> Model { get; set; }

        protected override void OnInitialized()
        {
            Model = new List<Message>();

            
        }
    }
}
