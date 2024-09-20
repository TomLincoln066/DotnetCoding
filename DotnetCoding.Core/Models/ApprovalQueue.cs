using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// requirement - 4 create a new class called ApprovalQueue to store the approval requests, along with the reasons for the request (create/update), the request date, and the product information.
namespace DotnetCoding.Core.Models
{
    public class ApprovalQueue
    {
        public int Id { get; set; }
        public int ProductId { get; set; }  // Link to ProductDetails
        public string RequestReason { get; set; }  // Reason (Create, Update)
        public DateTime RequestDate { get; set; }  // Date of request
        public string State { get; set; }  // Pending, Approved, Rejected
    }
}
