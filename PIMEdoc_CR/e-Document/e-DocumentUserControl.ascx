<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>

<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" ValidateRequestMode="Disabled" CodeBehind="e-DocumentUserControl.ascx.cs" Inherits="PIMEdoc_CR.Default.e_Document.e_DocumentUserControl" %>
<%@ Register TagPrefix="uc1" TagName="UpdateProgress" Src="../../PIMEdoc/UpdateProgress.ascx" %>


<link rel="stylesheet" type="text/css" href="../CSS/bootstrap.min.css" />
<link rel="stylesheet" type="text/css" href="../CSS/jquery-ui.css" />
<link rel="stylesheet" type="text/css" href="../CSS/Styles.css" />

<!-- Do not Deploy To PRD -->
<%--<script src="../JS/jquery-3.2.1.min.js"></script>--%>
<%--<script type="text/javascript" src="../JS/jquery.min.js"></script>
<script type="text/javascript" src="../JS/bootstraps.js"></script>--%>
<!-- Do not Deploy To PRD -->

<script type="text/javascript" src="../JS/jquery-ui.js"></script>



<style>
    .invalid-tb, .invalid-tb:focus, .invalid-tb:hover {
        border-color: red !important;
        box-shadow: 0px 0px 10px 0px rgba(255,0,0,0.75) !important;
    }



    .custom-combobox {
        position: relative !important;
        display: inline-block !important;
        width: 100% !important;
    }

    .custom-combobox-toggle {
        position: absolute !important;
        top: 0 !important;
        bottom: 0 !important;
        margin-left: -32px !important;
        padding: 0 !important;
    }

    .custom-combobox-input {
        margin: 0 !important;
        padding: 5px 10px !important;
        width: 100% !important;
    }

    .ui-state-default,
    .ui-widget-content .ui-state-default,
    .ui-widget-header .ui-state-default {
        background: #ffffff !important;
        font-weight: normal;
        color: #555555;
    }

    .displaynone {
        display: none !important;
    }

    .ui-datepicker {
        z-index: 1151 !important;
    }

    select {
        /*font-size: 15px !important;*/
    }

    .notifyPop {
        position: fixed;
        left: 50vw;
        margin-left: 35px;
        top: 50vh;
        margin-top: -210px;
        opacity: 0;
        transition: all 1s;
        -webkit-transition: all 1s;
    }

        .notifyPop div {
            width: 120px;
            font-size: 12px;
            padding: 50px;
            background-color: black;
            border-radius: 40px;
            color: white;
            text-align: center;
        }

    .notifyBtn:hover ~ .notifyPop {
        margin-left: 45px;
        margin-top: -220px;
        opacity: 1;
    }
</style>
<script>
    (function ($) {
        $.widget("custom.combobox", {
            _create: function () {
                this.wrapper = $("<span>")
                    .addClass("custom-combobox")
                    .insertAfter(this.element);

                this.element.hide();
                this._createAutocomplete();
                this._createShowAllButton();
            },

            _createAutocomplete: function () {
                var selected = this.element.children(":selected"),
                    value = selected.val() ? selected.text() : "";

                this.input = $("<input>")
                    .appendTo(this.wrapper)
                    .val(value)
                    .attr("title", "")
                    .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
                    .autocomplete({
                        delay: 0,
                        minLength: 0,
                        source: $.proxy(this, "_source")
                    })
                    .tooltip({
                        tooltipClass: "ui-state-highlight"
                    });

                this._on(this.input, {
                    autocompleteselect: function (event, ui) {
                        ui.item.option.selected = true;
                        this._trigger("select", event, {
                            item: ui.item.option
                        });

                        __doPostBack(this.element.attr("id"), this.element.attr("id"));
                    },

                    autocompletechange: "_removeIfInvalid"
                });
            },

            _createShowAllButton: function () {
                var input = this.input,
                    wasOpen = false;

                $("<a>")
                    .attr("tabIndex", -1)
                    //.attr("title", "Show All Items")
                    //.tooltip()
                    .appendTo(this.wrapper)
                    .button({
                        icons: {
                            primary: "ui-icon-triangle-1-s"
                        },
                        text: false
                    })
                    .removeClass("ui-corner-all")
                    .addClass("custom-combobox-toggle ui-corner-right")
                    .mousedown(function () {
                        wasOpen = input.autocomplete("widget").is(":visible");
                    })
                    .click(function () {
                        input.focus();

                        // Close if already visible
                        if (wasOpen) {
                            return;
                        }

                        // Pass empty string as value to search for, displaying all results
                        input.autocomplete("search", "");
                    });
            },

            _source: function (request, response) {
                var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                response(this.element.children("option").map(function () {
                    var text = $(this).text();
                    if (this.value && (!request.term || matcher.test(text)))
                        return {
                            label: text,
                            value: text,
                            option: this
                        };
                }));
            },

            _removeIfInvalid: function (event, ui) {

                // Selected an item, nothing to do
                if (ui.item) {
                    return;
                }

                // Search for a match (case-insensitive)
                var value = this.input.val(),
                    valueLowerCase = value.toLowerCase(),
                    valid = false;
                this.element.children("option").each(function () {
                    if ($(this).text().toLowerCase() === valueLowerCase) {
                        this.selected = valid = true;
                        return false;
                    }
                });

                // Found a match, nothing to do
                if (valid) {
                    return;
                }

                // Remove invalid value
                if (value != '') {
                    this.input
                        .val("")
                        .attr("title", value + " didn't match any item")
                        .tooltip("open");
                    this.element.val("");
                    this._delay(function () {
                        this.input.tooltip("close").attr("title", "");
                    }, 2500);
                    this.input.data("ui-autocomplete").term = "";
                } else {
                    this.input.val("");
                    this.element.val("");
                    this.input.data("ui-autocomplete").term = "";
                }
                __doPostBack(this.element.attr("id"), this.element.attr("id"));
            },

            _destroy: function () {
                this.wrapper.remove();
                this.element.show();
            }
        });
        $.ui.autocomplete.prototype._renderItem = function (ul, item) {
            var term = this.term.split(' ').join('|');
            var re = new RegExp("(" + term + ")", "gi");
            var t = item.label.replace(re, "<b style=\"background-color:yellow;\">$1</b>");
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append("<a>" + t + "</a>")
                .appendTo(ul);
        };
    })(jQuery);

    function pageLoad() {

        var stats = document.getElementById('<%= lbl_Status.ClientID %>');
        if (stats.textContent != null) {
            if (stats.textContent == "New Request" || stats.textContent == "Draft" || stats.textContent == "Rework" || stats.textContent == "Wait for Requestor Review") {
                <%--$('#<%=ddl_DocType.ClientID %>').combobox();--%>
                if (stats.textContent != "Wait for Requestor Review") {
                   <%-- $('#<%=ddl_Priority.ClientID %>').combobox();--%>
                    <%--$('#<%=ddlFromDepartment.ClientID %>').combobox();
                    $('#<%=ddlToDepartment.ClientID %>').combobox();--%>
                }
            }
        }
        $('.datepicker').datepicker({
            dateFormat: 'dd/mm/yy'
        });
    }</script>

<script type="text/javascript">
    function UploadDoc(fileUpload) {
        if (fileUpload.value != '') {
            document.getElementById("<%=btn_DocumentFileUpload.ClientID %>").click();
        }
    }
    function UploadFile(fileUpload) {
        if (fileUpload.value != '') {
            document.getElementById("<%=btn_AttachUpload.ClientID %>").click();
        }
    }
    function SearchDept() {
        document.getElementById("<%=btn_searchDepartment.ClientID %>").click();
    }
</script>


<uc1:UpdateProgress runat="server" ID="UpdateProgress"></uc1:UpdateProgress>


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
                                            <asp:TextBox runat="server" ID="txtSearch_Department" CssClass="form-control " placeholder="Search"></asp:TextBox>

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
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:Label Text="Department Acronym" runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_DepartmentAcronym" runat="server"></asp:Label>
                                                        <asp:HiddenField runat="server" ID="hdnDepartmentAcronym" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:Label Text="DEPARTMENT NAME TH" runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label Text='<%# Eval("DEPARTMENT_NAME_TH") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:Label Text="DEPARTMENT NAME EN" runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label Text='<%# Eval("DEPARTMENT_NAME_EN") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:Label Text="GROUP EMAIL" runat="server"></asp:Label>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label Text='<%# Eval("DEPARTMENT_GROUPMAIL") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="DEPARTMENT_CODE" HeaderText="Department Code" />--%>
                                                <%--<asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="DEPARTMENT_NAME_TH" HeaderText="Department Name TH" />--%>
                                                <%--<asp:BoundField ItemStyle-HorizontalAlign="Left" DataField="DEPARTMENT_NAME_EN" HeaderText="Department Name EN" />--%>
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
                        <%-- </div>
                    <div class="modal-footer">--%>
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
<!-- modal Document Search -->
<div class="modal fade" id="searchDocModal" role="dialog">
    <div class="modal-dialog" style="width: 80%; max-width: 1000px;">
        <asp:UpdatePanel ID="upDocModal" runat="server">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <div class="row">
                            <div class="col-xs-9 col-sm-8 form-horizontal">
                                <h4 class="modal-title">Search Document
                                </h4>
                            </div>
                            <div class="col-xs-3 col-sm-4 form-horizontal " style="text-align: right">
                                <asp:Label ID="Label89" Visible="false" runat="server"></asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="panel-collapse collapse in">

                            <asp:Panel runat="server" CssClass="panel-body" DefaultButton="btn_search">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="Search<br/>ค้นหา" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox runat="server" ID="txt_searchDoc" CssClass="form-control " placeholder="Search"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="Create Date From<br/>วันที่" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_CreateDateFrom" AutoPostBack="true" CssClass="form-control datepicker"></asp:TextBox>

                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="Create Date To<br/>ถึงวันที่" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_CreateDateTo" AutoPostBack="true" CssClass="form-control datepicker"></asp:TextBox>

                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="From Department<br/>หน่วยงานตั้งต้น" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList runat="server" ID="ddlFromDepartment" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="To Departmtent<br/>หน่วยงานรับเรื่อง" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList runat="server" ID="ddlToDepartment" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="Document Type<br/>ประเภทเอกสาร" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList runat="server" ID="ddlDocumentType" CssClass="form-control">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="หนังสือประกาศ" Value="ประกาศ"></asp:ListItem>
                                            <asp:ListItem Text="หนังสือคำสั่ง" Value="คำสั่ง"></asp:ListItem>
                                            <asp:ListItem Text="หนังสือระเบียบ" Value="ระเบียบ"></asp:ListItem>
                                            <asp:ListItem Text="หนังสือข้อบังคับ" Value="ข้อบังคับ"></asp:ListItem>
                                            <asp:ListItem Text="จดหมายออกภายนอก" Value="จดหมายออกภายนอก"></asp:ListItem>
                                            <asp:ListItem Text="อื่นๆ" Value="อื่นๆ"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gv_searchDocTable" OnPageIndexChanging="gv_searchDocTable_PageIndexChanging"
                                            OnRowDataBound="gv_searchDocTable_RowDataBound" OnSelectedIndexChanged="gv_searchDocTable_SelectedIndexChanged"
                                            AutoGenerateColumns="false" AllowPaging="true" PageSize="5" RowStyle-Wrap="false" EmptyDataRowStyle-HorizontalAlign="Center"
                                            ShowHeaderWhenEmpty="true" CssClass=" table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                            <Columns>
                                                <%--<asp:BoundField HeaderText="Doc No." DataField="DocNo" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" />--%>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Doc No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDocNo" runat="server" Text="-"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Document Type">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnDocNo" runat="server" />
                                                        <asp:Label ID="lblDocType" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Title" DataField="Title" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left" />
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Create Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCreateDate" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Creator">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCreator" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="From Department">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblFromDepartment" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="To Department">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblToDepartment" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server"></asp:Label>
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
                                <br />
                                <div class="row">
                                    <div class="col-sm-6"></div>
                                    <div class="col-sm-2">
                                        <asp:Button runat="server" ID="btn_search" OnClick="btn_searchDoc_Click" Text="Search" CssClass="btn btn-success btn-block" />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btn_Close" runat="server" CssClass="btn btn-danger btn-block" OnClick="btnClosePopupSearchDoc_Click" Text="Close"></asp:Button>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btn_ResetDoc" runat="server" CssClass="btn btn-danger btn-block" OnClick="btn_ResetDoc_OnClick" Text="Clear"></asp:Button>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                    <%-- <div class="modal-footer">
                        
                    </div>--%>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<!-- Popup Add Advisor -->
<div class="modal fade" id="searchAdvisor" role="dialog">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="upAdvisor" runat="server">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <div class="row">
                            <div class="col-xs-9 col-sm-8 form-horizontal">
                                <h4 class="modal-title">Request Comment</h4>
                            </div>
                            <div class="col-xs-3 col-sm-4 form-horizontal " style="text-align: right">
                            </div>
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="panel-collapse collapse in">
                            <asp:Panel runat="server" CssClass="panel-body" DefaultButton="lkbtnSearchAdvisor">
                                <div class="row">
                                    <div class="col-sm-4">
                                        <asp:Label runat="server" Text="Request Comment<br/>ขอความคิดเห็น" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-8">
                                        <div class="input-group stylish-input-group">
                                            <asp:TextBox runat="server" ID="txt_RequestComment" CssClass="form-control" placeholder="Request Comment"></asp:TextBox>
                                            <span class="input-group-addon">
                                                <asp:LinkButton type="submit" ID="lkbtnSearchAdvisor" runat="server" OnClick="lkbtnSearchAdvisor_Click">
                                                            <span class="glyphicon glyphicon-search"></span>
                                                </asp:LinkButton>
                                            </span>
                                        </div>
                                        <asp:TextBox runat="server" CssClass="displaynone" ID="txt_RequestCommentEMPID"></asp:TextBox>
                                        <asp:TextBox runat="server" CssClass="displaynone" ID="txt_RequestCommentDEPID"></asp:TextBox>
                                    </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-sm-8"></div>
                            <div class="col-sm-2">
                                <asp:Button ID="btnSubmitToRequestComment" runat="server" CssClass="btn btn-danger btn-block" OnClick="btnSubmitToRequestComment_Click" Text="Submit"></asp:Button>
                            </div>
                            <div class="col-sm-2">
                                <asp:Button ID="btnCancelRequestComment" runat="server" CssClass="btn btn-danger btn-block" OnClick="btnCancelRequestComment_Click" Text="Cancel"></asp:Button>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<!-- Popup Assign -->
<div class="modal fade" id="searchAssign" role="dialog">
    <div class="modal-dialog" style="width: 80%">
        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <div class="row">
                            <div class="col-xs-9 col-sm-8 form-horizontal">
                                <h4 class="modal-title">Assign</h4>
                            </div>
                            <div class="col-xs-3 col-sm-4 form-horizontal " style="text-align: right">
                            </div>
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="panel-collapse collapse in">
                            <asp:Panel runat="server" CssClass="panel-body" DefaultButton="lkbtnSearchAdvisor">
                                <div class="row">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gv_Assign" OnRowCommand="gv_Assign_RowCommand" OnRowDataBound="gv_Assign_RowDataBound" OnSelectedIndexChanged="gv_Assign_SelectedIndexChanged"
                                            AutoGenerateColumns="false" ShowFooter="true" AllowPaging="true" PageSize="5" RowStyle-Wrap="false" EmptyDataRowStyle-HorizontalAlign="Center" OnPageIndexChanging="gv_Assign_PageIndexChanging"
                                            ShowHeaderWhenEmpty="true" CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label Text='<%# Eval("Sequence") %>' runat="server" />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:LinkButton type="submit" ID="lkbtnSearchAssign" runat="server" OnClick="lkbtnSearchAssign_Click">
                                                            <span class="glyphicon glyphicon-plus"></span>
                                                        </asp:LinkButton>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="EmployeeID" DataField="EmpId" ItemStyle-HorizontalAlign="Center" />
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Employee Name">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnEmpID" runat="server" Value='<%# Eval("EmpId") %>'></asp:HiddenField>
                                                        <asp:Label ID="lblEmpName" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Department">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnDeptID" runat="server" Value='<%# Eval("DepartmentID") %>'></asp:HiddenField>
                                                        <asp:DropDownList ID="ddl_Department" CssClass="form-control" OnSelectedIndexChanged="ddl_AssignDepartment_OnSelectedIndexChanged" runat="server"></asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Email" DataField="Email" ItemStyle-HorizontalAlign="Center" />
                                                <asp:BoundField HeaderText="Tel" DataField="Tel" ItemStyle-HorizontalAlign="Center" />

                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button CssClass="Img_Delete" runat="server" ID="btn_DeleteRow" OnClientClick="return confirm('Are you sure you want to delete this item?');" CommandName="DeleteItem" CommandArgument='<%# Eval("Sequence") %>' />

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <asp:LinkButton type="submit" ID="lkbtnSearchAssign" runat="server" OnClick="lkbtnSearchAssign_Click">
                                                            <span class="glyphicon glyphicon-plus"></span>
                                                </asp:LinkButton>
                                            </EmptyDataTemplate>
                                            <FooterStyle HorizontalAlign="Center" />
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
                                <asp:Button ID="btn_SubmitAssign" runat="server" CssClass="btn btn-success btn-block" OnClick="btnSubmitToAssign_Click" Text="Submit"></asp:Button>
                            </div>
                            <div class="col-sm-2">
                                <asp:Button ID="btn_CalcelAssign" runat="server" CssClass="btn btn-danger btn-block" OnClick="btnCancelAssign_Click" Text="Cancel"></asp:Button>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<!-- Popup ToDepartment -->
<div class="modal fade" id="toDepartmentModal" role="dialog">
    <div class="modal-dialog" style="width: 80%">
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <div class="row">
                            <div class="col-xs-9 col-sm-8 form-horizontal">
                                <h4 class="modal-title">To Department</h4>
                            </div>
                            <div class="col-xs-3 col-sm-4 form-horizontal " style="text-align: right">
                            </div>
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="panel-collapse collapse in">
                            <asp:Panel runat="server" CssClass="panel-body" DefaultButton="btn_ToDeptAdd">
                                <div class="row">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gv_ToDepartment" OnRowCommand="gv_ToDepartment_RowCommand" OnRowDataBound="gv_ToDepartment_RowDataBound"
                                            AutoGenerateColumns="false" ShowFooter="false" AllowPaging="true" PageSize="5" RowStyle-Wrap="false" EmptyDataRowStyle-HorizontalAlign="Center" OnPageIndexChanging="gv_ToDepartment_PageIndexChanging"
                                            ShowHeaderWhenEmpty="true" CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label Text='<%# Eval("Sequence") %>' runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Department Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDepCode" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Department Name TH">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnDeptID" runat="server" Value='<%# Eval("DeptID") %>'></asp:HiddenField>
                                                        <asp:Label ID="lblDepNameTH" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Department Name EN">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDepNameEN" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button CssClass="Img_Delete" runat="server" ID="btn_DeleteRow" OnClientClick="return confirm('Are you sure you want to delete this item?');" CommandName="DeleteItem" CommandArgument='<%# Eval("Sequence") %>' />

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle HorizontalAlign="Center" />
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
                                <asp:Button ID="btn_ToDeptAdd" runat="server" CssClass="btn btn-success btn-block" OnClick="btn_searchDepartment_Click" Text="Add Department"></asp:Button>
                            </div>
                            <div class="col-sm-2">
                                <asp:Button ID="btn_ToDeptClose" runat="server" CssClass="btn btn-danger btn-block" OnClick="btn_ToDeptClose_Click" Text="Close"></asp:Button>
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
        <!-- e-Document Header -->
        <div style="width: 100%; margin: 30px auto 0 auto; height: auto !important">
            <asp:Label Text="e-Document" runat="server" Style="font-size: 35px !important;" CssClass="lbl_Title"></asp:Label>
            <div style="border-top: 1px solid #FBB300; margin-bottom: 10px;">
            </div>
        </div>
        <asp:UpdatePanel ID="updatePanel_user" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="ddl_CreatorDepartment" />
                <asp:PostBackTrigger ControlID="ddl_CreatorSubDepartment" />
                <asp:PostBackTrigger ControlID="ddl_CreatorPosition" />
                <asp:PostBackTrigger ControlID="ddl_RequestorDepartment" />
                <asp:PostBackTrigger ControlID="ddl_RequestorSubDepartment" />
                <asp:PostBackTrigger ControlID="ddl_RequestorPosition" />
            </Triggers>
            <ContentTemplate> 
                <asp:Button ID="btnSendMail" runat="server" Text="Send Mail Test" OnClick="btnSendMail_Click" Style="display: none"  />
                <!-- Creator Panel -->
                <div runat="server" class="panel-group" id="panelCreator">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseCreator">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapseCreator">
                                    <asp:Label ID="Label32" runat="server" Text="Creator" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label33" runat="server" Text="ผู้อัพโหลดเอกสาร" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <asp:Panel runat="server" ID="collapseCreator" class="panel-collapse collapse in" Style="overflow: hidden">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label34" runat="server" Text="Emp ID" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label35" runat="server" Text="รหัสพนักงาน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:Label CssClass="form-control" runat="server" ID="lbl_CreatorID"></asp:Label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label40" runat="server" Text="Department" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label41" runat="server" Text="หน่วยงานหลัก" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <%--<asp:TextBox CssClass="form-control input-sm" runat="server" ID="txt_CreatorDepartment"></asp:TextBox>--%>
                                        <asp:DropDownList runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_CreatorDepartment_SelectedIndexChanged" ID="ddl_CreatorDepartment" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label36" runat="server" Text="Name-Surname" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label37" runat="server" Text="ชื่อ - นามสกุล" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:Label CssClass="form-control" runat="server" ID="lbl_CreatorName"></asp:Label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label42" runat="server" Text="Sub Department" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label43" runat="server" Text="หน่วยงานย่อย" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <%--<asp:TextBox CssClass="form-control input-sm" runat="server" ID="txt_CreatorDivision"></asp:TextBox>--%>
                                        <asp:DropDownList runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_CreatorSubDepartment_SelectedIndexChanged" ID="ddl_CreatorSubDepartment" CssClass="form-control"></asp:DropDownList>
                                    </div>

                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label44" runat="server" Text="Telephone" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label45" runat="server" Text="เบอร์โทร" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:Label CssClass="form-control " runat="server" ID="lbl_CreatorTel"></asp:Label>
                                    </div>

                                    <div class="col-sm-2">
                                        <asp:Label ID="Label38" runat="server" Text="Position" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label39" runat="server" Text="ตำแหน่งงาน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <%--<asp:TextBox CssClass="form-control input-sm" runat="server" ID="txt_CreatorPosition"></asp:TextBox>--%>
                                        <asp:DropDownList runat="server" AutoPostBack="true" ID="ddl_CreatorPosition" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                </div>

                <!-- Requester Panel -->
                <div runat="server" class="panel-group" id="panelRequestor">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseRequestor">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapseCreator">
                                    <asp:Label ID="Label46" runat="server" Text="Requestor" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label47" runat="server" Text="ผู้ร้องขอเอกสาร" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <asp:Panel runat="server" ID="collapseRequestor" class="panel-collapse collapse in" Style="overflow: hidden">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label48" runat="server" Text="Emp ID" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <!-- Modal Trigger -->
                                        <asp:Label ID="Label49" runat="server" Text="รหัสพนักงาน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="input-group stylish-input-group">
                                            <asp:Label CssClass="form-control " runat="server" Enabled="false" ID="lbl_RequestorID"></asp:Label>
                                            <span class="input-group-addon">
                                                <!-- Modal Trigger -->
                                                <asp:LinkButton type="submit" ID="btnSearchEmployee" runat="server" Enabled="true" OnClick="OpenPopup">
                                                        <span class="glyphicon glyphicon-search"></span>
                                                </asp:LinkButton>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label54" runat="server" Text="Department" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label55" runat="server" Text="หน่วยงานหลัก" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <%-- <asp:TextBox CssClass="form-control input-sm" runat="server" Enabled="false" ID="txt_RequestorDepartment"></asp:TextBox>--%>
                                        <asp:DropDownList runat="server" OnSelectedIndexChanged="ddl_RequestorDepartment_SelectedIndexChanged" AutoPostBack="true" ID="ddl_RequestorDepartment" CssClass="form-control"></asp:DropDownList>

                                    </div>

                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label50" runat="server" Text="Name-Surname" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label51" runat="server" Text="ชื่อ - นามสกุล" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:Label CssClass="form-control " runat="server" ID="lbl_RequestorName"></asp:Label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label56" runat="server" Text="Sub Department" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label57" runat="server" Text="หน่วยงานย่อย" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <%--<asp:TextBox CssClass="form-control input-sm" runat="server" Enabled="false" ID="txt_RequestorDivision"></asp:TextBox>--%>
                                        <asp:DropDownList runat="server" OnSelectedIndexChanged="ddl_RequestorSubDepartment_SelectedIndexChanged" AutoPostBack="true" ID="ddl_RequestorSubDepartment" CssClass="form-control"></asp:DropDownList>
                                    </div>


                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label58" runat="server" Text="Telephone" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label59" runat="server" Text="เบอร์โทร" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:Label CssClass="form-control" runat="server" Enabled="false" ID="lbl_RequestorTel"></asp:Label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label52" runat="server" Text="Position" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label53" runat="server" Text="ตำแหน่งงาน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList runat="server" AutoPostBack="true" ID="ddl_RequestorPosition" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-offset-8 col-sm-4">
                                        <asp:CheckBox CssClass="rbl" AutoPostBack="true" ID="chkDocNoBySubDepartment" OnCheckedChanged="chkDocNoBySubDepartment_CheckedChanged" Visible="false" Text="Occur by Sub-Department<br/>ออกเลขที่เอกสารในนามของศูนย์" runat="server" />
                                    </div>
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
        <!-- Information Panel-->
        <asp:Panel runat="server" class="panel-group" DefaultButton="btn5" ID="panelInfomation">
            <div class="panel panel-default">
                <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseInfo">
                    <h4 class="panel-title">
                        <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapseInfo">
                            <asp:Label ID="Label1" runat="server" Text="Information" CssClass="TableStyleENG_Head"></asp:Label>
                            <br />
                            <asp:Label ID="Label2" runat="server" Text="รายละเอียดข้อมูลเอกสาร" CssClass="TableStyleTH_Head"></asp:Label>
                        </a>
                    </h4>
                </div>
                <div id="collapseInfo" class="panel-collapse collapse in" runat="server" style="overflow: hidden">
                    <div class="panel-body">
                        <asp:UpdatePanel ID="updatePanel_information" runat="server">
                            <Triggers>
                                <%--<asp:PostBackTrigger ControlID="rdb_Type" />--%>
                                <%--<asp:PostBackTrigger ControlID="rdb_Category" />--%>
                                <%--<asp:PostBackTrigger ControlID="ddl_DocType" />--%>
                            </Triggers>
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label60" runat="server" Text="Status" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label61" runat="server" Text="สถานะ" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:HiddenField runat="server" ID="hdf_Status"></asp:HiddenField>
                                        <asp:Label ID="lbl_Status" runat="server" Text="New Request" Enabled="false" CssClass="form-control" />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label62" runat="server" Text="Document No." CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label63" runat="server" Text="เลขที่เอกสาร" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:TextBox CssClass="form-control " Enabled="false" Text="Auto Generate" runat="server" ID="lbl_DocumentNo"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label23" runat="server" Text="Type :" CssClass="lbl_ENG"></asp:Label>
                                        <span class="red">*</span>
                                        <br />
                                        <asp:Label ID="Label24" runat="server" Text="ประเภท :" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <%--<asp:RadioButtonList ID="rdb_Type" CssClass="rbl" AutoPostBack="true" OnSelectedIndexChanged="rdb_Type_SelectedIndexChanged" runat="server"
                                            RepeatDirection="Horizontal" RepeatLayout="Table">
                                            <asp:ListItem Text="ส่งเอกสารขออนุมัติ" Value="Submit" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="จัดเก็บเอกสาร" Value="Save"></asp:ListItem>
                                        </asp:RadioButtonList>--%>
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="rdb_Type" AutoPostBack="true" OnSelectedIndexChanged="rdb_Type_SelectedIndexChanged">
                                            <asp:ListItem Text="-- Please Select --" Value="" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="ส่งเอกสารขออนุมัติ" Value="Submit"></asp:ListItem>
                                            <asp:ListItem Text="จัดเก็บเอกสาร" Value="Save"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label64" runat="server" Text="Create Date" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label65" runat="server" Text="วันที่" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:TextBox CssClass="form-control " runat="server" ID="txt_CreateDate" Enabled="false"></asp:TextBox>
                                        <asp:HiddenField runat="server" ID="hdn_ApproveDate" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label66" runat="server" Text="Category" CssClass="lbl_ENG"></asp:Label>
                                        <span class="red">*</span>
                                        <br />
                                        <asp:Label ID="Label67" runat="server" Text="หมวดหมู่" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <%--<asp:RadioButtonList ID="rdb_Category" CssClass="rbl" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rdb_Category_SelectedIndexChanged"
                                            RepeatDirection="Horizontal" RepeatLayout="Table">
                                            <asp:ListItem Text="เอกสารภายในหน่วยงาน" Value="internal" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="เอกสารส่วนกลาง" Value="centre"></asp:ListItem>
                                        </asp:RadioButtonList>--%>
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="rdb_Category" AutoPostBack="true" OnSelectedIndexChanged="rdb_Category_SelectedIndexChanged">
                                            <asp:ListItem Text="-- Please Select --" Value="" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="เอกสารภายในหน่วยงาน" Value="internal"></asp:ListItem>
                                            <asp:ListItem Text="เอกสารส่วนกลาง" Value="centre"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label85" runat="server" Text="Document Type" CssClass="lbl_ENG"></asp:Label>
                                        <span class="red">*</span>
                                        <br />
                                        <asp:Label ID="Label86" runat="server" Text="ประเภทเอกสาร" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList ID="ddl_DocType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_DocType_SelectedIndexChanged" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>

                                </div>

                                <div class="row" id="info_OtherDoctype" runat="server" visible="false">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label83" runat="server" Text="Other" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label84" runat="server" Text="เอกสารอื่น ระบุ" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:DropDownList runat="server" CssClass="form-control" ID="ddl_otherDocType" />
                                        <%--<asp:TextBox CssClass="form-control input-sm" runat="server" ID="txt_otherDocumentType"></asp:TextBox>--%>
                                    </div>
                                </div>
                                <asp:Panel ID="other_extend" DefaultButton="btn5" runat="server" Visible="false">
                                    <div class="row">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label87" runat="server" Text="Other" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label88" runat="server" Text="เอกสารอื่น ระบุ" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-10">
                                            <asp:TextBox CssClass="form-control " runat="server" ID="txt_other"></asp:TextBox>
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="info_extend" DefaultButton="btn5" runat="server" Visible="false">
                                    <%-- TO --%>
                                    <div class="row" runat="server" id="panel_To">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label22" runat="server" Text="To" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label30" runat="server" Text="ถึง" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-10">
                                            <asp:TextBox CssClass="form-control " runat="server" ID="txt_to"></asp:TextBox>
                                        </div>
                                    </div>
                                    <%-- CC --%>
                                    <div class="row" runat="server" id="panel_Cc">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label19" runat="server" Text="CC" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label20" runat="server" Text="สำเนาถึง" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-10">
                                            <asp:TextBox CssClass="form-control " runat="server" ID="txt_CC"></asp:TextBox>
                                        </div>
                                    </div>
                                    <%-- Attachment --%>
                                    <asp:Panel runat="server" DefaultButton="btn5" CssClass="row" ID="panel_Attachment" Visible="False">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label79" runat="server" Text="Attachment" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label80" runat="server" Text="สิ่งที่แนบมาด้วย" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-10">
                                            <asp:TextBox CssClass="form-control " TextMode="MultiLine" Rows="2" runat="server" ID="txt_Attachment"></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                    <%-- Receieve Document No --%>
                                    <asp:Panel CssClass="row" DefaultButton="btn5" runat="server" ID="panel_RecieveDocNo" Visible="False">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label92" runat="server" Text="Receieve Document NO" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label93" runat="server" Text="เลขที่หนังสือรับจากภายนอก" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-10">
                                            <asp:TextBox CssClass="form-control" AutoPostBack="true" runat="server" OnTextChanged="txt_RecieveDocNo_TextChanged" ID="txt_RecieveDocNo"></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                    <%-- Recieve Date --%>
                                    <asp:Panel CssClass="row" DefaultButton="btn5" runat="server" ID="panel_RecieveDate" Visible="False">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label96" runat="server" Text="Document Date" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label97" runat="server" Text="วันที่ในเอกสาร" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-4">
                                            <div class="inner-addon right-addon date" data-provide="datepicker">
                                                <i class="glyphicon glyphicon-calendar"></i>
                                                <asp:TextBox runat="server" ID="txt_DocumentDate" AutoPostBack="true" CssClass="form-control datepicker"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label94" runat="server" Text="Document Recieve" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label95" runat="server" Text="วันที่รับเอกสาร" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-4">
                                            <div class="inner-addon right-addon date" data-provide="datepicker">
                                                <i class="glyphicon glyphicon-calendar"></i>
                                                <asp:TextBox runat="server" ID="txt_DocumentRecieve" AutoPostBack="true" CssClass="form-control datepicker"></asp:TextBox>

                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <%-- Source --%>
                                    <asp:Panel CssClass="row" DefaultButton="btn5" runat="server" ID="panel_Source" Visible="False">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label98" runat="server" Text="Source" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label99" runat="server" Text="แหล่งที่มาของเอกสาร" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-10">
                                            <asp:TextBox CssClass="form-control " runat="server" ID="txt_Source"></asp:TextBox>
                                        </div>
                                    </asp:Panel>
                                </asp:Panel>
                                <asp:Panel ID="memo_extend" DefaultButton="btn5" runat="server" Visible="False">
                                    <div class="row">
                                        <div class="col-sm-2">
                                            <asp:Label ID="Label100" runat="server" Text="Send To" CssClass="lbl_ENG"></asp:Label>
                                            <br />
                                            <asp:Label ID="Label101" runat="server" Text="ผู้ที่ต้องการส่งถึง" CssClass="lbl_THI"></asp:Label>
                                        </div>
                                        <div class="col-sm-10">
                                            <div class="input-group stylish-input-group">
                                                <asp:TextBox CssClass="form-control" runat="server" ID="txt_SendTo"></asp:TextBox>

                                                <asp:Label runat="server" Visible="false" Enabled="false" ID="lbl_sendToID" />
                                                <span class="input-group-addon">
                                                    <asp:LinkButton type="submit" ID="btn_searchSendTo" OnClick="OpenPopup" runat="server">
                                                            <span class="glyphicon glyphicon-search"></span>
                                                    </asp:LinkButton>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label68" runat="server" Text="Title" CssClass="lbl_ENG"></asp:Label>
                                        <span class="red">*</span>
                                        <br />
                                        <asp:Label ID="Label69" runat="server" Text="ชื่อเรื่อง" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox CssClass="form-control " TextMode="MultiLine" runat="server" ID="txt_title"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label3" runat="server" Text="Sub Title" CssClass="lbl_ENG"></asp:Label>

                                        <br />
                                        <asp:Label ID="Label4" runat="server" Text="ชื่อเรื่อง(เพิ่มเติม)" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox CssClass="form-control " MaxLength="500" TextMode="MultiLine" runat="server" ID="txt_subtitle"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label7" runat="server" Text="Description" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label15" runat="server" Text="รายละเอียดของเอกสาร" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox CssClass="form-control" TextMode="MultiLine" runat="server" ID="txt_DocDescription"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label70" runat="server" Text="From Department" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label71" runat="server" Text="หน่วยงานตั้งต้น" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox Enabled="false" CssClass="form-control " runat="server" ID="txt_FromDepartment"></asp:TextBox>
                                        <asp:Label runat="server" Visible="false" Enabled="false" ID="hdn_FromDepartmentID" />
                                    </div>
                                </div>
                                <div class="row" runat="server" id="div_ToDepartment">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label72" runat="server" Text="To Department" CssClass="lbl_ENG"></asp:Label>
                                        <span class="red">*</span>
                                        <br />
                                        <asp:Label ID="Label73" runat="server" Text="หน่วยงานรับเรื่อง" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <div class="input-group stylish-input-group">
                                            <asp:TextBox CssClass="form-control " TextMode="MultiLine" runat="server" ID="txt_ToDepartment" Enabled="false"></asp:TextBox>

                                            <asp:Label runat="server" Visible="false" Enabled="false" ID="hdn_ToDepartment" />
                                            <span class="input-group-addon">
                                                <asp:LinkButton type="submit" ID="btn_searchDepartment" OnClick="open_ToDepartmentPopup" runat="server">
                                                            <span class="glyphicon glyphicon-search"></span>
                                                </asp:LinkButton>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label74" runat="server" Text="Priority" CssClass="lbl_ENG"></asp:Label>

                                        <br />
                                        <asp:Label ID="Label75" runat="server" Text="ความเร่งด่วนของงาน" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList ID="ddl_Priority" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="ทั่วไป" Value="Normal"></asp:ListItem>
                                            <asp:ListItem Text="ด่วน" Value="Fast"></asp:ListItem>
                                            <asp:ListItem Text="ด่วนมาก" Value="Faster"></asp:ListItem>
                                            <asp:ListItem Text="ด่วนที่สุด" Value="Fastest"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label76" runat="server" Text="Deadline" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label77" runat="server" Text="วันที่ครบกำหนด" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_deadline" AutoPostBack="true" CssClass="form-control datepicker"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2"></div>
                                    <div class="col-sm-4">
                                        <asp:CheckBox ID="chk_InternalStamp" runat="server" Text="&nbsp; Stamp Internal Only<br/>&nbsp; เอกสารเฉพาะภายในหน่วยงาน" />
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:CheckBox Visible="false" ID="chk_DOA" AutoPostBack="true" OnCheckedChanged="chk_DOA_CheckedChanged" runat="server" Text="&nbsp; Design of Authority<br/>&nbsp; เอกสารเกี่ยวกับยอดเงิน" />
                                        <br />
                                        <asp:CheckBox ID="chk_AutoStamp" runat="server" Visible="False" Text="&nbsp; Auto Stamp Signature Electronic<br/>&nbsp; ให้ระบบทำการสแตมป์ลายเซ็นต์อิเลคทรอนิค" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label81" runat="server" Text="Reference" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label82" runat="server" Text="เอกสารอ้างถึง" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btn_addReferenceDocument" runat="server" OnClick="btn_addReferenceDocument_Click" Text="Add" CssClass="CssButton btn btn-browse" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2"></div>
                                    <div class="col-sm-10">
                                        <div class="table-responsive">
                                            <asp:GridView ID="gv_ReferenceDocument" AutoGenerateColumns="false" PageSize="10" runat="server"
                                                RowStyle-Wrap="false" ShowHeaderWhenEmpty="true" AutoGenerateDeleteButton="false" EmptyDataRowStyle-HorizontalAlign="Center"
                                                OnRowDataBound="gv_ReferenceDocument_RowDataBound" OnRowCommand="gv_ReferenceDocument_RowCommand"
                                                CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                                <Columns>
                                                    <asp:BoundField HeaderText="Sequence" DataField="Sequence" HeaderStyle-Width="30px" ItemStyle-HorizontalAlign="Center" />
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                        <HeaderTemplate>
                                                            <span>Document No.</span>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblReferenceDocumentNo" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField HeaderText="Title" DataField="Title" HeaderStyle-Width="30px" ItemStyle-HorizontalAlign="Left" />
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                        <HeaderTemplate>
                                                            <span>Category</span>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblReferenceCategory" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                        <HeaderTemplate>
                                                            <span>Document Type</span>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblReferenceDocumentType" runat="server"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderTemplate>
                                                            <span>Delete</span>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Button CssClass="Img_Delete" runat="server" ID="btn_DeleteRow" OnClientClick="return confirm('Are you sure you want to delete this item?');" CommandName="DeleteItem" CommandArgument='<%# Eval("Sequence") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No Data.
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </asp:Panel>



        <!-- Permission Panel-->
        <asp:Panel CssClass="panel panel-default" DefaultButton="btn5" ID="panel_Permission" runat="server">
            <asp:UpdatePanel ID="updatePanel_Permission" runat="server">
                <ContentTemplate>
                    <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapsePermission">
                        <h4 class="panel-title">
                            <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapse2">
                                <asp:Label ID="Label29" runat="server" Text="Permission" CssClass="TableStyleENG_Head"></asp:Label>
                                <br />
                                <asp:Label ID="Label78" runat="server" Text="ผู้มีสิทธิ์เห็น" CssClass="TableStyleTH_Head"></asp:Label>
                            </a>
                        </h4>
                    </div>

                    <div id="collapsePermission" class="panel-collapse collapse in" style="overflow: hidden">
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-sm-2">
                                    <asp:Label ID="Label5" runat="server" Text="Permission" CssClass="lbl_ENG"></asp:Label>
                                    <span class="red">*</span>
                                    <br />
                                    <asp:Label ID="Label26" runat="server" Text="สิทธิ์ในการเข้าถึง" CssClass="lbl_THI"></asp:Label>
                                </div>
                                <div class="col-sm-4">
                                    <asp:DropDownList ID="ddl_Permission" OnSelectedIndexChanged="ddl_Permission_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="เปิดเผยได้" Value="Public"></asp:ListItem>
                                        <asp:ListItem Text="ความลับ" Value="Secret"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div id="div_permission" runat="server" visible="false">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Button ID="btn_addPermission" runat="server" Text="Add Permission" OnClick="OpenPopup" CssClass="CssButton" />
                                    </div>
                                    <div class="col-sm-10">
                                    </div>
                                </div>
                                <div class="table-responsive">
                                    <asp:GridView ID="gv_Permission" AutoGenerateColumns="false" PageSize="10" runat="server"
                                        RowStyle-Wrap="false" ShowHeaderWhenEmpty="true" AutoGenerateDeleteButton="false" EmptyDataRowStyle-HorizontalAlign="Center"
                                        OnRowDataBound="gv_Permission_RowDataBound" OnRowCommand="gv_Permission_RowCommand"
                                        CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Sequence<br />
                                                            ลำดับ</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Text='<%# Eval("Sequence") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Name<br />
                                                            ชื่อ</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPermissionName" runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Department<span class="red">*</span><br />
                                                            หน่วยงานหลัก</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Panel ID="PnlPermissionDepartment" runat="server">
                                                        <asp:Label ID="lblPermissionSequence" Visible="false" runat="server" Text='<%# Eval("Sequence") %>'></asp:Label>
                                                        <asp:DropDownList ID="ddl_PermissionDepartment" runat="server" OnSelectedIndexChanged="ddl_PermissionDepartment_SelectedIndexChanged" Width="100%" AutoPostBack="true" CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Sub Department<span class="red">*</span><br />
                                                            หน่วยงานย่อย</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Panel ID="PnlPermissionSubDepartment" runat="server">
                                                        <asp:DropDownList ID="ddl_PermissionSubDepartment" runat="server" OnSelectedIndexChanged="ddl_PermissionSubDepartment_SelectedIndexChanged" Width="100%" AutoPostBack="true" CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Position<span class="red">*</span><br />
                                                            ตำแหน่งงาน</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Panel ID="PnlPermissionPosition" runat="server">
                                                        <asp:DropDownList ID="ddl_PermissionPosition" runat="server" OnSelectedIndexChanged="ddl_PermissionPosition_SelectedIndexChanged" Width="100%" AutoPostBack="true" CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </asp:Panel>
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
                                                    <asp:Button CssClass="Img_Delete" runat="server" ID="btn_DeleteRow" OnClientClick="return confirm('Are you sure you want to delete this item?');" CommandName="DeleteItem" CommandArgument='<%# Eval("Sequence") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            No Data.
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </div>
                            </div>
                            <div id="div_permissionPublic" runat="server">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Button ID="btn_addGroupEmail" runat="server" Text="Add GroupMail" OnClick="btn_searchDepartment_Click" CommandName="GroupMail" CssClass="CssButton" />
                                    </div>
                                    <div class="col-sm-10">
                                    </div>
                                </div>
                                <div class="table-responsive">
                                    <asp:GridView ID="gv_GroupEmail" AutoGenerateColumns="false" PageSize="10" runat="server"
                                        RowStyle-Wrap="false" ShowHeaderWhenEmpty="true" AutoGenerateDeleteButton="false" EmptyDataRowStyle-HorizontalAlign="Center"
                                        OnRowCommand="gv_GroupEmail_OnRowCommand"
                                        CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                        <Columns>
                                            <%--<asp:BoundField HeaderText="Sequence" DataField="Sequence" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />--%>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Sequence<br />
                                                            ลำดับ</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Text='<%# Eval("Sequence") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Department<br />
                                                            หน่วยงานหลัก</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_DepartmentName" runat="server" Text='<%# Eval("DepartmentName") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Group Email<br />
                                                            อีเมล์กลุ่ม</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_GroupMail" runat="server" Text='<%# Eval("DepartmentGroupMail") %>'></asp:Label>
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
                                                    <asp:Button CssClass="Img_Delete" runat="server" ID="btn_DeleteRow" OnClientClick="return confirm('Are you sure you want to delete this item?');" CommandName="DeleteItem" CommandArgument='<%# Eval("Sequence") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            No Data.
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </div>
                            </div>

                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        <!-- Line Approval Panel-->
        <asp:UpdatePanel ID="updatePanel_LineApproval" runat="server">
            <ContentTemplate>
                <asp:Panel ID="panel_Approval" runat="server" CssClass="panel panel-default">
                    <asp:Panel runat="server" ID="panel_LineApproval">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapse2">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapse2">
                                    <asp:Label ID="Label8" runat="server" Text="Line Approval" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label9" runat="server" Text="สายการอนุมัติ" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <div id="collapse2" class="panel-collapse collapse in" style="overflow: hidden">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Button ID="Btn_Add_Spec_Approver" runat="server" Text="Add Approver" OnClick="OpenPopup" CssClass="CssButton" />
                                    </div>
                                    <div class="col-sm-10">
                                    </div>
                                </div>
                                <div class="table-responsive">
                                    <asp:GridView ID="gvApprovelList" AutoGenerateColumns="false" PageSize="10" runat="server"
                                        RowStyle-Wrap="false" ShowHeaderWhenEmpty="true" AutoGenerateDeleteButton="false" EmptyDataRowStyle-HorizontalAlign="Center"
                                        OnRowDataBound="gvApprovelList_RowDataBound" OnRowCommand="gvApprovelList_RowCommand"
                                        CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Sequence<br />
                                                            ลำดับ</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Text='<%# Eval("Sequence") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Up/Down" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Up/Down<br />
                                                            ปรับขึ้น/ลง</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Button ID="btnUp" runat="server" Text="" CssClass="Img_Up" CommandName="ClickUP" CommandArgument='<%# Eval("Sequence") %>' />
                                                    <asp:Button ID="btnDown" runat="server" Text="" CssClass="Img_Down" CommandName="ClickDown" CommandArgument='<%# Eval("Sequence") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Approver Name<br />
                                                            ชื่อผู้อนุมัติ</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblApproverName" runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Department<span class="red">*</span><br />
                                                            หน่วยงานหลัก</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Panel ID="PnlApprovorDepartment" runat="server">
                                                        <asp:Label ID="lblApprovorSequence" Visible="false" runat="server" Text='<%# Eval("Sequence") %>'></asp:Label>
                                                        <asp:DropDownList ID="ddl_ApprovorDepartment" runat="server" OnSelectedIndexChanged="ddl_ApprovorDepartment_SelectedIndexChanged" Width="100%" AutoPostBack="true" CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Sub Department<span class="red">*</span><br />
                                                            หน่วยงานย่อย</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Panel ID="PnlApprovorSubDepartment" runat="server">
                                                        <asp:DropDownList ID="ddl_ApprovorSubDepartment" runat="server" OnSelectedIndexChanged="ddl_ApprovorSubDepartment_SelectedIndexChanged" Width="100%" AutoPostBack="true" CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 5px 0 5px 0;">
                                                        <span>Position<span class="red">*</span><br />
                                                            ตำแหน่งงาน</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Panel ID="PnlApprovorPosition" runat="server">
                                                        <asp:DropDownList ID="ddl_ApprovorPosition" runat="server" OnSelectedIndexChanged="ddl_ApprovorPosition_SelectedIndexChanged" Width="100%" AutoPostBack="true" CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </asp:Panel>
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
                                                    <asp:Button CssClass="Img_Delete" runat="server" ID="btn_DeleteRow" OnClientClick="return confirm('Are you sure you want to delete this item?');" CommandName="DeleteItem" CommandArgument='<%# Eval("Sequence") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            No Data.
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <!-- Content / Cost Center / DOA Amount -->
        <asp:UpdatePanel runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnPrint" />
                <asp:PostBackTrigger ControlID="chk_isAttachWord" />
            </Triggers>
            <ContentTemplate>
                <asp:Panel ID="PanelInfoExtend" runat="server" CssClass="panel panel-default">
                    <div id="infoExtend" class="panel-collapse collapse in" style="overflow: hidden">
                        <div class="panel-body" runat="server" id="divContent">
                            <div class="row">
                                <div class="col-xs-6 col-sm-6 col-lg-4">
                                    <asp:CheckBox Text="Edit Content in MSWord" Visible="false" Enabled="false" Checked runat="server" ID="chk_isAttachWord" AutoPostBack="true" OnCheckedChanged="chk_isAttachWord_CheckedChanged" CssClass="rbl" />
                                </div>

                                <div class="col-xs-4 col-xs-offset-2 col-sm-4  col-sm-offset-2 col-lg-2 col-lg-push-2">
                                    <asp:Button Text="Download Template" CssClass="CssButton" runat="server" ID="btn_DownloadTemplate" CommandName="Download" OnClick="btn_DownloadTemplate_Click" OnClientClick="window.setTimeout(function() { _spFormOnSubmitCalled = false; }, 10);" />
                                </div>
                                <div class="col-xs-4 col-sm-4 col-lg-2 col-lg-push-2">
                                    <asp:Button ID="btnPrint" Text="Preview" CommandName="Print" OnClick="btnPrint_Click" OnClientClick="window.setTimeout(function() { _spFormOnSubmitCalled = false; }, 10);" CssClass="CssButton" runat="server" />
                                </div>
                            </div>

                            <asp:Panel ID="tblAmount" runat="server" Visible="false" CssClass="row">
                                <div class="col-sm-2">
                                    <asp:Label ID="Label6" runat="server" Text="Cost Center :" CssClass="lbl_ENG"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label16" Text="รหัสต้นทุน :" runat="server" CssClass="lbl_THI" />
                                </div>
                                <div class="col-sm-2" style="margin-top: 8px;">
                                    <asp:Label ID="lbl_CostCenter" runat="server" CssClass="form-control " />
                                </div>
                                <div class="col-sm-2"></div>
                                <div class="col-sm-2">
                                    <asp:Label ID="Label25" runat="server" Text="Amount :" CssClass="lbl_ENG"></asp:Label>
                                    <span class="red">*</span>
                                    <br />
                                    <asp:Label ID="txt_Total" Text="จำนวนเงินสุทธิ :<br/>(รวม VAT)" runat="server" CssClass="lbl_THI" />
                                </div>
                                <div class="col-sm-2" style="margin-top: 8px;">
                                    <asp:TextBox ID="txt_Amount" AutoPostBack="true" OnTextChanged="txt_Amount_TextChanged" MaxLength="20" runat="server" CssClass="form-control " />
                                </div>
                                <div class="col-sm-2">
                                    <asp:Label ID="Label27" runat="server" Text="Baht" CssClass="lbl_ENG"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label28" Text="บาท" runat="server" CssClass="lbl_THI" />
                                </div>
                            </asp:Panel>


                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:UpdatePanel ID="updatePanel_Extended" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="btn_AttachUpload" />
                <asp:PostBackTrigger ControlID="btn_DocumentFileUpload" />
            </Triggers>
            <ContentTemplate>
                <!-- Attach File Panel-->
                <div class="panel panel-default">
                    <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseAttach">
                        <h4 class="panel-title">
                            <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapse3">
                                <asp:Label ID="Label10" runat="server" Text="Attach Files" CssClass="TableStyleENG_Head"></asp:Label>
                                <br />
                                <asp:Label ID="Label11" runat="server" Text="ไฟล์แนบ" CssClass="TableStyleTH_Head"></asp:Label>
                            </a>
                        </h4>
                    </div>
                    <asp:Panel ID="collapseAttach" runat="server" CssClass="panel-collapse collapse in" Style="overflow: hidden">
                        <div class="panel-body">
                            <asp:Panel CssClass="row" ID="panel_attachDoc" Visible="False" runat="server">
                                <div class="col-sm-4">
                                    <asp:Label ID="Label12" runat="server" Text="Document File" CssClass="lbl_ENG"></asp:Label>
                                    <span class="red">*เฉพาะไฟล์ PDF เท่านั้น*</span>
                                    <br />
                                    <asp:Label ID="Label13" runat="server" Text="เอกสาร :" CssClass="lbl_THI"></asp:Label>
                                </div>
                                <div class="col-sm-6">
                                    <asp:FileUpload runat="server" ID="doc_upload" CssClass="notifyBtn" AllowMultiple="false" />
                                    <asp:Label runat="server" ID="Label14" Visible="false"></asp:Label>
                                </div>
                                <div class="col-sm-2">
                                    <asp:Button ID="btn_DocumentFileUpload" runat="server" Text="Upload" CssClass="CssButton" CommandName="Y" OnClick="fileUploadBtn_Click" />
                                </div>
                            </asp:Panel>
                            <div class="row">
                                <div class="col-sm-12">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv_AttachDocumentFile" AutoGenerateColumns="false" PageSize="10" runat="server" EmptyDataRowStyle-HorizontalAlign="Center"
                                            RowStyle-Wrap="false" ShowHeaderWhenEmpty="true" OnRowDataBound="gv_AttachDocumentFile_OnRowDataBound" OnRowCommand="gv_AttachDocumentFile_RowCommand"
                                            CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 5px 0 5px 0;">
                                                            <span>Sequence<br />
                                                                ลำดับ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" Text='<%# Eval("Sequence") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 5px 0 5px 0;">
                                                            <span>File<br />
                                                                เอกสารแนบ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:HyperLink runat="server" ID="hpl_AttachDocFile" Text='<%# Eval("AttachFile") %>' NavigateUrl='<%# Eval("AttachFilePath") %>' Target="_blank"></asp:HyperLink>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 5px 0 5px 0;">
                                                            <span>ActorName<br />
                                                                ผู้อัพโหลดเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" Text="-" ID="lblEmpName"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 5px 0 5px 0;">
                                                            <span>Modified Date<br />
                                                                วันที่แนบเอกสาร</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" Text='<%# Eval("AttachDate") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <span>Delete</span>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Button CssClass="Img_Delete" runat="server" ID="btn_DeleteRowAttach" OnClientClick="return confirm('Are you sure you want to delete this item?');" CommandName="DeleteItem" CommandArgument='<%# Eval("Sequence") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                No Attach File.
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <div runat="server" id="panel_attachFile">
                                <div class="row">
                                    <div class="col-sm-4">
                                        <asp:Label ID="Label17" runat="server" Text="Attach File :" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label18" runat="server" Text="ไฟล์แนบ :" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-6">
                                        <asp:FileUpload runat="server" ID="file_upload" AllowMultiple="false" />
                                        <asp:Label runat="server" ID="lbl_docSet" Visible="false"></asp:Label>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btn_AttachUpload" runat="server" Text="Upload" CssClass="CssButton" CommandName="N" OnClick="fileUploadBtn_Click" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <div class="table-responsive">
                                            <asp:GridView ID="gv_AttachFile" AutoGenerateColumns="false" PageSize="10" runat="server" EmptyDataRowStyle-HorizontalAlign="Center"
                                                RowStyle-Wrap="false" ShowHeaderWhenEmpty="true" OnRowDataBound="gv_AttachFile_OnRowDataBound" OnRowCommand="gv_AttachFile_RowCommand"
                                                CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderTemplate>
                                                            <div style="text-align: center; padding: 5px 0 5px 0;">
                                                                <span>Sequence<br />
                                                                    ลำดับ</span>
                                                            </div>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" Text='<%# Eval("Sequence") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="300px" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderTemplate>
                                                            <div style="text-align: center; padding: 5px 0 5px 0;">
                                                                <span>File<br />
                                                                    เอกสารแนบ</span>
                                                            </div>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:HyperLink runat="server" Text='<%# Eval("AttachFile") %>' NavigateUrl='<%# Eval("AttachFilePath") %>' Target="_blank"></asp:HyperLink>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderTemplate>
                                                            <div style="text-align: center; padding: 5px 0 5px 0;">
                                                                <span>ActorName<br />
                                                                    ผู้อัพโหลดเอกสาร</span>
                                                            </div>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" Text="-" ID="lblEmpName"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderTemplate>
                                                            <div style="text-align: center; padding: 5px 0 5px 0;">
                                                                <span>Modified Date<br />
                                                                    วันที่แนบเอกสาร</span>
                                                            </div>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" Text='<%# Eval("AttachDate") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderTemplate>
                                                            <span>Delete</span>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Button CssClass="Img_Delete" runat="server" ID="btn_DeleteRowAttach" OnClientClick="return confirm('Are you sure you want to delete this item?');" CommandName="DeleteItem" CommandArgument='<%# Eval("Sequence") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    No Attach File.
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>

                <!-- Additional Commend Panel-->
                <asp:Panel DefaultButton="btn5" ID="Other_Div" runat="server" CssClass="Panel-group">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#additionalComment">
                            <h4 class="panel-title">
                                <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapse1">
                                    <asp:Label ID="Label31" runat="server" Text="Additional Comment" CssClass="TableStyleENG_Head"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label21" runat="server" Text="แสดงความคิดเห็นเพิ่ม" CssClass="TableStyleTH_Head"></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <div id="additionalComment" class="panel-collapse collapse in" style="overflow: hidden">
                            <div class="panel-body">
                                <asp:TextBox ID="txt_AdditionalComment" runat="server" CssClass="TextBoxStyle" TextMode="MultiLine" Rows="5" Width="100%" Height="100px" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>


                <%--Button_Div--%>
                <div id="DivButtonForViewer" visible="false" runat="server" class="row">
                    <div class="col-sm-offset-10 col-sm-2">
                        <asp:Button ID="btnClose2" Text="Close" OnClick="btnClose_Click" CssClass="CssButton custom-btn-Default" runat="server" />
                    </div>
                </div>
                <div id="DivButtonForSave" runat="server" class="row" visible="False">
                    <div class="col-sm-offset-6 col-sm-2">
                        <asp:Button ID="btnCancel1" OnClick="btn_Click" Text="Cancel" CssClass="CssButton" Visible="false" runat="server" OnClientClick="OnPreventDoubleClick({0}, 'Working on it...');return confirm('Are you sure you want to Cancel?');" />
                    </div>
                    <div class=" col-sm-2">
                        <asp:Button ID="btnSubmit1" OnClick="btn_Click" Text="Submit" CssClass="CssButton" runat="server" OnClientClick="OnPreventDoubleClick({0}, 'Working on it...');" />
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btnClose1" Text="Close" OnClick="btnClose_Click" CssClass="CssButton custom-btn-Default" runat="server" />
                    </div>
                </div>
                <div id="DivButtonForSubmit" runat="server" class="row" visible="True">
                    <div class="col-sm-offset-2 col-sm-2">
                        <asp:Button ID="btn2" OnClick="btnRequestComment_Click" Text="" Visible="false" CssClass="CssButton" runat="server" />
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btn3" OnClick="btn_Click" Text="" Visible="false" CssClass="CssButton" runat="server" OnClientClick="return confirm('Are you sure you want to Cancel?');" />
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btn4" Text="Save Draft" OnClick="btn_Click" CssClass="CssButton custom-btn-Info" runat="server" />
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btn5" Text="Submit" OnClick="btn_Click" CssClass="CssButton custom-btn-Success" runat="server" />
                    </div>
                    <div class="col-sm-2">
                        <asp:Button ID="btnClose" Text="Close" OnClick="btnClose_Click" CssClass="CssButton custom-btn-Default" runat="server" />
                    </div>
                </div>


                <!--Action History-->
                <div id="divHistory" class="panel panel-default" runat="server" visible="False">
                    <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapseActionHistory">
                        <h4 class="panel-title">
                            <a class="accordion-toggle" data-toggle="collapse" data-parent="#accordion" href="#collapse3">
                                <asp:Label ID="Label90" runat="server" Text="Action History" CssClass="TableStyleENG_Head"></asp:Label>
                                <br />
                                <asp:Label ID="Label91" runat="server" Text="ประวัติ" CssClass="TableStyleTH_Head"></asp:Label>
                            </a>
                        </h4>
                    </div>
                    <div id="collapseActionHistory" class="panel-collapse collapse in" style="overflow: hidden">
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-sm-12">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gv_ActionHistory" AutoGenerateColumns="false" runat="server" EmptyDataRowStyle-HorizontalAlign="Center"
                                            RowStyle-Wrap="true" ShowHeaderWhenEmpty="true" OnRowDataBound="gv_ActionHistory_RowDataBound"
                                            CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-sm-12">
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Action Date<br>
                                                                วันที่ดำเนินการ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lbl_ActionDate"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="250px" ItemStyle-HorizontalAlign="Left">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Action By<br>
                                                                ผู้ดำเนินการ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div style="padding: 0 10px 0 10px;">
                                                            <asp:Label runat="server" ID="lbl_ActionBy"></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Left">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Status<br>
                                                                สถานะ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div style="padding: 0 10px 0 10px;">
                                                            <asp:Label runat="server" ID="lbl_HistoryStatus" Text='<%# Eval("StatusBefore") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="150px" ItemStyle-HorizontalAlign="Left">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Action<br>
                                                                ดำเนินการโดย</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div style="padding: 0 10px 0 10px;">
                                                            <asp:Label runat="server" ID="lbl_HistoryAction" Text='<%# Eval("ActionName") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Left">
                                                    <HeaderTemplate>
                                                        <div style="text-align: center; padding: 10px 0 10px 0;">
                                                            <span>Additional Comment<br>
                                                                หมายเหตุ</span>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div style="padding: 0 10px 0 10px;">
                                                            <asp:Label runat="server" ID="lbl_HistoryComment" Text='<%# Eval("Comment") %>'></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                No Attach File.
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <br />
            </ContentTemplate>
        </asp:UpdatePanel>

    </div>
</asp:Panel>
<asp:Label ID="lblRole" ForeColor="White" runat="server"></asp:Label>
<script type="text/javascript" lang="javascript">
    //Disable postBackElement until postback done
    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
    function BeginRequestHandler(sender, args) { var oControl = args.get_postBackElement(); oControl.disabled = true; }

</script>
<script>
    function OnPreventDoubleClick(elm, disableText) {
        //Disable all CssButton when do some action
        var listActionBtn = document.getElementsByClassName('CssButton');
        if (listActionBtn) {
            for (var i = 0; i < listActionBtn.length; i++) {
                listActionBtn[i].disabled = true;
            }
        }
        elm.disabled = false;
        elm.value = disableText;
    }


    //will delete later
    function DisableButton() {
        var listButton = document.getElementsByClassName("CssButton");
        for (var i = 0; i < listButton.item.length; i++) {
            listButton.item[i].DisableButton = 'true';
        }
    }
    function EnableButton() {
        var listButton = document.getElementsByClassName("CssButton");
        for (var i = 0; i < listButton.item.length; i++) {
            listButton.item[i].DisableButton = 'false';
        }
    }
    function DisableEnter() {
        $('html').bind('keypress', function (e) {
            if (e.keyCode == 13) {
                return false;
            }
        });
    }


</script>
