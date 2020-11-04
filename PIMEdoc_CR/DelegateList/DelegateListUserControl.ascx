<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DelegateListUserControl.ascx.cs" Inherits="PIMEdoc_CR.DelegateList.DelegateListUserControl" %>
<%@ Register TagPrefix="uc1" TagName="UpdateProgress" Src="../../PIMEdoc/UpdateProgress.ascx" %>
<link rel="stylesheet" href="../CSS/bootstrap.min.css" />
<link rel="stylesheet" href="../CSS/jquery-ui.css" />
<link rel="stylesheet" href="../CSS/Styles.css" />

<script src="../JS/jquery-1.10.2.js"></script>
<script src="../JS/bootstrap.js"></script>
<script src="../JS/jquery-ui.js"></script>

<script type="text/javascript">
    function pageLoad() {
        $('.datepicker').datepicker({
            dateFormat: 'dd/mm/yy'
        });
    }
</script>
<style>
    select {
        font-size: 15px !important;
    }
</style>
<div class="container" style="max-width: 1200px; width: 100%;">

    <uc1:updateprogress runat="server" id="UpdateProgress1"></uc1:updateprogress>
    <!-- modal Employee Search -->
    <div class="modal fade" id="searchEmpReqModal" role="dialog">
        <div class="modal-dialog" style="width: 80%; max-width: 1000px;">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <div class="row">
                                <div class="col-xs-9 col-sm-8 form-horizontal">
                                    <h4 class="modal-title">Search Employee</h4>
                                </div>
                                <div class="col-xs-3 col-sm-4 form-horizontal " style="text-align: right">
                                    <asp:Label ID="lblPopupMode" Visible="false" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="modal-body">
                            <div class="panel-collapse collapse in">
                                <asp:Panel runat="server" CssClass="panel-body" DefaultButton="searchReqBtn">
                                    <div class="row">
                                        <div class="col-sm-2">
                                            <asp:Label runat="server" Text="Search<br/>ค้นหา" CssClass="control-label"></asp:Label>
                                        </div>
                                        <div class="col-sm-10">
                                            <div class="input-group stylish-input-group">
                                                <asp:TextBox runat="server" ID="txt_searchBox" CssClass="form-control input-sm" placeholder="Search"></asp:TextBox>
                                                <span class="input-group-addon">
                                                    <asp:LinkButton type="submit" ID="searchReqBtn" OnClick="searchBtn_Click" runat="server">
                                                            <span class="glyphicon glyphicon-search"></span>
                                                    </asp:LinkButton>
                                                </span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="table-responsive">
                                            <asp:GridView runat="server" ID="gv_searchEmpReqTable" OnRowDataBound="searchEmpTable_RowDataBound" OnSelectedIndexChanged="gv_searchEmpReqTable_SelectedIndexChanged"
                                                AutoGenerateColumns="false" AllowPaging="true" PageSize="5" RowStyle-Wrap="false" EmptyDataRowStyle-HorizontalAlign="Center" OnPageIndexChanging="gv_searchEmpReqTable_PageIndexChanging"
                                                ShowHeaderWhenEmpty="true" CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                                <Columns>
                                                    <asp:BoundField HeaderText="EmployeeID" DataField="EMPLOYEEID" ItemStyle-HorizontalAlign="Center" />
                                                    <%--<asp:BoundField HeaderText="DEPARTMENT_ID" DataField="DEPARTMENT_ID" Visible="true" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:BoundField HeaderText="SUBDEPARTMENT_ID" DataField="SUBDEPARTMENT_ID" Visible="true"  ItemStyle-HorizontalAlign="Center" />
                                                    <asp:BoundField HeaderText="POSITION_TD" DataField="POSITION_TD" Visible="true"  ItemStyle-HorizontalAlign="Center" />--%>

                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Employee Name">
                                                        <ItemTemplate>
                                                            <asp:HiddenField ID="hdnEmpID" runat="server" Value='<%# Eval("EMPLOYEEID") %>'></asp:HiddenField>
                                                            <asp:HiddenField ID="hdnDeptID" runat="server" Value='<%# Eval("DEPARTMENT_ID") %>'></asp:HiddenField>
                                                            <asp:HiddenField ID="hdnSubDeptID" runat="server" Value='<%# Eval("SUBDEPARTMENT_ID") %>'></asp:HiddenField>
                                                            <asp:HiddenField ID="hdnPosID" runat="server" Value='<%# Eval("POSITION_TD") %>'></asp:HiddenField>

                                                            <asp:Label ID="lblEmpName" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Department">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblDepartment" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="SubDepartment">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSubDepartment" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Position">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPosition" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Email" DataField="EMAIL" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:BoundField HeaderText="Tel" DataField="TELEPHONE" ItemStyle-HorizontalAlign="Center" />
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No Data.
                                                </EmptyDataTemplate>
                                                <PagerStyle CssClass="pager-container" Wrap="true" />
                                                <PagerSettings Mode="Numeric" PageButtonCount="5" PreviousPageText="previous" NextPageText="next" FirstPageText="<" LastPageText=">" Position="Bottom" />
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <div class="row">
                                <div class="col-sm-8"></div>
                                <div class="col-sm-2">
                                    <asp:Button ID="btn_resetBtn" runat="server" CssClass="btn btn-danger btn-block" OnClick="btn_resetBtn_Click" Text="Reset"></asp:Button>

                                </div>
                                <div class="col-sm-2">
                                    <asp:Button ID="btn_closeBtn" runat="server" CssClass="btn btn-secondary btn-block" OnClick="btnClosePopup_Click" Text="Close"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>


        </div>
    </div>
    <div id="divBody" runat="server">
        <!-- DelegateList Header -->
        <div style="width: 100%; height: auto !important">
            <asp:Label Text="Delegate List" runat="server" style="font-size:35px !important;" CssClass="lbl_Title"></asp:Label>
            <div style="border-top: 1px solid #FBB300; margin-bottom: 10px;">
            </div>
        </div>
        <asp:UpdatePanel ID="updatePanel_user" runat="server">
            <Triggers>
            </Triggers>
            <ContentTemplate>
                <!-- DelegteList Panel -->
                <div runat="server" class="panel-group" id="panelDelegate">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseDelegate">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapseDelegate">
                                    <asp:Label ID="Label32" runat="server" Text="Delegate List" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label33" runat="server" Text="หน้ารายการการมอบอำนาจการอนุมัติงาน" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <asp:Panel runat="server" ID="collapseDelegate" class="panel-collapse collapse in" Style="overflow: hidden" DefaultButton="btn_Search">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label9" runat="server" Text="Search" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label10" runat="server" Text="ค้นหา" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox runat="server" ID="txt_search" CssClass="form-control input-sm" placeholder="Search"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label11" runat="server" Text="Approver" CssClass="lbl_ENG"></asp:Label>
                                        <span class="red">*</span>
                                        <br />
                                        <asp:Label ID="Label12" runat="server" Text="ผู้อนุมัติงาน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="input-group stylish-input-group">
                                            <asp:Label CssClass="form-control" runat="server" ID="lbl_Approver"></asp:Label>
                                            <asp:HiddenField Value="" runat="server" ID="hdn_ApproverID" />
                                            <span class="input-group-addon">
                                                <!-- Modal Trigger -->
                                                <asp:LinkButton type="submit" ID="lkb_SearchApprover" runat="server" Enabled="true" OnClick="OpenPopup">
                                                        <span class="glyphicon glyphicon-search"></span>
                                                </asp:LinkButton>
                                            </span>
                                        </div>
                                    </div>

                                    <div class="col-sm-2">
                                        <asp:Label ID="Label13" runat="server" Text="Delegate To" CssClass="lbl_ENG"></asp:Label>
                                        <span class="red">*</span>
                                        <br />
                                        <asp:Label ID="Label14" runat="server" Text="ผู้อนุมัติงานแทน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="input-group stylish-input-group">
                                            <asp:Label CssClass="form-control" runat="server" ID="lbl_DelegateTo"></asp:Label>
                                            <asp:HiddenField Value="" runat="server" ID="hdn_DelegateToID" />
                                            <span class="input-group-addon">
                                                <!-- Modal Trigger -->
                                                <asp:LinkButton type="submit" ID="lkb_searchDelegateTo" runat="server" Enabled="true" OnClick="OpenPopup">
                                                        <span class="glyphicon glyphicon-search"></span>
                                                </asp:LinkButton>
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label17" runat="server" Text="Date From" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label18" runat="server" Text="วันที่" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_DateFrom" AutoPostBack="true" CssClass="form-control datepicker"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label3" runat="server" Text="Date To" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label4" runat="server" Text="ถึงวันที่" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_DateTo" AutoPostBack="true" CssClass="form-control datepicker"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <%-- By Button Panel --%>
                                <div class="row">
                                    <div class="col-sm-8"></div>
                                    <div class="col-sm-2">
                                        <asp:Button Text="Search" runat="server" CssClass="CssButton" ID="btn_Search" OnClick="btn_Search_Click"/>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button Text="Reset" runat="server" CssClass="CssButton" ID="btn_Reset" OnClick="btn_Reset_Click" />
                                    </div>
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                </div>

                <%-- By DocNo Panel --%>
                <div runat="server" class="panel-group" id="panelDelegateByDocNo">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseByDocNo">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapseByDocNo">
                                    <asp:Label ID="Label25" runat="server" Text="Delegate By Document No" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label26" runat="server" Text="ให้สิทธิ์การอนุมัติตามใบงาน" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <asp:Panel runat="server" ID="collapseByDocNo" class="panel-collapse collapse in" Style="overflow: hidden">
                            <div class="panel-body">
                                <div class="row" runat="server" id="panel_gvDocNo">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gv_DelegateByDocNo" OnRowDataBound="gv_DelegateByDocNo_RowDataBound"
                                            AutoGenerateColumns="false" RowStyle-Wrap="true" EmptyDataRowStyle-HorizontalAlign="Center"
                                            ShowHeaderWhenEmpty="true" CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                            <Columns>
                                                <%--<asp:TemplateField HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <asp:CheckBox ID="chk_HeadByDocNO" runat="server" OnCheckedChanged="chk_HeadByDocNo_CheckedChanged" AutoPostBack="true" />
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chk_ByDocNO" runat="server" OnCheckedChanged="chk_ByDocNo_CheckedChanged" AutoPostBack="true" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Document NO<br />
                                                                เลขที่เอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_DocNo" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Category<br />
                                                                หมวดหมู่</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_Category"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Document Type<br />
                                                                ประเภทเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_DocType"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Title<br />
                                                                ชื่อเรื่อง</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_Title" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Delegate To<br />
                                                                ผู้อนุมัติงานแทน</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_DelegateTo" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Date From<br />
                                                                วันที่</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_DateFrom" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Date To<br />
                                                                ถึงวันที่</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_DateTo" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Remark<br />
                                                                หมายเหตุ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_Remark" Text='<%# Eval("Remark") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Attachment<br />
                                                                ไฟล์แนบ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:HyperLink NavigateUrl="-" runat="server" Text="-" ID="hpl_AttachDoc"  Target="_blank"/>
                                                        <%--<asp:Label runat="server" ID="lbl_Attachment" Text="-"></asp:Label>--%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Modified Date<br />
                                                                วันที่แก้ไขเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_ModifiedDate"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Modified By<br />
                                                                ผู้แก้ไขเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_ModifiedBy"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Status<br />
                                                                สถานะ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_Status"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Create Date<br />
                                                                วันที่สร้างเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_CreateDate"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Requestor<br />
                                                                ผู้ร้องขอ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_Requestor"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Status<br />
                                                                สถานะ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_Status" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Amount<br />
                                                                ยอดเงิน</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_Amount"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                </div>
                
                
                <%-- By DocType Panel --%>
                <div runat="server" class="panel-group" id="panelDelegateByDocType">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseByDocType">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapseByDocType">
                                    <asp:Label ID="Label27" runat="server" Text="Delegate By Document Type" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label28" runat="server" Text="ให้สิทธิ์การอนุมัติตามประเภทเอกสาร" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <asp:Panel runat="server" ID="collapseByDocType" class="panel-collapse collapse in" Style="overflow: hidden">
                            <div class="panel-body">
                                <div class="row" runat="server" id="Div2">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gv_DelegateByDocType" OnRowDataBound="gv_DelegateByDocType_RowDataBound"
                                            AutoGenerateColumns="false" RowStyle-Wrap="true" EmptyDataRowStyle-HorizontalAlign="Center"
                                            ShowHeaderWhenEmpty="true" CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Document Type<br />
                                                                ประเภทเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_DocType"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                 <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Delegate To<br />
                                                                ผู้อนุมัติงานแทน</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_DelegateTo" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Date From<br />
                                                                วันที่</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_FromDate" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Date To<br />
                                                                ถึงวันที่</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_ToDate" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Remark<br />
                                                                หมายเหตุ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_Remark" Text='<%# Eval("Remark") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center"  ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Attachment<br />
                                                                ไฟล์แนบ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:HyperLink NavigateUrl="-" runat="server" Text="-" ID="hpl_AttachDoc"  Target="_blank"/>
                                                        <%--<asp:Label runat="server" ID="lbl_Attachment" Text="-"></asp:Label>--%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Modified Date<br />
                                                                วันที่แก้ไขเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_ModifiedDate"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Modified By<br />
                                                                ผู้แก้ไขเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_ModifiedBy"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Status<br />
                                                                สถานะ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_Status"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>

