<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorklistUserControl.ascx.cs" Inherits="PIMEdoc_CR.Default.e_Document.Worklist.WorklistUserControl" %>
<%@ Register TagPrefix="uc1" TagName="UpdateProgress" Src="../../PIMEdoc/UpdateProgress.ascx" %>
<link rel="stylesheet" href="../CSS/bootstrap.min.css" />
<link rel="stylesheet" href="../CSS/jquery-ui.css" />
<link rel="stylesheet" href="../CSS/Styles.css" />

<!-- Do not Deploy To PRD -->
<%--<script src="../JS/jquery-1.10.2.js"></script>
<script src="../JS/bootstrap.js"></script>--%>
<%--<script type="text/javascript" src="../JS/jquery.min.js"></script>--%>
<!-- Do not Deploy To PRD -->

<script type="text/javascript" src="../JS/jquery-ui.js"></script>



<style>
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

    .btn.action {
        background-color: white;
        border-radius: 2px;
        border: 1px solid #01883c;
        color: #01883c;
    }

        .btn.action.active {
            background-color: #01883c;
            color: white;
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
        $('.datepicker').datepicker({
            dateFormat: 'dd/mm/yy'
        });

        <%--$('#<%=ddlFromDepartment.ClientID %>').combobox();
        $('#<%=ddlToDepartment.ClientID %>').combobox();--%>
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
                                            AutoGenerateColumns="false" RowStyle-Wrap="false" EmptyDataRowStyle-HorizontalAlign="Center"
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
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <Triggers>
        <%--<asp:PostBackTrigger ControlID="ddl_TaskGroup" />--%>
    </Triggers>
    <ContentTemplate>
        <div class="container" style="max-width: 1200px; width: 100%;">
            <div id="divBody" runat="server">
                <!-- Work List Header -->
                <div style="width: 100%; margin: 30px auto 0 auto; height: auto !important">
                    <asp:Label Text="Work List" runat="server" Style="font-size: 35px !important;" CssClass="lbl_Title"></asp:Label>
                    <div style="border-top: 1px solid #FBB300; margin-bottom: 10px;">
                    </div>
                </div>
                <!-- WorkList -->
                <div runat="server" class="panel-group" id="lineApproval">
                    <div class="panel panel-default">
                        <div class="panel-heading panel-heading-custom" data-toggle="collapse" data-parent="#accordion" data-target="#collapse1">
                            <h4 class="panel-title">
                                <a class="accordion-toggle TableStyleENG_Head" data-toggle="collapse" data-parent="#accordion" href="#collapse1">
                                    <asp:Label ID="Label1" runat="server" Text="To Do List" CssClass=""></asp:Label>
                                    <br />
                                    <asp:Label ID="Label2" runat="server" Text="รายการงาน" CssClass=""></asp:Label>
                                </a>
                            </h4>
                        </div>
                        <div id="collapse1" class="panel-collapse collapse in" style="overflow: hidden">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label5" runat="server" Text="Search :" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label6" runat="server" Text="ค้นหา :" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-10">
                                        <asp:TextBox runat="server" ID="txt_Search" placeholder="search" CssClass="form-control input-md" />
                                    </div>

                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label3" runat="server" Text="Document Type :" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label4" runat="server" Text="ประเภทเอกสาร :" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList ID="ddl_DocType" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label9" runat="server" Text="Task Group :" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label10" runat="server" Text="ประเภทกลุ่มงาน :" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList ID="ddl_TaskGroup" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_TaskGroup_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Text="To Do List | งานของเรา" Value="To Do List"></asp:ListItem>
                                            <asp:ListItem Text="In Process | รอดำเนินการ" Value="In Process"></asp:ListItem>
                                            <asp:ListItem Text="Completed | งานที่จบกระบวนการ" Value="Completed"></asp:ListItem>
                                            <asp:ListItem Text="Cancelled | งานที่ยกเลิก" Value="Cancelled"></asp:ListItem>
                                            <asp:ListItem Text="Rejected | งานที่ไม่ผ่านการอนุมัติ" Value="Rejected"></asp:ListItem>
                                            <asp:ListItem Text="History | งานที่เกี่ยวข้อง" Value="History"></asp:ListItem>
                                            <asp:ListItem Text="My Assigned | งานที่ได้รับมอบหมาย" Value="Assign"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label13" runat="server" Text="Category :" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label14" runat="server" Text="หมวดหมู่ :" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList ID="ddl_Categoty" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="ทั้งหมด" Value=""></asp:ListItem>
                                            <asp:ListItem Text="ส่วนกลาง" Value="centre"></asp:ListItem>
                                            <asp:ListItem Text="ภายในหน่วยงาน" Value="internal"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label15" runat="server" Text="Status :" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label16" runat="server" Text="สถานะ :" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList ID="ddl_Status" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="ทั้งหมด" Value=""></asp:ListItem>
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
                                </div>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label11" runat="server" Text="Request Date From:" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label12" runat="server" Text="วันที่ :" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_RequestDateFrom" OnTextChanged="OnDateTimeChanged" AutoPostBack="true" CssClass="form-control datepicker" />
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label ID="Label7" runat="server" Text="Request Date To :" CssClass="lbl_ENG"></asp:Label>
                                        <br />
                                        <asp:Label ID="Label8" runat="server" Text="ถึงวันที่ :" CssClass="lbl_THI"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                            <i class="glyphicon glyphicon-calendar"></i>
                                            <asp:TextBox runat="server" ID="txt_RequestDateTo" OnTextChanged="OnDateTimeChanged" AutoPostBack="true" CssClass="form-control datepicker" />
                                        </div>
                                    </div>
                                </div>
                                <%--<div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="From Department :<br/>หน่วยงานตั้งต้น :" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList runat="server" ID="ddlFromDepartment" CssClass="form-control input-sm">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="To Departmtent :<br/>หน่วยงานรับเรื่อง :" CssClass="control-label"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:DropDownList runat="server" ID="ddlToDepartment" CssClass="form-control input-sm">
                                        </asp:DropDownList>
                                    </div>
                                </div>--%>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:Label runat="server" Text="Item Count :<br/>จำนวนงานที่พบ :" CssClass="lbl_ENG"></asp:Label>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:Label runat="server" ID="lbl_ItemCounter" CssClass="form-control input-sm">
                                        </asp:Label>
                                    </div>
                                    <div class="col-sm-2">
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" CssClass="CssButton" />
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btnReset" runat="server" OnClick="btnReset_Click" Text="Reset" CssClass="CssButton btn-danger" />
                                    </div>
                                </div>

                                <div class="table-responsive" style="padding-top: 20px;">

                                    <asp:GridView runat="server" ID="gv_WorkList" OnRowDataBound="gv_WorkList_RowDataBound" OnRowCommand="gv_WorkList_OnRowCommand" OnSelectedIndexChanged="gv_WorkList_SelectedIndexChanged"
                                        AutoGenerateColumns="false" AllowPaging="true" PageSize="10" RowStyle-Wrap="false" EmptyDataRowStyle-HorizontalAlign="Center" OnPageIndexChanging="gv_WorkList_PageIndexChanging"
                                        ShowHeaderWhenEmpty="true" CssClass="table table-responsive table-hover table-striped  table-bordered table-condensed header-center ">
                                        <Columns>

                                            <asp:TemplateField HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                        <asp:CheckBox ID="chkHeadWorkList" runat="server" OnCheckedChanged="chkHeadWorkList_OnCheckedChanged" AutoPostBack="true" />
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkWorkList" runat="server" OnCheckedChanged="chkWorkList_OnCheckedChanged" AutoPostBack="true" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="หมวดหมู่เอกสาร" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                        <span>Category<br />
                                                            หมวดหมู่เอกสาร</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Category" runat="server"></asp:Label>
                                                    <asp:Label Text="-" ID="lbl_Description" Visible="false" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderStyle-Width="150px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
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
                                                    <div style="text-align: left; white-space: normal; min-width: 300px;">
                                                        <asp:Label runat="server" ID="lbl_title" Style="word-wrap: break-word; word-break: break-all;"></asp:Label>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--<asp:BoundField HeaderStyle-CssClass="text-center" HeaderText="ชื่อเอกสาร" DataField="Title" ItemStyle-HorizontalAlign="left" />--%>
                                            <%--<asp:BoundField HeaderStyle-CssClass="text-center" HeaderText="สถานะ" DataField="Status" ItemStyle-HorizontalAlign="left" />--%>
                                            <asp:TemplateField HeaderStyle-Width="250px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                        <span>To Document<br />
                                                            หน่วยงานรับเรื่อง</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_ToDepartment" Text='<%# Eval("ToDepartmentName") %>' runat="server"></asp:Label>
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
                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                        <span>Requested By<br />
                                                            ผู้ร้องขอเอกสาร</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lbl_Requestor" runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-Width="150px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Left">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                        <span>Status<br />
                                                            สถานะ</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Text='<%# Eval("Status") %>'></asp:Label>
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
                                            <asp:TemplateField HeaderStyle-Width="70px" HeaderStyle-CssClass="text-center" ItemStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                        <span>History<br />
                                                            ประวัติ</span>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" ID="btn_History" CommandName="ShowHistory" CommandArgument='<%# Eval("DocID") %>'>
                                                            <span class="glyphicon glyphicon-search"></span>
                                                    </asp:LinkButton>
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

                                <!-- Paging -->
                                <br />
                                <div class="text-center">
                                    <asp:LinkButton ID="btn_Previous" CssClass="btn action" Style="float: left" OnCommand="btn_Previous_Command"
                                        runat="server">Previous
                                    </asp:LinkButton>
                                    <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnPage" CssClass="btn action"
                                                CommandName="Page" CommandArgument='<%# Container.DataItem.Equals(".") ? "": Container.DataItem %>'
                                                runat="server">
                                                                    <%# Container.DataItem %>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:LinkButton ID="btn_Next" CssClass="btn action" Style="float: right" OnCommand="btn_Next_Command"
                                        runat="server">Next
                                    </asp:LinkButton>
                                </div>
                                <br />
                                <div class="row" runat="server" id="div_approvement" visible="false">
                                    <div class="col-sm-10">
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btn_Approve" runat="server" OnClick="btn_Approve_Click" Text="Approve" CssClass="CssButton" CommandName="Approve" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal fade" id="searchApproverModal" role="dialog">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <div class="row">
                                            <div class="col-xs-9 col-sm-8 form-horizontal">
                                                <h4 class="modal-title">Approval History</h4>
                                            </div>
                                            <div class="col-xs-3 col-sm-4 form-horizontal " style="text-align: right">
                                                <asp:Label ID="lblPopupMode" Visible="false" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="modal-body">
                                        <div class="panel-collapse collapse in">
                                            <asp:Panel runat="server" CssClass="panel-body">
                                                <div class="row">

                                                    <div class="table-responsive">
                                                        <table class="cssTable table table-hover table-striped table-bordered table-condensed header-center col-sm-12">
                                                            <tr>
                                                                <th style="width: 120px">วันที่
                                                                </th>
                                                                <th>โดย
                                                                </th>
                                                                <th>ดำเนินการ
                                                                </th>
                                                                <th style="width: 170px">ความคิดเห็น
                                                                </th>
                                                            </tr>
                                                            <tr>
                                                                <td>15/12/2016 12:01
                                                                </td>
                                                                <td style="text-align: left">สิโรฒม์ ทัศนัยพิทักษ์กุล
                                                                </td>
                                                                <td>อนุมัติ
                                                                </td>
                                                                <td></td>
                                                            </tr>
                                                            <tr>
                                                                <td>15/12/2016 15:21
                                                                </td>
                                                                <td style="text-align: left">ธานุวัฒน์ แพลอย
                                                                </td>
                                                                <td>อนุมัติ
                                                                </td>
                                                                <td></td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
