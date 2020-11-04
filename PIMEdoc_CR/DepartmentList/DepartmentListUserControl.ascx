<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentListUserControl.ascx.cs" Inherits="PIMEdoc_CR.DepartmentList.DepartmentListUserControl" %>

<style type="text/css">
    .panel-heading {
        color: #01883c;
        padding: 21px 10px;
        font-size: 24px;
        margin-top: 30px;
        margin-left: 30px;
    }

    .pull-left {
        float: left !important;
    }

    .service {
        margin-top: -7px;
        height: 30px;
        width: 140px;
        border-bottom: solid 3px #01883c;
        margin-left: -10px;
        line-height: 20px;
    }

    .panelbodyDepartment {
        background-color: transparent;
        padding: 0px 10px;
        margin-top: 10px;
        margin-left: 30px;
    }
    /*.artDeprtment {
        -webkit-columns: 3 300px !important;
        -moz-columns: 3 300px !important;
        columns: 3 300px !important;
        -webkit-column-gap: 4em !important;
        -moz-column-gap: 4em !important;
        column-gap: 4em !important;
    }*/
    .itemDepartment {
        width: 100%;
        padding-top: 10px;
    }

    @media screen and (min-width: 780px) {
        .itemDepartment {
            width: 25%;
            padding-top: 20px;
        }
    }

    /* unvisited link */
    .DepartmentLink:link {
        color: #078E3C;
    }

    /* visited link */
    .DepartmentLink:visited {
        color: #078E3C;
    }

    /* mouse over link */
    .DepartmentLink:hover {
        color: #078E3C;
        text-decoration: underline;
    }

    /* selected link */
    .DepartmentLink:active {
        color: #078E3C;
    }
</style>
<div class="panel-heading">
    <span class="pull-left service">​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​Departments</span>
</div>
<div class="panelbodyDepartment">
    <div id="panelRow" runat="server">
    </div>
</div>


