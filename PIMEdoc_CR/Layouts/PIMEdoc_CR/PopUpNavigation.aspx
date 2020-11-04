<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PopUpNavigation.aspx.cs" Inherits="PIMEdoc_CR.Layouts.PIMEdoc_CR.PopUpNavigation" DynamicMasterPageFile="~masterurl/default.master" %>


<%@ Register TagPrefix="SharePointPublishing" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript">

        function ClosePopUp() {
            var Parameter = [];
            window.frameElement.commonModalDialogClose(1, Parameter);
        }
    </script>
    <style type="text/css">
        .hiddencontrol {
            display: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <%--<asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <uc2:Progress ID="Progress1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>--%>

    <table width="100%" class="ms-dialogHeader" style="padding-top: 8px; padding-bottom: 10px;" border="0" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td align="center" valign="middle" style="width: 0px; padding-right: 15px; padding-left: 15px;">
                    <img id="onetidtpweb1" alt="" src="/_layouts/15/images/detail.gif?rev=23" />
                </td>
                <td class="ms-dialogHeaderDescription" valign="top" style="width: 100%;">Edit the title, URL, and description of the navigation item.

                </td>
                <td align="right" class="ms-dialogHelpLink" nowrap="nowrap" valign="top" style="width: 0px; padding-right: 10px;"></td>
            </tr>
        </tbody>
    </table>
    <table width="100%" height="100%" border="0" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td height="0" id="bodyHeader"></td>
            </tr>
            <tr>
                <td class="ms-dialogBodyMain" valign="top" style="width: 100%; height: 100%;">
                    <div id="bodyMain" style="width: 584px; height: 300px; overflow: auto;">
                        <table width="100%" height="100%" class="ms-authoringcontrols" border="0">
                            <tbody>
                                <tr>
                                    <td>
                                        <table align="center" class="ms-authoringcontrols" border="0" cellspacing="5" cellpadding="2">
                                            <tbody>
                                                <tr>
                                                    <td align="right" valign="top" style="padding-top: 5px;">Title:</td>
                                                    <td valign="top">
                                                        <asp:TextBox ID="txtTitle" runat="server" Width="370px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="top" style="padding-top: 8px;">URL:
                                                    </td>
                                                    <td valign="top">
                                                        <SharePointPublishing:AssetUrlSelector ID="AssetUrlSelector1" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="top" style="padding-top: 5px;">Description:</td>
                                                    <td valign="top">
                                                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" Width="370px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" valign="top" style="padding-top: 5px;">Audience:</td>
                                                    <td valign="top">
                                                        <SharePoint:PeopleEditor ID="PeopleEditor1" runat="server" AllowEmpty="true" MultiSelect="true" SelectionSet="SPGroup" ValidatorEnabled="true" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2"></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>

                            </tbody>
                        </table>

                    </div>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="btnOK" runat="server" Text="OK" OnClick="btnOK_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="ClosePopUp()" />
                </td>
            </tr>
        </tbody>
    </table>
    <asp:Label ID="lbllog" CssClass="hiddencontrol" runat="server"></asp:Label>

    <asp:Panel ID="Panel1" runat="server"></asp:Panel>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    My Application Page
</asp:Content>
