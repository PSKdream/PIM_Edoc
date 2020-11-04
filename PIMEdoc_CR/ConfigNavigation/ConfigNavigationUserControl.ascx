<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigNavigationUserControl.ascx.cs" Inherits="PIMEdoc_CR.ConfigNavigation.ConfigNavigationUserControl" %>

<%--<script src="../JS/jquery.min.js"  type="text/javascript"></script>--%>
<%--<script src="../JS/jquery-3.2.1.min.js"  type="text/javascript"></script>--%>
<link href="../Css/stylesMenu.css" rel="stylesheet" />
<%--<script src="../JS/jquery-latest.min.js" type="text/javascript"></script>--%>
<script src="../JS/script.js"  type="text/javascript"></script>
<script type="text/javascript">
    var value;
    $(document).ready(function () {

        //I Banana uncomment
        <%--$('#cssmenu').html($('#<%= Menu1.ClientID%>').val());--%>

        <%--$('#' + '<%=TreeView1.ClientID%>').click(function () {
            
            var treeViewData = window["<%=TreeView1.ClientID%>" + "_Data"];
            if (treeViewData.selectedNodeID.value != "") {

                var selectedNode = document.getElementById(treeViewData.selectedNodeID.value);
                //var value;
                if (selectedNode.href == 'javascript:void(0)') {
                    value = '';
                }
                else {
                    value = selectedNode.href.substring(selectedNode.href.indexOf(",") + 3, selectedNode.href.length - 2);
                    var node = value.split('\\');
                    if (node.length == 3) {
                        value = node[2];
                    }
                }
                //
                document.getElementById("<%=TextBox2.ClientID%>").value = value;
                document.getElementById("<%=hdfNodeValue.ClientID%>").value = value;
                document.getElementById('<%= txtTreeValue.ClientID %>').value = value;
                //__doPostBack('<%= txtTreeValue.ClientID %>', 'Onchange');
                return;
            }
        });--%>

    });
    //I End - ready event

    function ShowPopUpForAddHeader(Action) {
        try {
            //I Validate for current clicked to none link : Begin
            var vNotLinkOrNotZero = document.getElementById("<%=hdfIsHeaderForAddLinkOrAddHeader.ClientID%>").value;
            if (vNotLinkOrNotZero == "0") {
                return false;
            }
            vNotLinkOrNotZero = "1";
            //I Validate for current clicked to none link : End
            /*
            var options = SP.UI.$create_DialogOptions();
            options.title = "Navigation Link";
            options.width = 640;
            options.height = 415;
            options.url = "/_layouts/15/PIMPortal_Theerat/PopUpNavigation.aspx?NodeID=" + value + "&Action=AddHeading";
            //options.url = "/_layouts/15/PIMPortal_Theerat/PopUpNavigation.aspx?NodeID=0000&Action=AddHeading";
            //options.dialogReturnValueCallback = onPopUpCloseCallBackWithDataTemp;
            options.dialogReturnValueCallback = ftRefrash;
            SP.UI.ModalDialog.showModalDialog(options);
            */
            var NodeID = document.getElementById("<%=hdfNodeIdForAddHeader.ClientID%>").value;
            //alert(NodeID);
            if (NodeID != "") {
                //var treeViewData = window["< %=TreeView1.ClientID%>" + "_Data"];
                //if (treeViewData.selectedNodeID.value != "") {
                var treeViewData = window["<%=TreeView1.ClientID%>" + "_Data"];
                var selectedNode = document.getElementById(treeViewData.selectedNodeID.value);
                /*
                var value;
                if (selectedNode.href == 'javascript:void(0)') {
                    value = '';
                }
                else {
                    value = selectedNode.href.substring(selectedNode.href.indexOf(",") + 3, selectedNode.href.length - 2);
                    var node = value.split('\\');
                    try {
                        if (node.length == 3) {
                            value = node[2];
                        }
                    } catch (e) {
                        alert("Error : " + e.message);
                    }
                }
                alert(value);
                return;
                */
                var text = selectedNode.innerHTML;
                var options = SP.UI.$create_DialogOptions();
                options.title = "Navigation Link";
                options.width = 640;
                options.height = 415;
                options.url = "/_layouts/15/PIMEDoc/PopUpNavigation.aspx?NodeID=" + NodeID + "&Action=" + Action
                    + "&Head=" + vNotLinkOrNotZero.toString();
                //options.url = "/_layouts/15/PIMPortal_Theerat/PopUpNavigation.aspx?NodeID=" + value + "&Action=" + Action;
                //options.url = "/_layouts/15/PIMPortal_Theerat/PopUpNavigation.aspx?NodeID=0000&Action=AddHeading";
                //options.url = "/_layouts/15/PIMPortal_Theerat/PopUpNavigation.aspx?NodeID=&Action=AddHeading";
                options.dialogReturnValueCallback = onPopUpCloseCallBackWithDataTemp;
                SP.UI.ModalDialog.showModalDialog(options);
            }
        } catch (e) {
            //  alert("Error [ShowPopUpForAddHeader] : " + e.message);
        }
        return false;
    }

    function ShowPopUp(Action) {
        try {
            //I Validate for current clicked to none link : Begin
            var vNotLinkOrNotZero = document.getElementById("<%=hdfIsHeaderForAddLinkOrAddHeader.ClientID%>").value;
            if (Action != "Edit") {
                if (vNotLinkOrNotZero == "0") {
                    return false;
                }
                vNotLinkOrNotZero = "0";
                //AddLink
            }
            //I Validate for current clicked to none link : End
            var treeViewData = window["<%=TreeView1.ClientID%>" + "_Data"];
            if (treeViewData.selectedNodeID.value != "") {

                var selectedNode = document.getElementById(treeViewData.selectedNodeID.value);
                var value;
                if (selectedNode.href == 'javascript:void(0)') {
                    value = '';
                }
                else {
                    value = selectedNode.href.substring(selectedNode.href.indexOf(",") + 3, selectedNode.href.length - 2);
                    if (value.toString() == "0000") {
                        //I Incase click root (none link)
                        return;
                    }
                    var node = value.split('\\');
                    if (node.length == 3) {
                        value = node[2];
                    }
                    //alert(value);
                }

                var text = selectedNode.innerHTML;
                var options = SP.UI.$create_DialogOptions();
                options.title = "Navigation Link";
                options.width = 640;
                options.height = 415;
                options.url = "/_layouts/15/PIMEDoc/PopUpNavigation.aspx?NodeID=" + value + "&Action=" + Action
                    + "&Head=" + vNotLinkOrNotZero.toString();
                //options.dialogReturnValueCallback = onPopUpCloseCallBackWithDataTemp;
                options.dialogReturnValueCallback = ftRefrash;
                SP.UI.ModalDialog.showModalDialog(options);
            }
        } catch (e) {
            //alert("Exception error [ShowPopUp] : " + e.message.toString());
        }
        return false;
    }

    function onPopUpCloseCallBackWithDataTemp(result, returnValue) {
        if (result == SP.UI.DialogResult.OK) {
            window.location = document.URL;
        }
    }
</script>
<style type="text/css">
    /*img[alt$='-1'] {
        margin-left: -20px;
    }*/
    .hiddencontrol {
        display: none;
    }
    ul>li { border-left: 1px solid #fff; }
    li:first-child {
	    border-left: 0px solid #000000 !important;
    }
    ul:not(.level1) > :first-child {
        border-left: 1px solid #ffffff !important;
    }
    .clsMenuSeparator a.highlighted.static {
        border-top: 3px solid #ffffff !important;
        background-color: #5f3870;
        color: #d5bede;
    }
    div.clsMenuSeparator > ul > li > ul {
        z-index: 99 !important;
    }
    /* Style of table navigation : Begin */
    .clsTableNavigaionMain {
        width: 400px;
        margin-left: 50px;
        font-family: Arial !important;
        font-size: 13px;
    }
    .clsTitleOfTableNavigation {
        padding: 2px 2px 2px 4px;
        border: 1px solid rgb(0, 0, 0);
        color: rgb(59, 59, 59);
        background-color: white;
    }
    .clsDisplayTextOfTableNavigation {
        border-width: 0px 1px 1px;
        border-style: solid;
        border-color: rgb(0, 0, 0);
        padding: 2px 2px 2px 4px;
        color: rgb(59, 59, 59);
        background-color: white;
    }
    .clsImgColumn {
        width: 16px;
        padding-right: 2px;
    }
    .clsButtonColumn{
        width: 16%;
        padding-left: 2px;
    }
    .clsImgMoveUp {
        /*background-image: url('../Img/MoveUp.gif');*/
        background-repeat: no-repeat;
        cursor: pointer;
        width: 16px;
        height: 16px;
    }
    .clsImgMoveDown {
        /*background-image: url('../Img/MoveDown.gif');*/
        background-repeat: no-repeat;
        cursor: pointer;
        width: 16px;
        height: 16px;
    }
    .clsImgEdit {
        /*background-image: url('../Img/Edit.png');*/
        background-repeat: no-repeat;
        cursor: pointer;
        width: 16px;
        height: 16px;
    }
    .clsImgDelete {
        /*background-image: url('../Img/BtnDelete.png');*/
        background-repeat: no-repeat;
        cursor: pointer;
        width: 16px;
        height: 16px;
    }
    .clsImgAddHead {
        /*background-image: url('../Img/folder.gif');*/
        background-repeat: no-repeat;
        cursor: pointer;
        width: 16px;
        height: 16px;
    }
    .clsImgAddLink {
        /*background-image: url('../Img/link.gif');*/
        background-repeat: no-repeat;
        cursor: pointer;
        width: 16px;
        height: 16px;
    }
    .clsButtonTitle {
        cursor: pointer;
        background-color: #fff !important;
        margin-left: 0px !important;
        white-space: normal;
        border: 0px solid #ababab !important;
        text-align: left;
    }
    
    /* Style of table navigation : End */
</style>
<asp:UpdateProgress ID="UpdateProgress1" runat="server">
    <ProgressTemplate>
       <%-- <uc2:Progress ID="Progress1" runat="server" />--%>
    </ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel runat="server">
    <%--<Triggers>
        <asp:PostBackTrigger ControlID="TreeView1" />
    </Triggers>--%>
    <ContentTemplate>
        <table class="clsTableNavigaionMain">
            <tbody>
                <tr><!-- Row 1 begin -->
                    <td class="clsTitleOfTableNavigation">
                        <table width="100%" class="ms-toolbar">
                            <tbody>
                                <tr>
                                    <td class="clsImgColumn">
                                        <img alt="" class="clsImgMoveUp" onclick="ftButtonClick('<%=btnMoveUp.ClientID%>');"
                                            src="../Img/MoveUp.gif" />
                                    </td>
                                    <td class="clsButtonColumn">
                                        <asp:Button ID="btnMoveUp" runat="server" Text="Move Up" CssClass="clsButtonTitle" OnClick="btnMoveUp_Click" AutoPostBack="True" />
                                    </td>
                                    <td class="clsImgColumn">
                                        <img alt="" class="clsImgMoveDown" onclick="ftButtonClick('<%=btnMoveDown.ClientID%>');"
                                            src="../Img/MoveDown.gif" />
                                    </td>
                                    <td class="clsButtonColumn">
                                        <asp:Button ID="btnMoveDown" runat="server" CssClass="clsButtonTitle" Text="Move Down" OnClick="btnMoveDown_Click" AutoPostBack="True" />
                                    </td>
                                    <td class="clsImgColumn">
                                        <img alt="" class="clsImgEdit" onclick="ftButtonClick('<%=btnEdit.ClientID%>');"
                                            src="../Img/Edit.png" />
                                    </td>
                                    <td class="clsButtonColumn">
                                        <asp:Button ID="btnEdit" runat="server" CssClass="clsButtonTitle" Text="Edit..." OnClientClick="return ShowPopUp('Edit')" />
                                    </td>
                                    <td class="clsImgColumn">
                                        <img alt="" class="clsImgDelete" onclick="ftButtonClick('<%=btnDelete.ClientID%>');"
                                            src="../Img/BtnDelete.png" />
                                    </td>
                                    <td class="clsButtonColumn">
                                        <asp:Button ID="btnDelete" runat="server" CssClass="clsButtonTitle" Text="Delete" OnClientClick="return confirm('Confirm Delete?');" OnClick="btnDelete_Click" />
                                    </td>
                                    <td class="clsImgColumn">
                                        <img alt="" class="clsImgAddHead" onclick="ftButtonClick('<%=btnAddHeading.ClientID%>');"
                                            src="../Img/folder.gif" />
                                    </td>
                                    <td class="clsButtonColumn">
                                        <asp:Button ID="btnAddHeading" runat="server" CssClass="clsButtonTitle" Text="Add Heading" OnClientClick="return ShowPopUpForAddHeader('AddHeading')" />
                                    </td>
                                    <td class="clsImgColumn">
                                        <img alt="" class="clsImgAddLink" onclick="ftButtonClick('<%=btnAddLink.ClientID%>');"
                                            src="../Img/link.gif" />
                                    </td>
                                    <td class="clsButtonColumn">
                                        <asp:Button ID="btnAddLink" runat="server" CssClass="clsButtonTitle" Text="Add Link" OnClientClick="return ShowPopUp('AddLink')" />
                                    </td>
                                </tr>
			                </tbody>
                        </table>
                    </td>
                </tr>
                <!-- Row 1 end -->
                <tr><!-- Row 2 begin -->
                    <td class="clsTitleOfTableNavigation">
                        <table width="100%" class="ms-toolbar">
                            <tbody>
                                <tr>
					                <td>
                                        <table width="100%" class="ms-toolbar">
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TreeView ID="TreeView1" runat="server"
                                                            SelectedNodeStyle-BackColor="Silver" Target="_self"
                                                            OnSelectedNodeChanged="TreeView1_SelectedNodeChanged" 
                                                            CollapseImageUrl="../../../../Img/treeFold.png"
                                                            ExpandImageUrl="../../../../Img/treeExpand.gif" 
                                                            >
                                                            <%--CollapseImageUrl="../../../../Img/treeFold.png" ExpandImageUrl="../../../../Img/treeExpand.gif" NoExpandImageUrl="../../../../Img/link.gif" --%>
                                                            <HoverNodeStyle Font-Underline="True" ForeColor="#6666AA" Font-Names="Arial" Font-Size="13px" />
                                                            <Nodes>
                                                                <asp:TreeNode Text="Root" Value="0000" Selected="False"></asp:TreeNode>
                                                            </Nodes>
                                                            <NodeStyle Font-Names="Arial" Font-Size="13px" ForeColor="Black" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="1px" />
                                                            <ParentNodeStyle Font-Bold="False" Font-Names="Arial" Font-Size="13px" />
                                                            <SelectedNodeStyle BackColor="#B5B5B5" Font-Underline="False" HorizontalPadding="5px" VerticalPadding="0px" Font-Names="Arial" Font-Size="13px" />
                                                        </asp:TreeView>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
					                </td>
				                </tr>
			                </tbody>
                        </table>
                    </td>
                </tr>
                <!-- Row 2 end -->
            </tbody>
        </table>
        
        <br />
        <table class="clsTableNavigaionMain">
            <tbody>
                <tr>
                    <td class="clsTitleOfTableNavigation">
                        <table width="100%" class="ms-toolbar">
                            <tbody>
                                <tr>
					                <td>Selected Item</td>
				                </tr>
			                </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="clsDisplayTextOfTableNavigation">
                        <div id="InsertInspectorPane">
                          <div style="margin: 5px 0px;">
                            <b>Title : </b>
                              <asp:Label ID="lblTitle" runat="server" Text=""></asp:Label>
                          </div>
                          <div style="margin: 5px 0px;">
                            <b>URL : </b>
                              <asp:Label ID="lblUrl" runat="server" Text=""></asp:Label>
                          </div>
                          <div style="margin: 5px 0px;">
                            <b>Description : </b>
                              <asp:Label ID="lblDescript" runat="server" Text=""></asp:Label>
                          </div>
                          <div style="margin: 5px 0px;">
                            <b>Level : </b>
                              <asp:Label ID="lblLevel" runat="server" Text=""></asp:Label>
                          </div>
                          <div style="margin: 5px 0px;">
                            <b>Parent : </b>
                              <asp:Label ID="lblParent" runat="server" Text=""></asp:Label>
                          </div>
                          <div style="margin: 5px 0px;">
                            <b>Sequence : </b>
                              <asp:Label ID="lblSequence" runat="server" Text=""></asp:Label>
                          </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        <asp:TextBox ID="txtTreeValue"  OnTextChanged="TextBoxClick_TextChanged" AutoPostBack="true" runat="server" Visible="False" />
        <asp:Button ID="btnRefrash" runat="server" Text="Refrash" OnClick="btnRefrash_Click" style="display:none;" />
        &nbsp;<asp:HiddenField ID="HiddenSiteURL" runat="server" />
        <asp:HiddenField ID="hdfNodeValue" runat="server" Value="Text from hidden field." />
        <asp:HiddenField ID="hdfNodeIdForAddHeader" runat="server" />
        <asp:HiddenField ID="hdfIsHeaderForAddLinkOrAddHeader" runat="server" Value="" />
        <br /><br />
        
    </ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    //setHeaderImg(); ft1();
    try {
        <%--alert(document.getElementById("<%=hdfNodeValue.ClientID%>").value);--%>
        //alert(value);
        <%--document.getElementById("<%=hdfNodeValue.ClientID%>").value = value.toString();--%>
        <%--var tmp = document.getElementById("<%=hdfNodeValue.ClientID%>").value;--%>
        //alert(tmp.toString());
        //$("div[id*='TreeView1'] > table > tbody > tr > td > img").src = "../Img/treeExpand.gif";
        //I Same : jsgei,jsGetElementById1,jsElementGetting1
        <%--document.getElementById("<%=TreeView1.ClientID%>").src = "../Img/treeExpand.gif";--%>
        //setHeaderImg();
        //DelTagTd();

        function setHeaderImg() {
            try {
                var oTmp = document.querySelectorAll("div#" + "<%=TreeView1.ClientID%>"
                    + "n0Nodes > table > tbody > tr > td > img");
                if (oTmp != null) {
                    for (var i1 = 0 ; i1 < oTmp.length ; i1++) {
                        oTmp[i1].src = "../Img/treeExpand.gif";
                    }
                }
            } catch (e) {
                var str = "Error : " + e.message;
            }
        }

        function ftRefrash() {
            try {
                document.getElementById("<%=btnRefrash.ClientID%>").click();
            } catch (e) {
                var str = "Error : " + e.message;
            }
        }
        //var tmp1 = 10;
        function ftButtonClick(argID) {
            try {
                document.getElementById(argID).click();
            } catch (e) {
                var str = "Error : " + e.message;
            }
        }
    } catch (e) {
        //alert("cannot get hidden file element");
        //alert("Exception error [Ready event] : " + e.message.toString());
    }

    function DelTagTd() {
        try {
            var elemAllDel = document.getElementsByClassName("clsRemoveTd");
            for (var i1 = 0; i1 < elemAllDel.length; i1++) {
                var NodeDel = elemAllDel[i1].parentNode.parentNode.previousSibling.previousSibling;
                var ParentDel = NodeDel.parentNode;
                ParentDel.removeChild(NodeDel);
            }
        } catch (e) {
            //alert("Exception error [DelTagTd] : " + e.message.toString());
        }
    }

</script>