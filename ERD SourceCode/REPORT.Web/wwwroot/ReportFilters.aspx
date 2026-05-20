<%@ Page Title="Report Filters" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"  CodeBehind="ReportFilters.aspx.cs" Inherits="REPORT.Web.ReportFilters" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <p class="ViSo-Text-Large">ViSo-Nice Reports Filters</p>
        <p runat="server" class="ViSo-Text" id="uxReportName"></p>
    </div>

    <div runat="server" id="uxFilters" >
        
    </div>
</asp:Content>
