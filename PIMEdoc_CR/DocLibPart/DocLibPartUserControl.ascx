<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocLibPartUserControl.ascx.cs" Inherits="PIMEdoc_CR.DocLibPart.DocLibPartUserControl" %>
<%@ Register TagPrefix="uc1" TagName="UpdateProgress" Src="../../PIMEdoc/UpdateProgress.ascx" %>
<link rel="stylesheet" href="../CSS/bootstrap.min.css" />
<link rel="stylesheet" href="../CSS/jquery-ui.css" />
<link rel="stylesheet" href="../CSS/Styles.css" />

<%--<script src="../JS/jquery-3.2.1.js"></script>--%>
<%--<script src="../JS/bootstrap.js"></script>--%>
<script src="../JS/jquery-ui.js"></script>

<%--<uc1:UpdateProgress runat="server" ID="UpdateProgress1"></uc1:UpdateProgress>--%>
<asp:Panel CssClass="container" runat="server" Style="max-width: 1200px; width: 100%;" Visible="true" ID="panel_Container">
    <div id="divBody" runat="server">
        <!-- Dolib Portal Header -->
        <div style="width: 100%; margin: 0 auto 0 auto; height: auto !important">
            <asp:Label Text="Document Library" runat="server" Style="font-size: 35px !important;" CssClass="lbl_Title"></asp:Label>
            <div style="border-top: 1px solid #FBB300; margin-bottom: 10px;">
            </div>
        </div>
        <div class="table-responsive">
            <asp:GridView runat="server" ID="gv_CentreDocList" OnRowDataBound="gv_DocList_RowDataBound" OnSelectedIndexChanged="gv_CentreDocList_SelectedIndexChanged"
                AutoGenerateColumns="false" AllowPaging="true" PageSize="5" RowStyle-Wrap="false" EmptyDataRowStyle-HorizontalAlign="Center" OnPageIndexChanging="gv_CentreDocList_PageIndexChanging"
                ShowHeaderWhenEmpty="true" CssClass="table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                <Columns>
                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lbl_DocID" runat="server" Text='<%# Eval("DocID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div style="text-align: center; padding: 10px 0 10px 0;">
                                <span>Document Type<br />
                                    ประเภทเอกสาร</span>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_DocType" Text="-" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="เลขที่เอกสาร" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div style="text-align: center; padding: 10px 0 10px 0; min-width: 100px;">
                                <span>Document No<br />
                                    เลขที่เอกสาร</span>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_DocNo" Text="-" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:BoundField HeaderStyle-Width="300px" HeaderStyle-CssClass="text-center" HeaderText="ชื่อเรื่อง" DataField="Title" ItemStyle-HorizontalAlign="left" />--%>
                    <asp:TemplateField HeaderText="ชื่อเรื่อง" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                        <HeaderTemplate>
                            <div style="text-align: center; padding: 10px 0 10px 0;">
                                <span>Title<br />
                                    ชื่อเรื่อง</span>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <%-- <div style="text-align: left; min-width: 300px; white-space: normal;">
                                <asp:Label Text='<%# Eval("Title") %>' runat="server" Style="word-wrap: break-word; word-break: break-all;"></asp:Label>
                            </div>--%>
                            <div style="text-align: left; min-width: 300px; white-space: normal;">
                                <asp:Label ID="lbl_title" Text="-" runat="server" Style="word-wrap: break-word; word-break: break-all;"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="หน่วยงานตั้งต้น" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                        <HeaderTemplate>
                            <div style="text-align: center; padding: 10px 0 10px 0;">
                                <span>From Department<br />
                                    หน่วยงานตั้งต้น</span>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_fromDepartment" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="วันที่สร้างเอกสาร" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div style="text-align: center; padding: 10px 0 10px 0; min-width: 100px;">
                                <span>Created Date<br />
                                    วันที่สร้างเอกสาร</span>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_CreatedDate" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ไฟล์เอกสาร" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                        <HeaderTemplate>
                            <div style="text-align: center; padding: 10px 0 10px 0;">
                                <span>Document File<br />
                                    ไฟล์เอกสาร</span>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_DocomentAttachName" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-Width="180px" HeaderStyle-CssClass="text-center" HeaderText="ผู้ที่สร้างเอกสาร">
                        <HeaderTemplate>
                            <div style="text-align: center; padding: 10px 0 10px 0;">
                                <span>Creator<br />
                                    ผู้ที่สร้างเอกสาร</span>
                            </div>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbl_Creator" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
                <EmptyDataTemplate>
                    No Data.
                </EmptyDataTemplate>
                <PagerStyle CssClass="pager-container" Wrap="true" />
                <PagerSettings Mode="Numeric" PageButtonCount="5" PreviousPageText="previous" NextPageText="next" FirstPageText="<" LastPageText=">" Position="Bottom" />
            </asp:GridView>
        </div>
        <%--<div runat="server" class="panel-group" id="DocLib">
            <div class="panel panel-default">
                <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#DocLib">
                    <h4 class="panel-title">
                        <a class="accordion-toggle TableStyleENG_Head" data-toggle="collapse" data-parent="#accordion" href="#DocLib">
                            <asp:Label ID="Label1" runat="server" Text="Document Library" CssClass=""></asp:Label>
                            <br />
                            <asp:Label ID="Label2" runat="server" Text="รายการเอกสาร" CssClass=""></asp:Label>
                        </a>
                    </h4>
                </div>
                <div id="DocLib" class="collapse in" style="overflow: hidden">
                        <asp:UpdatePanel ID="UpdateDocLibPart" runat="server">
                            <ContentTemplate>
                                    
                            </ContentTemplate>
                        </asp:UpdatePanel>
                </div>
            </div>
        </div>--%>
    </div>
</asp:Panel>
