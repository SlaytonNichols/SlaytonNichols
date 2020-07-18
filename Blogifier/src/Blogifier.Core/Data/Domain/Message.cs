using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Blogifier.Core.Data
{
    public class Message
    {
        public Message() { }
        public int Id { get; set; }
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
        public DateTime Sent { get; set; }
        public DateTime Read { get; set; }
        public DateTime Received { get; set; }
        public bool IsDeleted { get; set; }

    }
}
