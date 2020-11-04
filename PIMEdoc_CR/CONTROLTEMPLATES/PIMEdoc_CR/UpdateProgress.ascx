<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateProgress.ascx.cs" Inherits="PIMEdoc_CR.ControlTemplates.PIMEdoc_CR.UpdateProgress" %>


<script type="text/javascript">

    function MoveSiteTitle()
    {

    }

</script>

<asp:UpdateProgress runat="server">
            <ProgressTemplate>
                <div class="Modal">
                    <img src="../Img/ring-alt.gif" class="LoadingImg" />
                </div>
                <style>

                    .SvgImg {
                        background-image : url('');
                    }


                    .Modal {
                        position: fixed;
                        width: 100%;
                        height: 100%;
                        background-color: #F0F0F0; 
                        filter: alpha(opacity=50); 
                        opacity: 0.5;
                        top: 0;
                        left: 0;
                        right: 0;
                        bottom: 0;
                        z-index: 10050;
                    }
                    .LoadingImg {
                        position: absolute;
                        top: 0;
                        right: 0;
                        bottom:0;
                        left:0;
                        margin: auto;
                        z-index: 10051;
                    }
                </style>
            </ProgressTemplate>
        </asp:UpdateProgress>