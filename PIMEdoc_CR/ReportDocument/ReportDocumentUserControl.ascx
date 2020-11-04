<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDocumentUserControl.ascx.cs" Inherits="PIMEdoc_CR.ReportDocument.ReportDocumentUserControl" %>
<%@ Register TagPrefix="uc1" TagName="UpdateProgress" Src="../../PIMEdoc/UpdateProgress.ascx" %>


<link rel="stylesheet" href="../CSS/bootstrap.min.css" />
<link rel="stylesheet" href="../CSS/jquery-ui.css" />
<link rel="stylesheet" href="../CSS/Styles.css" />

<script src="../JS/jquery-3.2.1.min.js"></script>
<script src="../JS/bootstrap.js"></script>
<script src="../JS/jquery-ui.js"></script>
<script src="../JS/highcharts.js"></script>
<script src="../JS/pie.js"></script>

<script type="text/javascript">
    function pageLoad() {
        $('.datepicker').datepicker({
            dateFormat: 'dd/mm/yy'
        });
    }
</script>
<uc1:UpdateProgress runat="server" ID="UpdateProgress1"></uc1:UpdateProgress>
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
                                            <asp:TextBox runat="server" ID="txt_searchBox" CssClass="form-control" placeholder="Search"></asp:TextBox>
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
<!-- modal Department Search -->
<div class="modal fade" id="searchDepartmentModal" role="dialog">
    <div class="modal-dialog" style="width: 80%; max-width: 1000px;">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <div class="row">
                            <div class="col-xs-9 col-sm-8 form-horizontal">
                                <h4 class="modal-title">Search Department</h4>
                            </div>
                            <div class="col-xs-3 col-sm-4 form-horizontal " style="text-align: right">
                            </div>
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="panel-collapse collapse in">
                            <asp:Panel runat="server" class="panel-body" DefaultButton="lkbtnSearchDepartment">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="Search<br/>ค้นหา" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">

                                        <div class="input-group stylish-input-group">
                                            <asp:TextBox runat="server" ID="txtSearch_Department" CssClass="form-control" placeholder="Search"></asp:TextBox>

                                            <span class="input-group-addon">
                                                <asp:LinkButton type="submit" ID="lkbtnSearchDepartment" runat="server" OnClick="lkbtnSearchDepartment_Click">
                                                            <span class="glyphicon glyphicon-search"></span>
                                                </asp:LinkButton>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="Department Level" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:DropDownList ID="ddl_seachDeptLevel" AutoPostBack="true" OnSelectedIndexChanged="ddl_seachDeptLevel_SelectedIndexChanged" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="Level 1: คณะ/สำนัก/ศูนย์" Value="1" />
                                            <asp:ListItem Text="Level 2: งานต่างๆที่อยู่ภายใต้ คณะ/สำนัก" Value="0" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gv_SearchDepartment" OnRowDataBound="gv_SearchDepartment_RowDataBound"
                                            OnPageIndexChanging="gv_SearchDepartment_PageIndexChanging" OnSelectedIndexChanged="gv_SearchDepartment_SelectedIndexChanged"
                                            AutoGenerateColumns="false" AllowPaging="true" PageSize="5" RowStyle-Wrap="false"
                                            ShowHeaderWhenEmpty="true" CssClass="table table-hover table-striped  table-bordered table-condensed header-left col-sm-12">
                                            <Columns>
                                                <%--<asp:BoundField ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="150" DataField="DEPARTMENT_ID" HeaderText="Department ID" Visible="false"/>--%>
                                                <%--<asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="PRIMARY" HeaderText="Department Level" />--%>
                                                <asp:TemplateField Visible="false">
                                                    <HeaderTemplate>
                                                        <asp:Label Text="Department Level" runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_DepartmentLevel" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:Label Text="Department Code" runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_DepartmentCode" Text='<%# Eval("DEPARTMENT_CODE") %>' runat="server"></asp:Label>
                                                        <asp:HiddenField runat="server" ID="hdnDepartmentID" Value='<%# Eval("DEPARTMENT_ID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="DEPARTMENT_CODE" HeaderText="Department Code" />--%>
                                                <asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="DEPARTMENT_NAME_TH" HeaderText="Department Name TH" />
                                                <asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="DEPARTMENT_NAME_EN" HeaderText="Department Name EN" />
                                            </Columns>
                                            <EmptyDataTemplate>
                                                No Data.
                                            </EmptyDataTemplate>
                                            <EmptyDataRowStyle HorizontalAlign="Center" />
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
                                <asp:Button ID="btn_resetDepartment" runat="server" CssClass="btn btn-success btn-block" OnClick="btn_resetDepartment_Click" Text="Search"></asp:Button>


                            </div>
                            <div class="col-sm-2">
                                <asp:Button ID="btn_Close_popup_SearchDepartment" runat="server" CssClass="btn btn-danger btn-block" OnClick="btn_Close_popup_SearchDepartment_Click" Text="Close"></asp:Button>
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
            <asp:Label Text="Report of Document" runat="server" style="font-size:35px !important;" CssClass="lbl_Title"></asp:Label>
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
                                    <asp:Label ID="Label32" runat="server" Text="Report of Document" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label33" runat="server" Text="รายงานสถิติเอกสาร" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <asp:Panel runat="server" ID="collapseReport" class="panel-collapse collapse in" Style="overflow: hidden">
                            <div class="panel-body">
                                <div class="row">
                                    <%-- Infomation --%>
                                    <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:Label ID="Label48" runat="server" Text="Informaion" Style="font-size: 20px;" CssClass="lbl_ENG"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                                <asp:Label ID="Label1" runat="server" Text="Category" CssClass="lbl_ENG"></asp:Label>
                                                <span class="red">*</span>
                                                <br />
                                                <asp:Label ID="Label49" runat="server" Text="หมวดหมู่เอกสาร" CssClass="lbl_THI"></asp:Label>
                                            </div>
                                            <div class="col-xs-8 col-sm-9 col-md-10 col-lg-9">
                                                <asp:DropDownList ID="ddl_Category" CssClass="form-control" runat="server">
                                                    <asp:ListItem Text="เอกสารภายในหน่วยงาน" Value="internal"></asp:ListItem>
                                                    <asp:ListItem Text="เอกสารส่วนกลาง" Value="centre"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                                <asp:Label ID="Label5" runat="server" Text="Document Type" CssClass="lbl_ENG"></asp:Label>
                                                <br />
                                                <asp:Label ID="Label6" runat="server" Text="ประเภทเอกสาร" CssClass="lbl_THI"></asp:Label>
                                            </div>
                                            <div class="col-xs-8 col-sm-9 col-md-10 col-lg-9">
                                                <asp:DropDownList ID="ddl_DocType" CssClass="form-control" runat="server">
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                                <asp:Label ID="Label8" runat="server" Text="Filter By" CssClass="lbl_ENG"></asp:Label>
                                                <br />
                                                <asp:Label ID="Label9" runat="server" Text="ค้นหาโดย" CssClass="lbl_THI"></asp:Label>
                                            </div>
                                            <div class="col-xs-8 col-sm-9 col-md-10 col-lg-9">
                                                <asp:RadioButtonList ID="rdb_FilterByEmpDept" CssClass="rbl" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rdb_FilterByEmpDept_SelectedIndexChanged"
                                                    RepeatDirection="Horizontal" RepeatLayout="Table">
                                                    <asp:ListItem Text="Employee" Value="emp" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Department" Value="dept"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                        <div class="row" runat="server" id="panel_searchEmp">
                                            <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                                <asp:Label ID="Label10" runat="server" Text="Employee" CssClass="lbl_ENG"></asp:Label>
                                                <br />
                                                <asp:Label ID="Label11" runat="server" Text="ชื่อพนักงาน" CssClass="lbl_THI"></asp:Label>
                                            </div>
                                            <div class="col-xs-8 col-sm-9 col-md-10 col-lg-9">
                                                <div class="input-group stylish-input-group">
                                                    <asp:Label CssClass="form-control " runat="server" Enabled="false" ID="lbl_Employee"></asp:Label>
                                                    <asp:HiddenField runat="server" ID="hdn_EmployeeID" />
                                                    <span class="input-group-addon">
                                                        <!-- Modal Trigger -->
                                                        <asp:LinkButton type="submit" ID="btnSearchEmployee" runat="server" OnClick="OpenPopup">
                                                            <span class="glyphicon glyphicon-search"></span>
                                                        </asp:LinkButton>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                        <div runat="server" id="panel_searchDept" visible="false">
                                            <div class="row">
                                                <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                                    <asp:Label ID="Label12" runat="server" Text="Department" CssClass="lbl_ENG"></asp:Label>
                                                    <br />
                                                    <asp:Label ID="Label13" runat="server" Text="หน่วยงาน" CssClass="lbl_THI"></asp:Label>
                                                </div>
                                                <div class="col-xs-8 col-sm-9 col-md-10 col-lg-9">
                                                    <div class="input-group stylish-input-group">
                                                        <asp:Label CssClass="form-control" runat="server" Enabled="false" ID="lbl_Department"></asp:Label>
                                                        <asp:HiddenField runat="server" ID="hdn_DepartmentID" />
                                                        <span class="input-group-addon">
                                                            <!-- Modal Trigger -->
                                                            <asp:LinkButton type="submit" ID="btnSearchDepartment" runat="server" OnClick="btn_searchDepartment_Click">
                                                            <span class="glyphicon glyphicon-search"></span>
                                                            </asp:LinkButton>
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                                    <asp:Label ID="Label14" runat="server" Text="Sub Department" CssClass="lbl_ENG"></asp:Label>
                                                    <br />
                                                    <asp:Label ID="Label15" runat="server" Text="หน่วยงานย่อย" CssClass="lbl_THI"></asp:Label>
                                                </div>
                                                <div class="col-xs-8 col-sm-9 col-md-10 col-lg-9">
                                                    <asp:DropDownList runat="server" ID="ddl_SubDepartment" CssClass="form-control">
                                                    </asp:DropDownList>
                                                   <%-- <div class="input-group stylish-input-group">
                                                        <asp:Label CssClass="form-control input-sm" runat="server" Enabled="false" ID="lbl_SubDepartment"></asp:Label>
                                                        <asp:HiddenField runat="server" ID="hdn_SubDepartmentID" />
                                                        <span class="input-group-addon">
                                                            <!-- Modal Trigger -->
                                                            <asp:LinkButton type="submit" ID="btnSearchSubDepartment" runat="server" OnClick="btn_searchDepartment_Click">
                                                            <span class="glyphicon glyphicon-search"></span>
                                                            </asp:LinkButton>
                                                        </span>
                                                    </div>--%>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <%-- Time Period --%>
                                    <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:Label ID="Label2" runat="server" Text="Time Period" Style="font-size: 20px;" CssClass="lbl_ENG"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                                <asp:Label ID="Label3" runat="server" Text="Filter By" CssClass="lbl_ENG"></asp:Label>
                                                <br />
                                                <asp:Label ID="Label4" runat="server" Text="ค้นหาโดย" CssClass="lbl_THI"></asp:Label>
                                            </div>
                                            <div class="col-xs-8 col-sm-9 col-md-10 col-lg-9">
                                                <asp:DropDownList ID="ddl_FilterByTimePeriod" CssClass="form-control" AutoPostBack="true" runat="server" OnSelectedIndexChanged="ddl_FilterByTimePeriod_SelectedIndexChanged">
                                                    <asp:ListItem Text="Daily" Value="Daily" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Weekly" Value="Weekly"></asp:ListItem>
                                                    <asp:ListItem Text="On Month" Value="Month"></asp:ListItem>
                                                    <asp:ListItem Text="Monthly" Value="Monthly"></asp:ListItem>
                                                    <asp:ListItem Text="Year" Value="Year"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <%-- Daily --%>
                                        <div class="row" runat="server" id="panel_Daily">
                                            <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                            </div>
                                            <div class="col-xs-3 col-sm-4 col-md-4 col-lg-4">
                                                <div class="inner-addon right-addon date" data-provide="datepicker">
                                                    <i class="glyphicon glyphicon-calendar"></i>
                                                    <asp:TextBox CssClass="form-control datepicker" runat="server" ID="txt_DailyFrom" AutoPostBack="true" OnTextChanged="txt_DailyFrom_TextChanged"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-xs-2 col-sm-1 col-md-2 col-lg-1">
                                                <asp:Label ID="Label18" runat="server" Text="To" Style="text-align: center;" CssClass="lbl_ENG"></asp:Label>
                                            </div>
                                            <div class="col-xs-3 col-sm-4 col-md-4 col-lg-4">
                                                <div class="inner-addon right-addon date" data-provide="datepicker">
                                                    <i class="glyphicon glyphicon-calendar"></i>
                                                    <asp:TextBox CssClass="form-control  datepicker" runat="server" ID="txt_DailyTo" AutoPostBack="true" OnTextChanged="txt_DailyTo_TextChanged"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <%-- Weekly --%>
                                        <div class="row" runat="server" id="panel_Weekly" visible="false">
                                            <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                            </div>
                                            <div class="col-xs-3 col-sm-4 col-md-4 col-lg-4">
                                                <div class="inner-addon right-addon date" data-provide="datepicker">
                                                    <i class="glyphicon glyphicon-calendar"></i>
                                                    <asp:TextBox CssClass="form-control datepicker" runat="server" ID="txt_WeeklyFrom" AutoPostBack="true" OnTextChanged="txt_WeeklyFrom_TextChanged"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-xs-2 col-sm-1 col-md-2 col-lg-1">
                                                <asp:Label ID="Label17" runat="server" Text="To" Style="text-align: center;" CssClass="lbl_ENG"></asp:Label>
                                            </div>
                                            <div class="col-xs-3 col-sm-4 col-md-4 col-lg-4">
                                                <div class="inner-addon right-addon date" data-provide="datepicker">
                                                    <i class="glyphicon glyphicon-calendar"></i>
                                                    <asp:TextBox CssClass="form-control  datepicker" runat="server" ID="txt_WeeklyTo" AutoPostBack="true" OnTextChanged="txt_WeeklyTo_TextChanged"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <%-- On Month --%>
                                        <div class="row" runat="server" id="panel_OnMonth" visible="false">
                                            <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                            </div>
                                            <div class="col-xs-5 col-sm-6 col-md-6 col-lg-6">
                                                <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_OnMonth">
                                                    <asp:ListItem Text="January" Value="1" />
                                                    <asp:ListItem Text="February" Value="2" />
                                                    <asp:ListItem Text="March" Value="3" />
                                                    <asp:ListItem Text="April" Value="4" />
                                                    <asp:ListItem Text="May" Value="5" />
                                                    <asp:ListItem Text="June" Value="6" />
                                                    <asp:ListItem Text="July" Value="7" />
                                                    <asp:ListItem Text="August" Value="8" />
                                                    <asp:ListItem Text="September" Value="9" />
                                                    <asp:ListItem Text="October" Value="10" />
                                                    <asp:ListItem Text="November" Value="11" />
                                                    <asp:ListItem Text="December" Value="12" />
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-xs-3 col-sm-3 col-md-4 col-lg-3">
                                                <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_OnMonthYear">
                                                    <asp:ListItem Text="2014" Value="2014" />
                                                    <asp:ListItem Text="2015" Value="2015" />
                                                    <asp:ListItem Text="2016" Value="2016" />
                                                    <asp:ListItem Text="2017" Value="2017" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <%-- Monthly --%>
                                        <div runat="server" id="panel_Monthly" visible="false">
                                            <div class="row">
                                                <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                                </div>
                                                <div class="col-xs-8 col-sm-9 col-md-10 col-lg-9">
                                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_MonthlyYear">
                                                        <asp:ListItem Text="2014" Value="2014" />
                                                        <asp:ListItem Text="2015" Value="2015" />
                                                        <asp:ListItem Text="2016" Value="2016" />
                                                        <asp:ListItem Text="2017" Value="2017" />
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                                </div>
                                                <div class="col-xs-3 col-sm-4 col-md-4 col-lg-4">
                                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_FromMonth" AutoPostBack="true" OnSelectedIndexChanged="ddl_FromMonth_SelectedIndexChanged">
                                                        <asp:ListItem Text="January" Value="1" />
                                                        <asp:ListItem Text="February" Value="2" />
                                                        <asp:ListItem Text="March" Value="3" />
                                                        <asp:ListItem Text="April" Value="4" />
                                                        <asp:ListItem Text="May" Value="5" />
                                                        <asp:ListItem Text="June" Value="6" />
                                                        <asp:ListItem Text="July" Value="7" />
                                                        <asp:ListItem Text="August" Value="8" />
                                                        <asp:ListItem Text="September" Value="9" />
                                                        <asp:ListItem Text="October" Value="10" />
                                                        <asp:ListItem Text="November" Value="11" />
                                                        <asp:ListItem Text="December" Value="12" />
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-xs-2 col-sm-1 col-md-2 col-lg-1">
                                                    <asp:Label ID="Label19" runat="server" Text="To" Style="text-align: center;" CssClass="lbl_ENG"></asp:Label>
                                                </div>
                                                <div class="col-xs-3 col-sm-4 col-md-4 col-lg-4">
                                                    <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_ToMonth" AutoPostBack="true" OnSelectedIndexChanged="ddl_ToMonth_SelectedIndexChanged">
                                                        <asp:ListItem Text="January" Value="1" />
                                                        <asp:ListItem Text="February" Value="2" />
                                                        <asp:ListItem Text="March" Value="3" />
                                                        <asp:ListItem Text="April" Value="4" />
                                                        <asp:ListItem Text="May" Value="5" />
                                                        <asp:ListItem Text="June" Value="6" />
                                                        <asp:ListItem Text="July" Value="7" />
                                                        <asp:ListItem Text="August" Value="8" />
                                                        <asp:ListItem Text="September" Value="9" />
                                                        <asp:ListItem Text="October" Value="10" />
                                                        <asp:ListItem Text="November" Value="11" />
                                                        <asp:ListItem Text="December" Value="12" />
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <%-- Year --%>
                                        <div class="row" runat="server" id="panel_Year" visible="false">
                                            <div class="col-xs-4 col-sm-3 col-md-2 col-lg-3">
                                            </div>
                                            <div class="col-xs-3 col-sm-4 col-md-4 col-lg-4">
                                                <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_FromYear" AutoPostBack="true" OnSelectedIndexChanged="ddl_FromYear_SelectedIndexChanged">
                                                    <asp:ListItem Text="2017" Value="2017" />
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-xs-2 col-sm-1 col-md-2 col-lg-1">
                                                <asp:Label ID="Label20" runat="server" Text="To" Style="text-align: center;" CssClass="lbl_ENG"></asp:Label>
                                            </div>
                                            <div class="col-xs-3 col-sm-4 col-md-4 col-lg-4">
                                                <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_ToYear" AutoPostBack="true" OnSelectedIndexChanged="ddl_ToYear_SelectedIndexChanged">
                                                    <asp:ListItem Text="2017" Value="2017" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2 col-md-2 col-lg-4"></div>
                                    <div class="col-xs-6 col-sm-4 col-md-4 col-lg-2">
                                        <asp:Button Text="Search" ID="btn_Submit" CssClass="CssButton btn btn-success" runat="server" OnClick="btn_Submit_Click" />
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-md-4 col-lg-2">
                                        <asp:Button Text="Reset" ID="btn_Reset" CssClass="CssButton btn btn-alert" runat="server" OnClick="btn_Reset_Click" />
                                    </div>
                                    <div class="col-sm-2 col-md-2 col-lg-4"></div>
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                </div>

                <%-- Report Panel --%>
                <div runat="server" class="panel-group" id="panel_Report">
                    <div class="panel panel-default">
                        <div class="table-responsive">
                            <div id="ShowGraph" runat="server" class="col-lg-12"></div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Panel>
