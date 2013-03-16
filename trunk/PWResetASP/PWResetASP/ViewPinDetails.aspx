<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPinDetails.aspx.cs"
    Inherits="PWResetASP.ViewPinDetails" Title="View PIN Details" MasterPageFile="~/Site.master"%>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Panel ID="pnl1" runat="server" DefaultButton="ButtonGoBack">
    <h2>
        View PIN Details</h2>
    <p>
        <asp:Label ID="LabelUserInfo" runat="server"></asp:Label>
    </p>
    <p>
        <asp:Label ID="LabelPinDetails" runat="server"></asp:Label>
    </p>
    <p>
        <asp:Button ID="ButtonGoBack" runat="server" Text="&lt;&lt; Return" />
    </p>
    </asp:Panel>
</asp:Content>
