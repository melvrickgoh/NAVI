<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Task_OUserControl.ascx.cs" Inherits="TeamAllocation.Task_O.Task_OUserControl" %>
<style type="text/css">
    .style1
    {
        width: 149%;
    }
    .style2
    {
        width: 95px;
    }
    .style3
    {
        width: 168px;
    }
</style>

<table class="style1">
    <tr>
        <td class="style2">
            Who are you?</td>
        <td>
            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" 
                onselectedindexchanged="DropDownList1_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:Label ID="Label1" runat="server" ForeColor="Red" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">
            &nbsp;</td>
        <td>
            <asp:GridView ID="GridView1" runat="server" EnableModelValidation="True" 
                onselectedindexchanged="GridView1_SelectedIndexChanged" Width="771px" 
                AutoGenerateSelectButton="True">
                
            </asp:GridView>
        </td>
    </tr>
</table>
<p>
    &nbsp;</p>
<p>
    <asp:Label ID="Label2" runat="server"></asp:Label>
</p>
<p style="width: 810px">
    Do you want to approve it?</p>
<table class="style1">
    <tr>
        <td class="style3">
            <asp:RadioButton ID="RadioButton1" runat="server" Text="YES" 
            GroupName="groupO"/>
            <asp:RadioButton ID="RadioButton2" runat="server" Text="NO" 
            GroupName="groupO"/>
        </td>
        <td>
            <asp:Button ID="GO" runat="server" onclick="Button1_Click" Text="GO" />
        </td>
    </tr>
</table>

