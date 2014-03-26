<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductSchedulerUserControl.ascx.cs" Inherits="ProductScheduler.ProductScheduler.ProductSchedulerUserControl" %>
<style type="text/css">
    .style1
    {
        width: 100%;
    }
    .style2
    {
        height: 23px;
    }
</style>

<table class="style1">
    <tr>
        <td class="style2">
            <asp:Label ID="lblDockingTime" runat="server" Text="Docking Time"></asp:Label>
&nbsp;&nbsp;
            <asp:DropDownList ID="ddlDockingTime" runat="server" AutoPostBack="True" 
                onselectedindexchanged="ddlDockingTime_SelectedIndexChanged1">
            </asp:DropDownList>
&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnScheduleProduct" runat="server" 
                onclick="btnScheduleProduct_Click" Text="Schedule Products to Selected Ships" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
            <br />
            <asp:GridView ID="displayGV" runat="server" CellPadding="4" 
                EnableModelValidation="True" ForeColor="#333333" >
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkHeader" runat="server" AutoPostBack="True" 
                                oncheckedchanged="chkHeader_CheckedChanged1" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkShip" runat="server" AutoPostBack="True" 
                                oncheckedchanged="chkShip_CheckedChanged" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            </asp:GridView>
        </td>
    </tr>
</table>

