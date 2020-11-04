<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportTimeManagementUserControl.ascx.cs" Inherits="PIMEdoc_CR.ReportTimeManagement.ReportTimeManagementUserControl" %>
<%@ Register TagPrefix="uc1" TagName="UpdateProgress" Src="../../PIMEdoc/UpdateProgress.ascx" %>

<link rel="stylesheet" href="../CSS/bootstrap.min.css" />
<link rel="stylesheet" href="../CSS/jquery-ui.css" />
<link rel="stylesheet" href="../CSS/Styles.css" />

<script src="../JS/jquery-3.2.1.min.js"></script>
<script src="../JS/bootstrap.js"></script>
<script src="../JS/jquery-ui.js"></script>

<script type="text/javascript">
    function pageLoad() {
        $('.datepicker').datepicker({
            dateFormat: 'dd/mm/yy'
        });
    }
</script>
<uc1:UpdateProgress runat="server" ID="UpdateProgress1"></uc1:UpdateProgress>
<!-- Popup History -->
<div class="modal fade" id="showHistory" role="dialog">
    <div class="modal-dialog" style="width: 80%;">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <div class="row">
                            <div class="col-xs-9 col-sm-8 form-horizontal">
                                <h4 class="modal-title">History</h4>
                            </div>
                            <div class="col-xs-3 col-sm-4 form-horizontal " style="text-align: right">
                            </div>
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="panel-collapse collapse in">
                            <asp:Panel runat="server" CssClass="panel-body">


                                <div class="row">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gv_History" OnRowDataBound="gv_History_OnRowDataBound"
                                            AutoGenerateColumns="false" RowStyle-Wrap="true" EmptyDataRowStyle-HorizontalAlign="Center"
                                            ShowHeaderWhenEmpty="true" CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Action Date<br />
                                                                วันที่ดำเนินการ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_ActionDate"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Action By<br />
                                                                ผู้ดำเนินการ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_ActionBy"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Status<br />
                                                                สถานะ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" Text='<%# Eval("StatusBefore") %>'>></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Action<br />
                                                                ดำเนินการโดย</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" Text='<%# Eval("ActionName") %>'>></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Additional Comment<br />
                                                                หมายเหตุ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div style="padding: 0 10px 0 10px;">
                                                            <asp:Label runat="server" ID="lbl_HistoryComment" Text='<%# Eval("Comment") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <%--<asp:BoundField HeaderText="Status" DataField="StatusBefore" ItemStyle-HorizontalAlign="Center" />--%>
                                                <%--<asp:BoundField HeaderText="Action" DataField="ActionName" ItemStyle-HorizontalAlign="Center" />--%>
                                                <%--<asp:BoundField HeaderText="Additional Comment" DataField="Comment" ItemStyle-HorizontalAlign="Center" />--%>
                                            </Columns>
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
                                <%--<asp:Button ID="Button1" runat="server" CssClass="btn btn-danger btn-block" OnClick="btnSubmitToAssign_Click" Text="Submit"></asp:Button>--%>
                            </div>
                            <div class="col-sm-2">
                                <asp:Button ID="btn_closePopup" runat="server" CssClass="btn btn-danger btn-block" OnClick="btn_closePopup_OnClick" Text="Close"></asp:Button>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<asp:Panel CssClass="container" runat="server" Style="max-width: 1200px; width: 100%;" Visible="true" ID="panel_Container">
    <div id="divBody" runat="server">
        <!-- Report Header -->
        <div style="width: 100%; margin: 30px auto 0 auto; height: auto !important">
            <asp:Label Text="Time Management Report" runat="server" Font-Size="22px" CssClass="lbl_Title"></asp:Label>
            <div style="border-top: 1px solid #FBB300; margin-bottom: 10px;">
            </div>
        </div>

        <asp:UpdatePanel ID="updatePanel_user" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <!-- Filter Panel -->
                <div runat="server" class="panel-group" id="panel_Filter">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseReport">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapseReport">
                                    <asp:Label ID="Label32" runat="server" Text="Time Management Report" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label33" runat="server" Text="รายงานระยะเวลาดำเนินการ" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <asp:Panel runat="server" ID="collapseReport" class="panel-collapse collapse in" Style="overflow: hidden">
                            <div class="panel-body">
                                <%-- Infomation --%>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:Label ID="Label48" runat="server" Text="Filter" Style="font-size: 20px;" CssClass="lbl_ENG"></asp:Label><br />
                                        <asp:Label ID="Label1" runat="server" Text="ตัวกรองข้อมูล" Style="font-size: 20px;" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label12" runat="server" Text="Data Type" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label13" runat="server" Text="ประเภทข้อมูล" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:RadioButtonList runat="server" ID="rdb_DataType" CssClass="rbl" AutoPostBack="true"
                                            RepeatDirection="Horizontal" RepeatLayout="Table">
                                            <asp:ListItem Text="Total Time to Finish Process<br/>ข้อมูลระยะเวลาตั้งแต่ต้นจนจบรายการ" Value="finish" Selected="True" />
                                            <asp:ListItem Text="Time Action to Action<br/>ข้อมูลระยะเวลาของแต่ละการดำเนินการ" Value="action" />
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label2" runat="server" Text="Search" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label3" runat="server" Text="คำค้นหา" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-10">
                                        <asp:TextBox runat="server" CssClass="form-control" ID="txt_search" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label4" runat="server" Text="Category" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label5" runat="server" Text="หมวดหมู่เอกสาร" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_Category">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label6" runat="server" Text="Document Type" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label7" runat="server" Text="ประเภทเอกสาร" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_DocType">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label8" runat="server" Text="Other Type" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label9" runat="server" Text="ประเภทอื่นๆ" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_OtherDocType">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label10" runat="server" Text="From Department" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label11" runat="server" Text="หน่วยงานตั้งต้น" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_FromDepartment">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label14" runat="server" Text="To Department" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label15" runat="server" Text="หน่วยงานรับเรื่อง" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_ToDepartment">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label16" runat="server" Text="Priority" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label17" runat="server" Text="ความเร่งด่วนของงาน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_Priority">
                                            <asp:ListItem Text="-- All Priority --" Value="Normal" />
                                            <asp:ListItem Text="ทั่วไป" Value="Normal" />
                                            <asp:ListItem Text="ด่วน" Value="Fast" />
                                            <asp:ListItem Text="ด่วนมาก" Value="Faster" />
                                            <asp:ListItem Text="ด่วนที่สุด" Value="Fastest" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label18" runat="server" Text="Deadline" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label19" runat="server" Text="วันที่ครบกำหนด" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_Deadline" CssClass="form-control datepicker"></asp:TextBox>

                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label20" runat="server" Text="Permission Doc" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label21" runat="server" Text="สิทธิ์การเข้าถึงเอกสาร" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_Permission">
                                            <asp:ListItem Text="-- All Permission --" Value="" />
                                            <asp:ListItem Text="เปิดเผย" Value="Public" />
                                            <asp:ListItem Text="ความลับ" Value="Secret" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label22" runat="server" Text="Design of Authority" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label23" runat="server" Text="เอกสารเกี่ยวกับยอดเงิน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_DOA">
                                            <asp:ListItem Text="All" Value=""/>
                                            <asp:ListItem Text="Yes" Value="Y" />
                                            <asp:ListItem Text="No" Value="N" />
                                        </asp:DropDownList>
                                        <%--<asp:CheckBox Text="Design of Authority<br/>เอกสารเกี่ยวกับยอดเงิน" CssClass="rdb" ID="chk_DOA" runat="server" />--%>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="Create Date From<br/>วันที่" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_CreateDateFrom" CssClass="form-control datepicker"></asp:TextBox>

                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="Create Date To<br/>ถึงวันที่" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_CreateDateTo" CssClass="form-control datepicker"></asp:TextBox>

                                        </div>
                                    </div>
                                </div>
                                <%--<div class="row">
                                    <div class="col-lg-2">
                                        <asp:Label ID="Label22" runat="server" Text="Status" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label23" runat="server" Text="สถานะของเอกสาร" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_Status">
                                            <asp:ListItem Text="-- All Status --" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Draft" Value="Draft"></asp:ListItem>
                                            <asp:ListItem Text="Wait for Requestor Review" Value="Wait for Requestor Review"></asp:ListItem>
                                            <asp:ListItem Text="Wait for Approve" Value="Wait for Approve"></asp:ListItem>
                                            <asp:ListItem Text="Wait for Comment" Value="Wait for Comment"></asp:ListItem>
                                            <asp:ListItem Text="Wait for Admin Centre" Value="Wait for Admin Centre"></asp:ListItem>
                                            <asp:ListItem Text="Rework" Value="Rework"></asp:ListItem>
                                            <asp:ListItem Text="Completed" Value="Completed"></asp:ListItem>
                                            <asp:ListItem Text="Rejected" Value="Rejected"></asp:ListItem>
                                            <asp:ListItem Text="Cancelled" Value="Cancelled"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>--%>
                                <div class="row">
                                    <div class="col-sm-2 col-md-2 col-lg-4"></div>
                                    <div class="col-xs-6 col-sm-4 col-md-4 col-lg-2">
                                        <asp:Button Text="Search" ID="btn_Submit" CssClass="CssButton" runat="server" OnClick="btn_Submit_Click" />
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-md-4 col-lg-2">
                                        <asp:Button Text="Reset" ID="btn_Reset" CssClass="CssButton" runat="server" OnClick="btn_Reset_Click" />
                                    </div>
                                    <div class="col-sm-2 col-md-2 col-lg-4"></div>
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                </div>

                <%-- GV Panel --%>

                <div runat="server" class="panel-group" id="Div1">
                    <div class="panel panel-default">
                        <asp:Panel runat="server" ID="Panel1" class="panel-collapse collapse in" Style="overflow: hidden">
                            <div class="panel-body">

                                <div class="row">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gv_Report" OnPageIndexChanging="gv_Report_PageIndexChanging"
                                            OnRowDataBound="gv_Report_RowDataBound" OnSelectedIndexChanged="gv_Report_SelectedIndexChanged"
                                            AutoGenerateColumns="false" AllowPaging="true" PageSize="20" RowStyle-Wrap="false" EmptyDataRowStyle-HorizontalAlign="Center"
                                            ShowHeaderWhenEmpty="true" CssClass=" table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Document No.<br />
                                                                เลขที่เอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnDocID" runat="server" Value='<%# Eval("DocID") %>' />
                                                        <asp:Label ID="lblDocNo" runat="server" Text='<%# Eval("DocNo") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Document Type<br />
                                                                ประเภทเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnDocNo" runat="server" />
                                                        <asp:Label ID="lblDocType" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Other Type<br />
                                                                ประเภทเอกสารอื่นๆ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOtherDocType" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Title<br />
                                                                ชื่อเรื่อง</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTitle" Text='<%# Eval("Title") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>From Department<br />
                                                                หน่วยงานตั้งต้น</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFromDepartment" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>To Dapartment<br />
                                                                หน่วยงานรับเรื่อง</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblToDepartment" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Priority<br />
                                                                ระดับความเร่งด่วน</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPriority" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Deadline<br />
                                                                วันที่ครบกำหนด</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDeadline" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>DOA<br />
                                                                เอกสารเกี่ยวกับยอดเงิน</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDOA" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Permission<br />
                                                                สิทธิ์ในการเข้าถึง</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPermissionDoc" runat="server" Text='<%# Eval("PermissionType") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Create Date<br />
                                                                วันที่สร้างเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCreateDate" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Created By<br />
                                                                ผู้สร้างเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCreator" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="EmployeeID" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmpID" runat="server" Text='<%# Eval("EmpID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Requestor Name<br />
                                                                ผู้ร้องขอเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmpName" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Status Before" HeaderStyle-HorizontalAlign="Center" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatusBe4" runat="server" Text='<%# Eval("StatusBefore") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="ActionName" HeaderStyle-HorizontalAlign="Center">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Action Name<br />
                                                                การดำเนินการ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblActionName" runat="server" Text='<%# Eval("ActionName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Action Date<br />
                                                                วันที่ดำเนินการ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblActionDate" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Status">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Status<br />
                                                                สถานะเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                                                     <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Timing<br />
                                                                เวลาที่ใช้ในการดำเนินการ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTiming" runat="server"></asp:Label>
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
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>


                <%-- summary panel --%>

                <div runat="server" class="panel-group" id="Div2">
                    <div class="panel panel-default">
                        <asp:Panel runat="server" ID="Panel2" class="panel-collapse collapse in" Style="overflow: hidden">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12 col-md-6 col-md-push-6 col-lg-4 col-lg-push-8">
                                        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4">
                                            <asp:Label ID="Label24" runat="server" Text="Total Time" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label25" runat="server" Text="ระยะเวลาทั้งหมด" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-xs-8 col-sm-8 col-md-8 col-lg-8">
                                            <asp:Label Text="-" runat="server" ID="lbl_TotalTime"/>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-xs-12 col-sm-12 col-md-6 col-md-push-6 col-lg-4 col-lg-push-8">
                                        <div class="col-xs-4 col-sm-4 col-md-4 col-lg-4">
                                            <asp:Label ID="Label26" runat="server" Text="Mean Time" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label27" runat="server" Text="ค่าเฉลี่ย" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-xs-8 col-sm-8 col-md-8 col-lg-8">
                                            <asp:Label Text="-" runat="server" ID="lbl_MeanTime"/>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Panel>
