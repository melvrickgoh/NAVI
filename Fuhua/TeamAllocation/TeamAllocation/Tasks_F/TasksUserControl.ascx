<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TasksUserControl.ascx.cs" Inherits="TeamAllocation.Tasks.TasksUserControl" %>
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
            <asp:DropDownList ID="DropDownListF" runat="server" AutoPostBack="True" 
                onselectedindexchanged="DropDownListF_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:Label ID="LabelF1" runat="server" ForeColor="Red" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">
            &nbsp;</td>
        <td>
            <asp:GridView ID="GridViewF" runat="server" EnableModelValidation="True" 
                onselectedindexchanged="GridViewF_SelectedIndexChanged" Width="771px" 
                AutoGenerateSelectButton="True">
                
            </asp:GridView>
        </td>
    </tr>
</table>
<p>
    &nbsp;</p>
<p>
    <asp:Label ID="LabelF2" runat="server"></asp:Label>
</p>
<p style="width: 810px">
    Do you want to approve it?</p>
<table class="style1">
    <tr>
        <td class="style3">
            <asp:RadioButton ID="RadioButtonF1" runat="server" Text="YES" />
            <asp:RadioButton ID="RadioButtonF2" runat="server" Text="NO" />
        </td>
        <td>
            <asp:Button ID="GOF" runat="server" onclick="ButtonS_Click" Text="GO" />
        </td>
    </tr>
</table>

