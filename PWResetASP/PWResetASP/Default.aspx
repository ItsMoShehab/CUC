<%@ Page Title="Log In" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" 
    CodeBehind="Default.aspx.cs" Inherits="PWResetASP.Account.Login" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">

        .style1
        {
            width: 101px;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        log into unity connection</h2>
<p>
    <asp:Label ID="LabelStatus" runat="server" ForeColor="Red"></asp:Label>
</p>
    <asp:Panel ID="pnl1" runat="server" DefaultButton="ButtonLogin">
 
        <table style="width: 100%;">
    <tr>
        <td align="right" class="style1">
            <asp:Label ID="LabelServerName" runat="server" Text="Server Name:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="TextBoxServerName" runat="server" CausesValidation="True" 
                MaxLength="80" Width="176px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td align="right" class="style1">
            <asp:Label ID="LabelName" runat="server" Text="Login Name:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="TextBoxName" runat="server" CausesValidation="True" 
                MaxLength="80" Width="176px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td align="right" class="style1">
            <asp:Label ID="LabelPassword" runat="server" Text="Password:" 
                text-align="right"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="TextBoxPassword" runat="server" CausesValidation="True" 
                MaxLength="80" TextMode="Password" Width="176px"></asp:TextBox>
        </td>
    </tr>
</table>
<p>
    <asp:Button ID="ButtonLogin" runat="server" OnClientClick="this.disabled = true; this.value = 'Verifying...';"
        UseSubmitBehavior="false" OnClick="ButtonLogin_Click" Text="Login" />
</p>
</asp:Panel>
</asp:Content>
