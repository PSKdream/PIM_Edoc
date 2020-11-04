<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultUserControl.ascx.cs" Inherits="PIMEdoc_CR.Default.DefaultUserControl" %>
<%@ Register TagPrefix="uc1" TagName="UpdateProgress" Src="../../PIMEdoc/UpdateProgress.ascx" %>
<%@ Register Src="~/_controltemplates/15/PIMEdoc/UpdateProgress.ascx" TagPrefix="uc2" TagName="UpdateProgress" %>

<link rel="stylesheet" type="text/css" href="../CSS/Styles.css" />
<link rel="stylesheet" type="text/css" href="../CSS/bootstrap.min.css" />
<link rel="stylesheet" type="text/css" href="../CSS/jquery-ui.css" />

<!-- Do not Deploy To PRD -->
<%--<script type="text/javascript" src="../JS/jquery-1.10.2.js"></script>
<script src="../JS/bootstrap.js"></script>--%>
<%--<script type="text/javascript" src="../JS/jquery.min.js"></script>
<script type="text/javascript" src="../JS/bootstraps.js"></script>--%>
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

    select {
        font-size: 15px !important;
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
    }

</script>



<uc2:UpdateProgress runat="server" id="UpdateProgress" />
<!-- Authorize Modal -->
<div class="modal fade" id="modalAuthorize" runat="server" role="dialog">
    <div class="modal-dialog modal-lg">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-body" style="text-align: center">
                        <asp:Label Text="Loading your authorization..." ID="lblAuthorizeResult" runat="server" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>


<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class="container" style="max-width: 1200px; width: 100%;">
            <div id="divBody" runat="server">
                <!-- Work List Header -->
                <div style="width: 100%; margin: 30px auto 0 auto; height: auto !important">
                    <asp:Label Text="e-Document" runat="server" Style="font-size: 35px !important;" CssClass="lbl_Title"></asp:Label>
                    <div style="border-top: 1px solid #FBB300; margin-bottom: 10px;">
                    </div>
                </div>
                <!-- WorkList -->
                <div runat="server" class="panel-group" id="lineApproval">
                    <div class="panel panel-default">
                        <div id="collapse1" class="panel-collapse collapse in" style="overflow: hidden">
                            <div class="panel-body">
                                <div class="col-xs-12 col-sm-12 col-lg-3">
                                    <div class="col-xs-6 col-sm-4 col-lg-6" style="text-align: center">
                                        <asp:Button runat="server" ID="btn_Annouce" CommandName="P" CssClass="ImgAnnoun_table" OnClick="btn_Command_Click" /><br />
                                        <asp:Label runat="server" ID="txt_Annouce" Text="ประกาศ" Font-Size="10" CssClass="lbl_DetailEng"></asp:Label>
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-lg-6" style="text-align: center">
                                        <asp:Button runat="server" ID="btn_Command" CommandName="C" OnClick="btn_Command_Click" CssClass="ImgCommand_table" /><br />
                                        <asp:Label runat="server" ID="txt_Command" Text="คำสั่ง" Font-Size="10" CssClass="lbl_DetailEng"></asp:Label>
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-lg-6" style="text-align: center">
                                        <asp:Button runat="server" ID="btn_Rule" CommandName="L" OnClick="btn_Command_Click" CssClass=" ImgMom_table" /><br />
                                        <asp:Label runat="server" ID="txt_Rule" Text="ข้อบังคับ" Font-Size="10" CssClass="lbl_DetailEng"></asp:Label>
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-lg-6" style="text-align: center">
                                        <asp:Button runat="server" ID="btn_Order" CommandName="R" OnClick="btn_Command_Click" CssClass="ImgMemo_table" /><br />
                                        <asp:Label runat="server" ID="txt_Order" Text="ระเบียบ" Font-Size="10" CssClass="lbl_DetailEng"></asp:Label>
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-lg-6" style="text-align: center">
                                        <asp:Button runat="server" ID="btn_Ex" CommandName="Ex" OnClick="btn_Command_Click" CssClass="ImgOut_table" /><br />
                                        <asp:Label runat="server" ID="txt_Ex" Text="จดหมายออก" Font-Size="10" CssClass="lbl_DetailEng"></asp:Label>
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-lg-6" style="text-align: center">
                                        <asp:Button runat="server" ID="btn_ExEn" CommandName="ExEn" OnClick="btn_Command_Click" CssClass="ImgOut_table" /><br />
                                        <asp:Label runat="server" ID="txt_ExEn" Text="จดหมายออก (ภาษาอังกฤษ)" Font-Size="10" CssClass="lbl_DetailEng"></asp:Label>
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-lg-6" style="text-align: center">
                                        <asp:Button runat="server" ID="btn_Recieve" CommandName="Im" OnClick="btn_Command_Click" CssClass="ImgIn_table" /><br />
                                        <asp:Label runat="server" ID="txt_Recieve" Text="หนังสือเข้า" Font-Size="10" CssClass="lbl_DetailEng"></asp:Label>
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-lg-6" style="text-align: center">
                                        <asp:Button runat="server" ID="btn_M" CommandName="M" OnClick="btn_Command_Click" CssClass="ImgMemo_table" /><br />
                                        <asp:Label runat="server" ID="txt_M" Text="บันทึกข้อความ" Font-Size="10" CssClass="lbl_DetailEng"></asp:Label>
                                    </div>
                                    <div class="col-xs-6 col-sm-4 col-lg-12" style="text-align: center">
                                        <asp:Button runat="server" ID="btn_Other" CommandArgument="other" CommandName="Other" OnClick="btn_Command_Click" CssClass="ImgMemo_table" /><br />
                                        <asp:Label runat="server" ID="txt_Other" Text="ฮื่นๆ" Font-Size="10" CssClass="lbl_DetailEng"></asp:Label>
                                    </div>
                                </div>
                                <div class="clearfix visible-xs"></div>
                                <div class="col-xs-12 col-sm-12 col-lg-9">
                                    <div>

                                        <!-- Nav tabs -->
                                        <ul class="nav nav-tabs" role="tablist">
                                            <%-- <li role="presentation" class="active"><asp:HyperLink runat="server" NavigateUrl="#centrePane"  aria-controls="centrePane" role="tab" data-toggle="tab">เอกสารส่วนกลาง</asp:HyperLink></li>
                                                    <li role="presentation"><asp:HyperLink runat="server" NavigateUrl="#internalPane" aria-controls="internalPane"  role="tab" data-toggle="tab">เอกสารภายในหน่วยงาน</asp:HyperLink></li>--%>
                                            <li role="presentation" class="active" runat="server" id="centre_tab">
                                                <asp:LinkButton ID="linkBtn_CentrePane" OnClick="linkBtn_CentrePane_Click" Text="เอกสารส่วนกลาง" role="tab" runat="server"></asp:LinkButton>
                                            </li>
                                            <li role="presentation" runat="server" id="internal_tab">
                                                <asp:LinkButton ID="linkBtn_InternalPane" OnClick="linkBtn_InternalPane_Click" Text="เอกสารภายในหน่วยงาน" role="tab" runat="server"></asp:LinkButton>
                                            </li>

                                        </ul>


                                        <!-- Tab panes -->
                                        <asp:Panel runat="server" DefaultButton="btn_CentreSearch" CssClass="tab-content" Style="border: #ddd 1px solid; border-top-color: transparent; border-radius: 0 0 4px 4px;">
                                            <div role="tabpanel" class="tab-pane fade in active" id="centrePane" style="padding: 0 10px 0 10px; margin-right: 20px;">
                                                <div class="row">
                                                    <div class="col-lg-1">
                                                        <asp:Label ID="Label64" runat="server" Text="Search" CssClass="lbl_ENG"></asp:Label>
                                                    </div>
                                                    <div class="col-lg-11">
                                                        <asp:TextBox runat="server" ID="txt_CentreSearch" placeholder="search" CssClass="form-control input-md" />
                                                    </div>

                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-2" style="padding-right: 0px;">
                                                        <asp:Label ID="Label6" runat="server" Text="Create Date From" CssClass="lbl_ENG"></asp:Label>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                                            <i class="glyphicon glyphicon-calendar"></i>
                                                            <asp:TextBox runat="server" ID="txt_CentreCreateFrom" OnTextChanged="OnDateTimeChanged" AutoPostBack="true" CssClass="form-control datepicker"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2" style="padding-right: 0px;">
                                                        <asp:Label ID="Label7" runat="server" Text="Create Date To" CssClass="lbl_ENG"></asp:Label>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <div class="inner-addon right-addon date" data-provide="datepicker">
                                                            <i class="glyphicon glyphicon-calendar"></i>
                                                            <asp:TextBox runat="server" ID="txt_CentreCreateTo" OnTextChanged="OnDateTimeChanged" AutoPostBack="true" CssClass="form-control datepicker" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-2" style="padding-right: 0;">
                                                        <asp:Label ID="Label1" runat="server" Text="Status" CssClass="lbl_ENG"></asp:Label>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <asp:DropDownList runat="server" ID="ddl_Status" OnSelectedIndexChanged="ddl_Status_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control">
                                                            <asp:ListItem Text="== ALL ==" Value="" />
                                                            <asp:ListItem Text="Completed" Value="Completed" />
                                                            <asp:ListItem Text="Cancelled" Value="Cancelled" />
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-lg-8"></div>
                                                    <div class="col-xs-6 col-sm-6 col-lg-2">
                                                        <asp:Button Text="Search" ID="btn_CentreSearch" OnClick="btn_CentreSearch_Click" CssClass="btn btn-success custom-btn-Success form-control" runat="server" />
                                                    </div>
                                                    <div class="col-xs-6 col-sm-6 col-lg-2">
                                                        <asp:Button Text="Reset" ID="btn_CentreReset" CssClass="btn btn-danger custom-btn-Danger form-control" OnClick="btn_CentreReset_Click" runat="server" />

                                                    </div>
                                                </div>
                                                <br />
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gv_CentreDocList" OnRowDataBound="gv_DocList_RowDataBound" OnSelectedIndexChanged="gv_CentreDocList_SelectedIndexChanged" OnInit="gv_CentreDocList_Init"
                                                        AutoGenerateColumns="false" AllowPaging="true" RowStyle-Wrap="false" PageSize="10" EmptyDataRowStyle-HorizontalAlign="Center" OnPageIndexChanging="gv_CentreDocList_PageIndexChanging"
                                                        ShowHeaderWhenEmpty="true" CssClass="table table-hover table-striped  table-bordered table-condensed header-center col-xs-12 col-sm-12 col-lg-12">

                                                        <Columns>
                                                            <asp:TemplateField Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_DocID" runat="server" Text='<%# Eval("DocID") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <HeaderTemplate>
                                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                                        <span>Document No<br />
                                                                            เลขที่เอกสาร</span>
                                                                    </div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_DocNo" Text="-" runat="server"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <HeaderTemplate>
                                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                                        <span>Other Document Type<br />
                                                                            ประเภทเอกสารย่อย</span>
                                                                    </div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_SubOtherType" Text="-" runat="server"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200">
                                                                <HeaderTemplate>
                                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                                        <span>Title<br />
                                                                            ชื่อเรื่อง</span>
                                                                    </div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <div style="text-align: left; min-width: 300px; white-space: normal;">
                                                                        <asp:Label ID="lbl_title" Text="-" runat="server" Style="word-wrap: break-word; word-break: break-all;"></asp:Label>
                                                                    </div>
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
                                                                    <asp:Label ID="lbl_fromDepartment" runat="server"></asp:Label>
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
                                                                    <%--<asp:Label ID="lbl_DocumentAttachName" runat="server"></asp:Label>--%>
                                                                    <asp:LinkButton OnClick="lbtnDocumentAttachName_Click" ID="lbtnDocumentAttachName" runat="server">
                                                                        <span class="glyphicon glyphicon-file"> <u>View</u></span>
                                                                    </asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <HeaderTemplate>
                                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                                        <span>Created Date<br />
                                                                            วันที่สร้างเอกสาร</span>
                                                                    </div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_CreatedDate" runat="server"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <HeaderTemplate>
                                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                                        <span>Status<br />
                                                                            สถานะเอกสาร</span>
                                                                    </div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_Status" Text="-" runat="server"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderStyle-Width="180px" ItemStyle-HorizontalAlign="Left">
                                                                <HeaderTemplate>
                                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                                        <span>Creator<br />
                                                                            ผู้สร้างเอกสาร</span>
                                                                    </div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_Creator" runat="server"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField Visible="false">
                                                                <HeaderTemplate>
                                                                    <div style="text-align: center; padding: 10px 0 10px 0;">
                                                                        <span>Description<br />
                                                                            รายละเอียดเอกสาร</span>
                                                                    </div>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl_Desctiption" runat="server"></asp:Label>
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
                                            </div>

                                        </asp:Panel>

                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<script>
    function selectTab(x) {
        var centre = document.getElementById('<%=centre_tab.ClientID %>');
        var internal = document.getElementById('<%=internal_tab.ClientID %>');
        if (x === 'centre') {
            if (!centre.classList.contains('active')) {
                centre.classList.add('active');
                internal.classList.remove('active');
            }
        }
        if (x === 'internal') {
            if (!internal.classList.contains('active')) {
                internal.classList.add('active');
                centre.classList.remove('active');
            }
        }
    }
</script>
