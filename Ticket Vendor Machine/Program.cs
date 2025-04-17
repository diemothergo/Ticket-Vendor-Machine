using System;
using Ticket_Vendor_Machine;
using System.Windows.Forms;
using System.Configuration;
namespace TicketVendorApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TicketForm());
        }
    }
}
