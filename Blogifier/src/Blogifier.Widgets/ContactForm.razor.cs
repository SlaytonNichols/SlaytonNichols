using Blogifier.Core.Services;
using Microsoft.AspNetCore.Components;
using Sotsera.Blazor.Toaster;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Microsoft.Extensions.Configuration;
using System;
using Blogifier.Core.Data;

namespace Blogifier.Widgets
{
    public partial class ContactForm : ComponentBase
    {
        [Inject]
        protected IConfiguration Configuration { get; set; }
        [Inject]
        protected IJsonStringLocalizer<ContactForm> Localizer { get; set; }
        [Inject]
        protected IToaster Toaster { get; set; }

        [Inject]
        protected IDataService DataService { get; set; }

        protected ContactModel Contact { get; set; }

        protected override void OnInitialized()
        {
            Contact = new ContactModel { };            
        }

        protected async Task Send()
        {
            try
            {
                //TODO: Automapper maybe?
                var Message = new Message
                {
                    FirstName = Contact.FirstName,
                    LastName = Contact.LastName,
                    MessageSubject = Contact.MessageSubject,
                    MessageBody = Contact.MessageBody,
                    EmailAddress = Contact.EmailAddress,
                    PhoneNumber = Contact.PhoneNumber
                };

                await DataService.Message.InsertMessage(Message);

                Toaster.Success("Saved");
            }
            catch (Exception ex)
            {
                Toaster.Error(ex.Message);
            }            
        }
    }

    public  class ContactModel
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; }
        [Required, StringLength(50)]
        public string LastName { get; set; }
        [Required, EmailAddress, StringLength(100)]
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        [Required, StringLength(100)]
        public string MessageSubject { get; set; }
        [Required, StringLength(1000)]
        public string MessageBody { get; set; }
    }
}
