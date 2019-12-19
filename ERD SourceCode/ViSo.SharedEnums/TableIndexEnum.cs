using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViSo.SharedEnums
{
  public enum TableIndexEnum
  {
    [Description("Not Selected")]
    None = 0,

    [Description("Company")]
    Con_Companies = 1,

    [Description("Company Office")]
    Con_CompanyOffice = 2,

    [Description("Contacts")]
    Con_Contacts = 3,

    [Description("Sales Order Master")]
    Sor_Master = 4,

    [Description("System Company")]
    Ctl_CompanyControl = 5,

    [Description("Inventory Master")]
    Inv_Master = 6,

    [Description("Sales Order Invoice Line")]
    Sor_InvoiceLines = 7,

    [Description("Sales Order Invoice Payment")]
    Sor_InvoicePayments = 8,

    [Description("Purchase Order Invoice Line")]
    Por_InvoiceLines = 9,

    [Description("Purchase Order Invoice Header")]
    Por_InvoiceHeader = 10,

    [Description("Purchase Order Invoice Payments")]
    Por_InvoicePayments = 11,

    [Description("Sales Order Invoice")]
    Sor_Invoice = 12,

    [Description("Purchase Order Line")]
    Por_Line = 13,

    [Description("Sales Order Line")]
    Sor_Detail = 14
  }
}
