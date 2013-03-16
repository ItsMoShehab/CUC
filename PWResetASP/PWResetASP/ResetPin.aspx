<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResetPin.aspx.cs" Inherits="PWResetASP.ResetPin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 85px;
        }
        .style3
        {
            width: 85px;
            height: 21px;
        }
        .style4
        {
            height: 21px;
        }
        .style5
        {
            width: 85px;
            height: 24px;
        }
        .style6
        {
            height: 24px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnl1" runat="server" DefaultButton="buttonOK">
    <h2>
        Reset User PIN</h2>
    <p>
        <asp:Label ID="LabelStatus" runat="server" ForeColor="Red"></asp:Label>
    <p>
        <asp:Label ID="LabelUserInfo" runat="server"></asp:Label>
    <table class="style1">
        <tr>
            <td align="right" class="style3">
                <asp:Literal ID="Literal1" runat="server" Text="New PIN:"></asp:Literal>
            </td>
            <td class="style4">
                <asp:TextBox ID="TextBoxNewPin" runat="server" MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right" class="style2">
                <asp:Literal ID="Literal2" runat="server" Text="Verify:"></asp:Literal>
            </td>
            <td>
                <asp:TextBox ID="TextBoxVerifyNewPin" runat="server" MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right" class="style2">
                &nbsp;</td>
            <td>
                <asp:CheckBox ID="checkMustChange" runat="server" 
                    Text="User must change at next login" />
            </td>
        </tr>
        <tr>
            <td align="right" class="style5">
            </td>
            <td class="style6">
                <asp:CheckBox ID="checkDoesNotExpire" runat="server" Text="Does not expire" />
            </td>
        </tr>
        <tr>
            <td align="right" class="style2">
                &nbsp;</td>
            <td>
                <asp:CheckBox ID="checkClearHackedLockout" runat="server" 
                    Text="Clear hacked lockout" />
            </td>
        </tr>
        <tr>
            <td align="right" class="style2">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td align="right" class="style2">
                &nbsp;</td>
            <td>
                <asp:Button ID="buttonOK" runat="server" OnClick="buttonOK_Click" Text="Reset" OnClientClick="this.disabled = true; this.value = 'Working...';"
                    UseSubmitBehavior="false" />
                <asp:Button ID="buttonCancel" runat="server" ForeColor="Red" 
                    onclick="buttonCancel_Click" Text="Cancel" />
            </td>
        </tr>
    </table>
    </asp:Panel>
</asp:Content>
