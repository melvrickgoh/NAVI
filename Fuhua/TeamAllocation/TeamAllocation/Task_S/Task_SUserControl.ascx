﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Task_SUserControl.ascx.cs" Inherits="TeamAllocation.Task_S.Task_SUserControl" %>
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
            <asp:DropDownList ID="DropDownList_safety_name" runat="server" AutoPostBack="True" 
                onselectedindexchanged="DropDownListS_SelectedIndexChanged">
            </asp:DropDownList>
            <asp:Label ID="Label_dropdown_errormsg" runat="server" ForeColor="Red" Text="Label"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="style2">
            &nbsp;</td>
        <td>
            <asp:GridView ID="GridView_safety_tasklist" runat="server" EnableModelValidation="True" 
                onselectedindexchanged="GridViewS_SelectedIndexChanged" Width="771px" 
                AutoGenerateSelectButton="True">
                
            </asp:GridView>
        </td>
    </tr>
</table>
<p>
    &nbsp;</p>
<p>
    <asp:Label ID="Label_safety_selected_task" runat="server"></asp:Label>
</p>
<p style="width: 810px">
    Do you want to approve it?</p>
<table class="style1">
    <tr>
        <td class="style3">
            <asp:RadioButton ID="RadioButton_safety_approve" runat="server" Text="YES" 
                GroupName="groupYN" />
            <asp:RadioButton ID="RadioButton_safety_reject" runat="server" Text="NO" 
                GroupName="groupYN" />
        </td>
        <td>
            <asp:Button ID="GO_safety_confirm_selection" runat="server" onclick="ButtonS_Click" Text="GO" />
        </td>
    </tr>
</table>

