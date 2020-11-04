<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MappingSecretaryUserControl.ascx.cs" Inherits="PIMEdoc_CR.MappingSecretary.MappingSecretaryUserControl" %>
<%@ Register TagPrefix="uc1" TagName="UpdateProgress" Src="../../PIMEdoc/UpdateProgress.ascx" %>
<link rel="stylesheet" href="../CSS/bootstrap.min.css" />
<link rel="stylesheet" href="../CSS/jquery-ui.css" />
<link rel="stylesheet" href="../CSS/Styles.css" />

<script src="../JS/jquery-1.10.2.js"></script>
<script src="../JS/bootstrap.js"></script>
<script src="../JS/jquery-ui.js"></script>
<style>
</style>

<div class="container" style="max-width: 1200px; width: 100%;">

    <uc1:UpdateProgress runat="server" ID="UpdateProgress1"></uc1:UpdateProgress>
    <!-- modal Employee Search -->
    <div class="modal fade" id="searchEmpReqModal" role="dialog">
        <div class="modal-dialog" style="width: 80%; max-width: 1000px;">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <Triggers>
                </Triggers>
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
                                <div class="col-sm-2"></div>
                                <div class="col-sm-2">
                                    <asp:Button ID="btn_closeBtn" runat="server" CssClass="btn btn-danger btn-block" OnClick="btnClosePopup_Click" Text="Close"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>


        </div>
    </div>
    <div id="divBody" runat="server">
        <!-- Mapping Secretary Header -->
        <div style="width: 100%; height: auto !important">
            <asp:Label Text="Mapping Secretary" runat="server" style="font-size:35px !important;" CssClass="lbl_Title"></asp:Label>
            <div style="border-top: 1px solid #FBB300; margin-bottom: 10px;">
            </div>
        </div>
        <asp:UpdatePanel ID="updatePanel_user" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="chk_especially" />
                <asp:PostBackTrigger ControlID="ddl_Department" />
                <asp:PostBackTrigger ControlID="ddl_SubDepartment" />
            </Triggers>
            <ContentTemplate>
                <!-- Mapping Secretary Panel -->
                <div runat="server" class="panel-group" id="panelSecretary">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseSecretary">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapseSecretary">
                                    <asp:Label ID="Label32" runat="server" Text="Upload Signature Image" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label33" runat="server" Text="หน้าเพิ่มผูกข้อมูลเลขา" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <asp:Panel runat="server" ID="collapseSecretary" class="panel-collapse collapse in" Style="overflow: hidden">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label48" runat="server" Text="Employee" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label49" runat="server" Text="ชื่อพนักงาน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-8">
                                        <div class="input-group stylish-input-group">
                                            <asp:Label CssClass="form-control" runat="server" ID="lbl_EmployeeName"></asp:Label>
                                            <asp:HiddenField Value="" runat="server" ID="hdn_EmplyeeID" />
                                            <span class="input-group-addon">
                                                <!-- Modal Trigger -->
                                                <asp:LinkButton type="submit" ID="btnSearchEmployee" runat="server" Enabled="true" OnClick="OpenPopup">
                                                        <span class="glyphicon glyphicon-search"></span>
                                                </asp:LinkButton>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:CheckBox ID="chk_especially" runat="server" Text="Especially" AutoPostBack="true" OnCheckedChanged="chk_especially_CheckedChanged" />
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label1" runat="server" Text="Department" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label2" runat="server" Text="หน่วยงานหลัก" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-8">
                                        <asp:DropDownList runat="server" ID="ddl_Department" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddl_Department_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2"></div>
                                </div>
                                <div runat="server" id="panel_especially" visible="false">
                                    <div class="row">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label5" runat="server" Text="SubDepartment" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label6" runat="server" Text="หน่วยงานย่อย" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-8">
                                            <asp:DropDownList runat="server" ID="ddl_SubDepartment" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddl_SubDepartment_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col-sm-2"></div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label7" runat="server" Text="Position" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label8" runat="server" Text="ตำแหน่งงาน" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-8">
                                            <asp:DropDownList runat="server" ID="ddl_Position" CssClass="form-control" OnSelectedIndexChanged="ddl_Position_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col-sm-2"></div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label3" runat="server" Text="Secretary" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label4" runat="server" Text="ชื่อเลขา" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-8">
                                        <div class="input-group stylish-input-group">
                                            <asp:Label CssClass="form-control" runat="server" ID="lbl_Secretary"></asp:Label>
                                            <asp:HiddenField Value="" runat="server" ID="hdn_SecretaryID" />
                                            <span class="input-group-addon">
                                                <!-- Modal Trigger -->
                                                <asp:LinkButton type="submit" ID="btn_searchSecretaryEmp" runat="server" Enabled="true" OnClick="OpenPopup">
                                                        <span class="glyphicon glyphicon-search"></span>
                                                </asp:LinkButton>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="col-sm-2"></div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-2"></div>
                                    <div class="col-sm-2">
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button Text="Submit" ID="btn_Submit" CssClass="CssButton" OnClick="btn_Submit_Click" runat="server" />

                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button Text="Reset" ID="btn_Reset" CssClass="CssButton" OnClick="btn_Reset_Click" runat="server" />

                                    </div>
                                    <div class="col-sm-2"></div>
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                </div>

                <div class="row" runat="server" id="panel_gvSecretary" visible="false">
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gv_Secretary" OnRowDataBound="gv_Secretary_RowDataBound" OnRowCommand="gv_Secretary_RowCommand"
                            AutoGenerateColumns="false" RowStyle-Wrap="true" EmptyDataRowStyle-HorizontalAlign="Center"
                            ShowHeaderWhenEmpty="true" CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                            <Columns>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                            <span>Employee ID<br />
                                                รหัสพนักงาน</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("EmpID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                            <span>Employee Name<br />
                                                ชื่อพนักงาน</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbl_EmployeeName"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                            <span>Department Name<br />
                                                หน่วยงานหลัก</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbl_Department"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                            <span>Secretary ID<br />
                                                รหัสเลขา</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("SecretaryID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                            <span>Secretary Name<br />
                                                ชื่อเลขา</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbl_Secretary"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                            <span>Especially<br />
                                                เคสพิเศษ</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbl_Especially" Text='<%# Eval("Especially") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                            <span>Modified Date<br />
                                                วันที่แก้ไข</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lbl_ModifiedDate" ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div style="text-align: center; padding: 5px 0 5px 0;">
                                            <span>Delete<br />
                                                ลบ</span>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Button CssClass="Img_Delete" runat="server" ID="btn_DeleteRow" OnClientClick="return confirm('Are you sure you want to delete this item?');" CommandName="DeleteItem" CommandArgument='<%# Eval("Row") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>


            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
