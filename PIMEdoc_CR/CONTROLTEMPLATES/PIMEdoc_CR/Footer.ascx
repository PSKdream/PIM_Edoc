<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Footer.ascx.cs" Inherits="PIMEdoc_CR.ControlTemplates.PIMEdoc_CR.Footer" %>

<style>
    h4 {
        color: white;
    }

   .tfoot, .tfoot a, .tfoot a:hover, .tfoot a:visited {
        color: black;
        text-decoration:none;
    }

        .tfoot a:hover {
            color: #01883c;
            text-decoration:none;
        }
        /*====================================PC Responsive===========================================*/
.column {
    float: left;
  /* padding-top: 5px;*/
}

    .column.full {
        width: 100%;
    }

    .column.two-thirds {
        width: 66.7%;
    }

    .column.half {
        width: 50%;
    }
    .column.third {
        width: 33.3%;
    }
    .column.thirds-five {
        width: 85%;
    }
     .column.five {
        width: 15%;
    }
    .column.fourth {
        width: 25%;
    }
   .ItemColumn {
        width: 100%;
    }

    @media screen and (min-width: 780px) {
        .ItemColumn {
            width: 25%;
            float:left;
        }

    }
</style>
<div class="div_body">
    <div style="width: 100%; background: #f6f6f6; border-top:3px solid #01883c;" >
        <div style="max-width: 1100px; width: 100%; margin: 0 auto; height: auto !important">
            <table style="width: 100%;">
                <tr>
                    <td>
                        <div class="ItemColumn" style="vertical-align: top; padding: 20px 0px">
                            <asp:Label runat="server" Text="PIM" Font-Size="16px" Font-Bold="true" ForeColor="#01883c"></asp:Label>
                            <br />
                            <asp:GridView ID="grvMenu1" runat="server" ShowHeader="false" CssClass="tfoot" AutoGenerateColumns="false" GridLines="None">
                                <Columns>
                                    <asp:HyperLinkField DataTextField="RelateName" DataNavigateUrlFields="LinkUrl" Target="_blank" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="ItemColumn" style="vertical-align: top; padding: 20px 0px">
                            <asp:Label runat="server" Text="คณะ/วิทยาลัย" Font-Size="16px" Font-Bold="true" ForeColor="#01883c"></asp:Label>
                            <br />
                            <asp:GridView ID="grvMenu2" runat="server" ShowHeader="false"  CssClass="tfoot" AutoGenerateColumns="false" GridLines="None">
                                <Columns>
                                    <asp:HyperLinkField DataTextField="RelateName" DataNavigateUrlFields="LinkUrl" Target="_blank" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="ItemColumn" style="vertical-align: top; padding: 20px 0px">
                            <asp:Label runat="server" Text="หลักสูตรที่เปิดสอน" Font-Size="16px" Font-Bold="true" ForeColor="#01883c"></asp:Label>
                            <br />
                            <asp:GridView ID="grvMenu3" runat="server"  CssClass="tfoot" ShowHeader="false" AutoGenerateColumns="false" GridLines="None">
                                <Columns>
                                    <asp:HyperLinkField DataTextField="RelateName" DataNavigateUrlFields="LinkUrl" Target="_blank" />
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="ItemColumn" style="vertical-align: top; padding: 20px 0px">
                            <asp:Label runat="server" Text="แหล่งความรู้" Font-Size="16px" Font-Bold="true" ForeColor="#01883c"></asp:Label>
                            <br />
                            <asp:GridView ID="grvMenu4" runat="server" CssClass="tfoot" ShowHeader="false" AutoGenerateColumns="false" GridLines="None">
                                <Columns>
                                    <asp:HyperLinkField DataTextField="RelateName" DataNavigateUrlFields="LinkUrl" Target="_blank" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div style="width: 100%; background: #01883c">
        <div style="max-width: 1100px; width: 100%; margin: 0 auto; height: auto !important; text-align: center; padding: 20px 0px; color: #fff; font-size:1 em !important">
            <asp:Label ID="lblAddress" runat="server" />
        </div>
    </div>
</div>
