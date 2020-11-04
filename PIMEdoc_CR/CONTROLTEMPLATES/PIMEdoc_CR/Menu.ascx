<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="PIMEdoc_CR.ControlTemplates.PIMEdoc_CR.Menu" %>
<meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes" />
<%--<meta http-equiv="X-UA-Compatible" content="IE=Edge">--%>
<meta http-equiv="X-UA-Compatible" content="IE=8">
<link href="/Css/styles.css" rel="stylesheet" />
<link href="/Css/stylesMenu.css" rel="stylesheet" />
<script src="/JS/jquery-3.2.1.min.js" type="text/javascript"></script>
<script src="/JS/script.js"></script>

<style type="text/css">
    #s4-workspace {
        width: 100% !important;
        overflow-x: hidden !important;
        overflow-y: auto !important;
    }

    #titleAreaBox {
        width: 100% !important;
    }

    #cssmenu {
        display: none;
    }

    @media screen and (max-width: 780px) {
        /* Menu responsing : Begin */
        div.clsMenuSeparator {
            display: none !important;
        }
        /* Set show menu icon */
        div.CssMenuDivNav > div:nth-child(1) {
            /*display: block !important;*/
        }
        /* Set show menu icon */
        div.CssMenuDivNav > div:nth-child(2) {
            display: block !important;
        }
        /* Set show menu icon (ใช้ในหน้า worklist) */
        div.CssMenuDivNav > div:nth-child(3) {
            /*display: none !important;*/
        }
        /* Set hidden menu icon (ใช้ในหน้า worklist) */
        div.CssMenuDivNav > div:nth-child(4) {
            display: none !important;
        }
        /* Set size of div element ที่เก็บ menu และ icon ของ K2 */
        div.div_body.CssMenu > div:nth-child(2) {
            width: 400px !important;
        }
        /*
        .five.column {
            width: 40% !important;
        }
        */
        .thirds-five.column {
            width: 20% !important;
        }
    }

    .thirds-five.column {
        width: 85%;
    }
    /* Set size of div element ที่เก็บ menu และ icon ของ K2 */
    div.div_body.CssMenu > div:nth-child(2) {
        width: 100% !important;
    }

    div.clsMenuSeparator > ul > li {
        position: relative !important;
        float: left !important;
    }

    li.static {
        position: relative !important;
        float: left !important;
    }

    a.IconApproveNotice {
        min-width: 17px;
    }

    @media screen and (max-width: 780px) {

        div#idDivMain {
            margin-top: -40px;
            display: block;
        }

        table.TableClass {
            width: 310px !important;
        }

        #cssmenu {
            display: block;
        }

        li:first-child {
            border-left: 0px solid #000000 !important;
        }

        ul:not(.level1) > :first-child {
            border-left: 0px solid #ffffff !important;
        }

        .dropdown {
            position: relative;
            font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
            font-size: 12px;
        }

        li.dropdown {
        }

        .nav > li > a {
            position: relative;
            display: block;
        }

        .nav-pills > li > a {
            border-radius: 4px;
        }

        a.dropdown {
            position: relative;
            display: block;
            padding: 10px 15px;
            border-radius: 4px;
            margin-top: -15px;
            text-decoration: none !important;
            color: #fff;
            margin: 2px;
        }

        .caret {
            display: inline-block;
            width: 0;
            height: 0;
            margin-left: 2px;
            vertical-align: middle;
            border-top: 4px solid;
            border-right: 4px solid transparent;
            border-left: 4px solid transparent;
        }

        .nav {
            display: block;
            font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
            font-size: 12px;
            padding-left: 0;
            margin-bottom: 0;
        }

            .nav > li {
                position: relative;
                display: block;
            }

                .nav > li > a:hover,
                .nav > li > a:focus {
                    text-decoration: none;
                    background-color: #eee;
                }

                .nav > li.disabled > a {
                    color: #777;
                }

                    .nav > li.disabled > a:hover,
                    .nav > li.disabled > a:focus {
                        color: #777;
                        text-decoration: none;
                        cursor: not-allowed;
                        background-color: transparent;
                    }

            .nav .open > a,
            .nav .open > a:hover,
            .nav .open > a:focus {
                background-color: #eee;
                border-color: #337ab7;
            }

            .nav .nav-divider {
                height: 1px;
                margin: 9px 0;
                overflow: hidden;
                background-color: #e5e5e5;
            }

            .nav > li > a > img {
                max-width: none;
            }

        .nav-tabs {
            border-bottom: 1px solid #ddd;
        }

            .nav-tabs > li {
                float: left;
                margin-bottom: -1px;
            }

                .nav-tabs > li > a {
                    margin-right: 2px;
                    line-height: 1.42857143;
                    border: 1px solid transparent;
                    border-radius: 4px 4px 0 0;
                }

                    .nav-tabs > li > a:hover {
                        border-color: #eee #eee #ddd;
                    }

                .nav-tabs > li.active > a,
                .nav-tabs > li.active > a:hover,
                .nav-tabs > li.active > a:focus {
                    color: #555;
                    cursor: default;
                    background-color: #fff;
                    border: 1px solid #ddd;
                    border-bottom-color: transparent;
                }

            .nav-tabs.nav-justified {
                width: 100%;
                border-bottom: 0;
            }

                .nav-tabs.nav-justified > li {
                    float: none;
                }

                    .nav-tabs.nav-justified > li > a {
                        margin-bottom: 5px;
                        text-align: center;
                    }

                .nav-tabs.nav-justified > .dropdown .dropdown-menu {
                    top: auto;
                    left: auto;
                }

        @media (min-width: 768px) {
            .nav-tabs.nav-justified > li {
                display: table-cell;
                width: 1%;
            }

                .nav-tabs.nav-justified > li > a {
                    margin-bottom: 0;
                }
        }

        .nav-tabs.nav-justified > li > a {
            margin-right: 0;
            border-radius: 4px;
        }

        .nav-tabs.nav-justified > .active > a,
        .nav-tabs.nav-justified > .active > a:hover,
        .nav-tabs.nav-justified > .active > a:focus {
            border: 1px solid #ddd;
        }

        @media (min-width: 768px) {
            .nav-tabs.nav-justified > li > a {
                border-bottom: 1px solid #ddd;
                border-radius: 4px 4px 0 0;
            }

            .nav-tabs.nav-justified > .active > a,
            .nav-tabs.nav-justified > .active > a:hover,
            .nav-tabs.nav-justified > .active > a:focus {
                border-bottom-color: #fff;
            }
        }

        .nav-pills > li {
            float: left;
        }

            .nav-pills > li > a {
                border-radius: 4px;
            }

            .nav-pills > li + li {
                margin-left: 2px;
            }

            .nav-pills > li.active > a,
            .nav-pills > li.active > a:hover,
            .nav-pills > li.active > a:focus {
                color: #fff;
                background-color: #c7b9d0;
                /*background-color: #337ab7;*/
            }

        .nav-stacked > li {
            float: none;
        }

            .nav-stacked > li + li {
                margin-top: 2px;
                margin-left: 0;
            }

        .nav-justified {
            width: 100%;
        }

            .nav-justified > li {
                float: none;
            }

                .nav-justified > li > a {
                    margin-bottom: 5px;
                    text-align: center;
                }

            .nav-justified > .dropdown .dropdown-menu {
                top: auto;
                left: auto;
            }

        @media (min-width: 768px) {
            .nav-justified > li {
                display: table-cell;
                width: 1%;
            }

                .nav-justified > li > a {
                    margin-bottom: 0;
                }
        }

        .nav-tabs-justified {
            border-bottom: 0;
        }

            .nav-tabs-justified > li > a {
                margin-right: 0;
                border-radius: 4px;
            }

            .nav-tabs-justified > .active > a,
            .nav-tabs-justified > .active > a:hover,
            .nav-tabs-justified > .active > a:focus {
                border: 1px solid #ddd;
            }

        @media (min-width: 768px) {
            .nav-tabs-justified > li > a {
                border-bottom: 1px solid #ddd;
                border-radius: 4px 4px 0 0;
            }

            .nav-tabs-justified > .active > a,
            .nav-tabs-justified > .active > a:hover,
            .nav-tabs-justified > .active > a:focus {
                border-bottom-color: #fff;
            }
        }

        .dropup .caret,
        .navbar-fixed-bottom .dropdown .caret {
            content: "";
            border-top: 0;
            border-bottom: 4px solid;
        }

        .dropup .dropdown-menu,
        .navbar-fixed-bottom .dropdown .dropdown-menu {
            top: auto;
            bottom: 100%;
            margin-bottom: 1px;
        }

        .nav > li > a:hover, .nav > li > a:focus {
            text-decoration: none;
            background-color: #c7b9d0;
        }

        ul.dropdown > li > a:hover, ul.dropdown > li > a:focus {
            text-decoration: none;
            background-color: #c7b9d0;
        }

        li.dropdown > ul.dropdown {
            margin-left: -25px !important;
        }

        ul.dropdown {
            /*list-style: none !important;*/
        }
    }

    .cssBadge {
        position: absolute;
        margin-left: -26px;
        margin-top: -7px;
        padding: 4px 7px;
        border-radius: 50%;
        min-width: 25px;
        color: white !important;
        font-weight: bolder;
        min-height: 25px;
    }

    .bgRed {
        background: red !important;
    }

    .CssDynamicMenu {
        display: none;
    }

    ul.level1.static {
        width: auto !important;
        float: left !important;
        position: relative !important;
    }

    .clsMenuSeparator a.highlighted.static {
        /*border-top: 3px solid #ffffff !important;
        background-color: #03873A;*/
        color: #d5bede;
    }

    div.clsMenuSeparator > ul > li > ul {
        z-index: 99 !important;
    }

    .clsMenuStatic {
        background-color: #03873A !important;
        padding: 20px 20px 20px 20px !important;
        color: white !important;
        font-size: 22px !important;
        text-decoration: none;
        display: block !important;
    }

    div#idDivMain {
        margin-top: -40px;
        display: none;
    }

    div.clsMenuSeparator {
        display: block;
    }

        div.clsMenuSeparator > ul > li {
            position: relative !important;
            float: left !important;
        }

    li.static {
        position: relative !important;
        float: left !important;
    }
</style>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <contenttemplate>
        <div class="div_body CssMenu">
            
            <div style="background: #01883C; float: left; width: 100%">
                <div style="max-width: 1100px; width: 100%; margin: 0 auto">
                    <div class="menu" style="float:left;display:block;">
                        <div id="cssmenu" class="CssMenuDivNav">
                            
        <!-- navbar-brand is hidden on larger screens, but visible when the menu is collapsed -->
        <a class="navbar-brand" href="#">
        </a>
                            <div id="menu-line"></div>
                            <div id="menu-button" class="clsToggleMenu" onclick="$('#idDivMain').slideToggle();if((typeof ftChgHdfOfMenu).toString() == 'function'){ftChgHdfOfMenu();}"></div>
                        </div>
                        <asp:HiddenField ID="hdfFlagShowMenu" runat="server" Value="0" />
                        <asp:Menu ID="Menu1" runat="server" Orientation="Horizontal" BackColor="#FFFFFF" DynamicHorizontalOffset="2" Font-Names="TH SarabunPSK" Font-Size="0.8em" ForeColor="#03873A" StaticSubMenuIndent="10px" PathSeparator="\" CssClass="clsMenuSeparator">
                            <DynamicHoverStyle BackColor="#FFFFFF" ForeColor="#03873A" />
                            <DynamicMenuItemStyle BackColor="#FFFFFF" ForeColor="#03873A" HorizontalPadding="20px" VerticalPadding="10px" Font-Size="22px" />
                            <DynamicMenuStyle CssClass="CssDynamicMenu" />
                            <DynamicSelectedStyle />
                            <Items>
                            </Items>
                            <%--<StaticMenuItemStyle HorizontalPadding="20px" VerticalPadding="20px" ForeColor="White" BackColor="#03873A" Font-Size="12px" />--%>
                            <StaticMenuItemStyle CssClass="clsMenuStatic" />
                            <StaticSelectedStyle />
                        </asp:Menu>
                        <div class="row" id="idDivMain" style="margin-top:-40px;width:155%;display:none;">
    
                            <div class="col-md-3">
                                <%//Response.Write(this.sElementOfMobile);%>
                                <%=this.sElementOfMobile%>
                            </div>
                            <div class="clearfix visible-lg"></div>

                        </div>
                        <script type="text/javascript">
                            var bReduceSize = false;
                            var bFlagShowMenu = false;
                            /*
                            //typeof ftChgHdfOfMenu                  // Returns undefined (if myCar is not declared)
                            //if((typeof ftChgHdfOfMenu).toString() == "function"){ftChgHdfOfMenu();}
                            //alert("Test");
                            */
                            function ftChgHdfOfMenu() {
                                try {
                                    var jsHdf1 = document.getElementById("<%=hdfFlagShowMenu.ClientID%>");
                                    if (jsHdf1.value == "0") {
                                        jsHdf1.value = "1";
                                    } else {
                                        jsHdf1.value = "0";
                                    }
                                } catch (e) {
                                    /*alert("Exception error [ftChgHdfOfMenu] : " + e.message.toString());*/
                                }
                            }
                            function ftPageLoad_Menu() {
                                try {
                                    var tmpSizeMenu = $(window).width();
                                    if (tmpSizeMenu < 780) {
                                        var jsHdf1 = document.getElementById("<%=hdfFlagShowMenu.ClientID%>");
                                        if (jsHdf1.value == "0") {
                                            $("#idDivMain").slideUp(0);
                                        } else {
                                            $("#idDivMain").slideDown(0);
                                        }
                                    }
                                    var sTypeName = (typeof ftRe_RenderSlideImage).toString();
                                    if (sTypeName != "undefined") {
                                        if ((typeof ftRe_RenderSlideImage) == "function") {
                                            ftRe_RenderSlideImage();
                                        }
                                    }
                                } catch (e) {
                                    /*alert("Exception error [ftPageLoad] : " + e.message.toString());*/
                                }
                            }
                            function ftToggleMenu(argElem) {
                                try {
                                    $(argElem).next().slideToggle("slow");
                                    if ($(argElem).children("span").attr("class") == "dropup") {
                                        $(argElem).children("span").attr("class", "dropdown");
                                    } else {
                                        if ($(argElem).children("span").attr("class") == "dropdown") {
                                            $(argElem).children("span").attr("class", "dropup");
                                        }
                                    }
                                }
                                catch (err) {
                                    /*alert("Exception : " + err.message);*/
                                }
                            }
                            $(document).ready(function () {
                                $("#idDivMain").slideUp(0);

                                var tmpSize = $(window).width();
                                if (tmpSize < 780) {
                                    bReduceSize = true;
                                } else {
                                    bReduceSize = false;
                                }

                                /*menu-button*/
                                $("#cssmenu").click(function () {
                                    $("#idDivMain").slideToggle();
                                    /*bFlagShowMenu = !bFlagShowMenu;*/
                                    ftChgHdfOfMenu();
                                });

                                $(window).resize(function () {
                                    var tmpSize = $(window).width();
                                    if (tmpSize < 780) {
                                        var jsHdf1 = document.getElementById("<%=hdfFlagShowMenu.ClientID%>");
                                        if (jsHdf1.value == "0") {
                                            $("#idDivMain").slideUp(0);
                                        } else {
                                            $("#idDivMain").slideDown(0);
                                        }

                                        bReduceSize = true;
                                    } else {
                                        $("#idDivMain").slideUp(0);
                                        bReduceSize = false;
                                    }


                                });
                            });
                            /*End - ready event*/
                        </script>
                          </div>
                    <div class="column" style="padding-top: 5px;float:right;">                      
                        <asp:Button ID="btnK2Task" runat="server" CssClass="IconK2" OnClick="btnK2Task_Click" />
                        <asp:Label runat="server" ID="lbl_WorklistCounter" Text="0" CssClass="cssBadge bgRed"></asp:Label>
                        <asp:HyperLink ID="LinkK2Task" runat="server" Target="_blank" onclick=""></asp:HyperLink>
                        <asp:Button ID="btnLang" runat="server" CssClass="IconLangTH" OnClick="btnLang_Click" />
                    </div>
                </div>
            </div>
        </div>
        
        <asp:HiddenField ID="HiddenLinkReqNeedApproveCount" runat="server" />
        <asp:HiddenField ID="HiddenHomeDisplay" runat="server" />
    
    </contenttemplate>
</asp:UpdatePanel>
