<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HRUserControl.ascx.cs" Inherits="TeamAllocation.HR.HRUserControl" %>
<style type="text/css">
    .style1
    {
        width: 100%;
    }
    .style2
    {
    }
    .style3
    {
        height: 33px;
    }
    .style5
    {
        width: 1153px;
    }
    .style6
    {
        width: 1063px;
    }
</style>
<table class="style1">
    <tr>
        <td class="style2" colspan="2">
            Arrival Time of Incoming Ships:
            <br />
            <br />
&nbsp;<asp:DropDownList ID="DropDownList_docking_time" runat="server" AutoPostBack="True" 
                onselectedindexchanged="DropDownList1_SelectedIndexChanged">
            </asp:DropDownList>
            <br />
            <br />
            <asp:Label ID="Label_dropdown_errormsg" runat="server" ForeColor="Red" Text="Label"></asp:Label>
            <br />
        </td>
    </tr>
    <tr>
        <td class="style2">
            &nbsp;</td>
        <td>
            <asp:GridView ID="GridView_team_allocation" runat="server" EnableModelValidation="True" 
                onselectedindexchanged="GridView1_SelectedIndexChanged" Width="771px" 
                AutoGenerateSelectButton="True">
                
            </asp:GridView>
        </td>
    </tr>
    </table>
<p>
</p>
<table class="style1">
    <tr>
        <td class="style3" colspan="4">
            Available People at the Time</td>
    </tr>
    <tr>
        <td class="style6">
            <br />
            F:<asp:RadioButtonList ID="RadioButtonList_finance_name" runat="server">
            </asp:RadioButtonList>
            <br />
            </td>
        <td class="style5">
            O:<asp:RadioButtonList ID="RadioButtonList_operations_name" runat="server">
            </asp:RadioButtonList>
            </td>
        <td class="style5">
            S:<asp:RadioButtonList ID="RadioButtonList_safety_name" runat="server">
            </asp:RadioButtonList>
            </td>
        <td class="style2">
            &nbsp;</td>
    </tr>
</table>


<p>
            <asp:Button ID="AssignTeam_confirmation" runat="server" Text="AssignTeam" 
                onclick="AssignTeam_Click" />
        </p>



