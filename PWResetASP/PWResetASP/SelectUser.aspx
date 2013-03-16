<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="SelectUser.aspx.cs" Inherits="PWResetASP.SelectUser" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Panel ID="pnl1" runat="server" DefaultButton="ButtonFindUsers">
    <p>
        <asp:Label ID="labelStatus" runat="server" ForeColor="Red"></asp:Label>
        </p>
    <p>
        <asp:Label ID="labelServerInfo" runat="server"></asp:Label>
        </p>
    <p>
        <asp:Literal ID="Literal1" runat="server" Text="User Selection Options:"></asp:Literal>
        <asp:DropDownList ID="comboUserFilterElement" runat="server">
            <asp:ListItem>{All Users}</asp:ListItem>
            <asp:ListItem Value="Alias"></asp:ListItem>
            <asp:ListItem Value="FirstName">First Name</asp:ListItem>
            <asp:ListItem Value="LastName">Last Name</asp:ListItem>
            <asp:ListItem Value="DisplayName">Display Name</asp:ListItem>
            <asp:ListItem Value="DTMFAccessId">Extension</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="comboUserFilterAction" runat="server">
            <asp:ListItem>StartsWith</asp:ListItem>
            <asp:ListItem>Is</asp:ListItem>
        </asp:DropDownList>
        <asp:TextBox ID="textUserFilterText" runat="server"></asp:TextBox>
        <asp:Button ID="ButtonFindUsers" runat="server" OnClick="ButtonFindUsers_Click" Text="Find Users"
            OnClientClick="this.disabled = true; this.value = 'Working...';" UseSubmitBehavior="false" />
        <asp:Literal ID="Literal2" runat="server" Text="  Users per page:"></asp:Literal>
        <asp:DropDownList ID="comboUsersToFetch" runat="server">
            <asp:ListItem>10</asp:ListItem>
            <asp:ListItem>25</asp:ListItem>
            <asp:ListItem>50</asp:ListItem>
            <asp:ListItem>100</asp:ListItem>
        </asp:DropDownList>
        </p>
        <asp:GridView ID="gridUsers" runat="server" AutoGenerateColumns="False" BackColor="White"
            BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" 
        CellPadding="4" ForeColor="Black"
            GridLines="Vertical" ShowHeaderWhenEmpty="True" 
        OnRowCommand="gridUsers_RowCommand" onrowdatabound="gridUsers_RowDataBound">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:ButtonField Text="Reset Pin" CommandName="ResetPin" />
                <asp:ButtonField Text="View Pin" CommandName="ViewPin" />
                <asp:BoundField DataField="Alias" HeaderText="Alias" ReadOnly="True">
                <ItemStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="FirstName" HeaderText="First Name" ReadOnly="True" />
                <asp:BoundField DataField="LastName" HeaderText="Last Name" ReadOnly="True" />
                <asp:BoundField DataField="DisplayName" HeaderText="Display Name" 
                    ReadOnly="True">
                <ItemStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="DTMFAccessId" HeaderText="Extension" 
                    ReadOnly="True" />
                <asp:BoundField DataField="ObjectId" HeaderText="ObjectId" />
            </Columns>
            <FooterStyle BackColor="#CCCC99" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" 
                HorizontalAlign="Left" Wrap="False" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#FBFBF2" />
            <SortedAscendingHeaderStyle BackColor="#848384" />
            <SortedDescendingCellStyle BackColor="#EAEAD3" />
            <SortedDescendingHeaderStyle BackColor="#575357" />
        </asp:GridView>
    <p>
        <asp:Button ID="buttonPreviousPage" runat="server" 
            onclick="buttonPreviousPage_Click" Text="&lt;&lt;" />
        <asp:Button ID="buttonNextPage" runat="server" 
            onclick="buttonNextPage_Click" Text="&gt;&gt;" />
        <asp:Label ID="LabelUserCountValue" runat="server" Text="Label"></asp:Label>
    </p>
    </asp:Panel>
    </asp:Content>

    