<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorklistPartUserControl.ascx.cs" Inherits="PIMEdoc_CR.WorklistPart.WorklistPartUserControl" %>
<%@ Register TagPrefix="uc1" TagName="UpdateProgress" Src="../../PIMEdoc/UpdateProgress.ascx" %>
<link rel="stylesheet" href="../CSS/bootstrap.min.css" />
<link rel="stylesheet" href="../CSS/jquery-ui.css" />
<link rel="stylesheet" href="../CSS/Styles.css" />

<script src="../JS/jquery-3.2.1.js"></script>
<script src="../JS/bootstrap.js"></script>
<script src="../JS/jquery-ui.js"></script>

<uc1:UpdateProgress runat="server" ID="UpdateProgress1"></uc1:UpdateProgress>
<asp:Panel CssClass="container" runat="server" Style="max-width: 1200px; width: 100%;" Visible="true" ID="panel_Container">
    <div id="divBody" runat="server">
        <!-- Worklist Portal Header -->
        <div style="width: 100%; margin: 0 auto 0 auto; height: auto !important">
            <asp:Label Text="Worklist" runat="server" style="font-size:35px !important;" CssClass="lbl_Title"></asp:Label>
            <div style="border-top: 1px solid #FBB300; margin-bottom: 10px;">
            </div>
        </div>
        <div class="table-responsive">
                <asp:GridView runat="server" ID="gv_WorkList" OnRowDataBound="gv_WorkList_RowDataBound" OnRowCommand="gv_WorkList_OnRowCommand" OnSelectedIndexChanged="gv_WorkList_SelectedIndexChanged"
                    AutoGenerateColumns="false" AllowPaging="true" PageSize="5" RowStyle-Wrap="false" EmptyDataRowStyle-HorizontalAlign="Center" OnPageIndexChanging="gv_WorkList_PageIndexChanging"
                    ShowHeaderWhenEmpty="true" CssClass=" table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                    <Columns>
                        <asp:TemplateField HeaderText="หมวดหมู่เอกสาร" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0;">
                                    <span>Category<br />
                                        หมวดหมู่เอกสาร</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lbl_Category" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-Width="150px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0; min-width:100px;">
                                    <span>Document No.<br />
                                        เลขที่เอกสาร</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lbl_DocNo" runat="server" Text="-"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="ไฟล์เอกสาร" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DocID" runat="server" Text='<%# Eval("DocID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-Width="150px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0;">
                                    <span>Document Type<br />
                                        ประเภทเอกสาร</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lbl_DocType" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0;">
                                    <span>Title<br />
                                        ชื่อเอกสาร</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%--<asp:Label runat="server" Text='<%# Eval("Title") %>'>></asp:Label>--%>
                                <div style="text-align: left; white-space: normal; min-width: 300px;">
                                    <asp:Label runat="server" ID="lbl_title" Style="word-wrap: break-word; word-break: break-all;"></asp:Label>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-Width="250px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0;">
                                    <span>To Document<br />
                                        หน่วยงานรับเรื่อง</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                
                            <div style="text-align: left; min-width: 300px; white-space: normal;">
                                <asp:Label ID="lbl_ToDepartment" Text='<%# Eval("ToDepartmentName") %>' runat="server" Style="word-wrap: break-word; word-break: break-all;"></asp:Label>
                            </div>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-Width="200px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0;">
                                    <span>Document File<br />
                                        ไฟล์เอกสาร</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:HyperLink runat="server" ID="hpl_AttachDoc" Target="_blank" Text="-"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-Width="150px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0;">
                                    <span>CreatedDate<br />
                                        วันที่สร้างเอกสาร</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lbl_CreatedDate" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-Width="250px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0; min-width:100px;">
                                    <span>Requested By<br />
                                        ผู้ร้องขอเอกสาร</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lbl_Requestor" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-Width="150px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0;">
                                    <span>Status<br />
                                        สถานะ</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("Status") %>'>></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderStyle-Width="150px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                            <HeaderTemplate>
                                <div style="text-align: center; padding: 10px 0 10px 0;">
                                    <span>Waiting For<br />
                                        ผู้ดำเนินการถัดไป</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lbl_WaitingFor" Text='<%# Eval("WaitingFor") %>' runat="server"></asp:Label>
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
                <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#workList">
                    <h4 class="panel-title">
                        <a class="accordion-toggle TableStyleENG_Head" data-toggle="collapse" data-parent="#accordion" href="#workList">
                            <asp:Label ID="Label1" runat="server" Text="Worklist" CssClass=""></asp:Label>
                            <br />
                            <asp:Label ID="Label2" runat="server" Text="รายการงาน" CssClass=""></asp:Label>
                        </a>
                    </h4>
                </div>
                <div id="workList" class="collapse in" style="overflow: hidden">
                    <asp:UpdatePanel ID="UpdateWorklistPart" runat="server">
                        <ContentTemplate>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>--%>
    </div>
</asp:Panel>
