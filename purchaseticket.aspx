<%@ Page Language="C#" AutoEventWireup="true" CodeFile="purchaseticket.aspx.cs" Inherits="BCEC.control_admin_purchaseticket"
    ValidateRequest="false" MasterPageFile="adminmaster.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

    <script type="text/javascript" language="javascript" src="../../includes/script/eventdetail.js"></script>

    <script type="text/javascript" src="../../includes/script/checkoutevent.js"></script>

    <script src="../../includes/script/imagepreview.js" type="text/javascript"></script>

    <script src="includes/script/purchaseticket.js" type="text/javascript"></script>

    <link href="includes/css/addeditevent.css" rel="stylesheet" type="text/css" />

    <script src="includes/script/Script.js" type="text/javascript"></script>

    <script type="text/javascript">
        function CheckPOS() {
            if (document.getElementById('ctl00_ContentPlaceHolder1_txtPOS').value == '') {
                alert('Please enter POS Code');
                return false;
            }
            else {
                return true;
            }

        }
    </script>

    <center>
        <form id="form1" runat="server">
        <div id="content_div">
            <asp:ScriptManager ID="sm1" runat="server">
            </asp:ScriptManager>
            <table cellpadding="0" cellspacing="0" style="width: 100%" align="center" class="input_table">
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <table style="width: 80%">
                            <tr>
                                <td class="form_header" colspan="2">
                                    <span>
                                        <asp:Literal ID="Literal1" runat="server">Ticket purchasing directly</asp:Literal></span>
                                </td>
                            </tr>
                            <tr>
                                <td class="input_form_caption_td" nowrap="nowrap">
                                    &nbsp;
                                </td>
                                <td align="left" width="70%">
                                    <asp:Literal ID="litMsg" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="input_form_caption_td" nowrap="nowrap">
                                    Select Event: <span>*</span>
                                </td>
                                <td align="left" width="70%">
                                    <asp:DropDownList ID="ddlEvent" runat="server" width="270px" AutoPostBack="true" class="listbox"
                                        OnSelectedIndexChanged="ddlEvents_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="hdnAdditionalDiscount" runat="server" Value="0" />
                                    <asp:HiddenField ID="hdnAllowNamesLater" runat="server" Value="0" />
                                </td>
                            </tr>
                            <tr>
                                <td class="input_form_caption_td" nowrap="nowrap">
                                    Select Date:<span>*</span>
                                </td>
                                <td align="left" width="70%">
                                    <asp:DropDownList ID="ddlDate" runat="server" class="listbox" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlDate_SelectedIndexChanged">
                                        <asp:ListItem Text="Select" Value="0" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="input_form_caption_td" nowrap="nowrap">
                                    Select Seating Area: <span>*</span>
                                </td>
                                <td align="left" width="70%">
                                    <asp:UpdateProgress runat="server" ID="PageUpdateProgress">
                                        <ProgressTemplate>
                                            <img src="../../images/search_loader.gif" alt="Loading..." />
                                        </ProgressTemplate>
                                    </asp:UpdateProgress>
                                    <asp:DropDownList ID="ddlSeatingArea" runat="server" class="listbox" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlSeatingArea_SelectedIndexChanged">
                                        <asp:ListItem Text="Select" Value="0" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="background-color:#D2D2D2;">
                                    <fieldset class="fieldset">
                                        <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                            <tr class="fieldset" id="trinvnumber" runat="server">
                                                <td class="input_form_caption_td" nowrap="nowrap" align="left"  style="background-color:#D2D2D2;border-bottom:none;">
                                                    Select Invoice No:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                </td>
                                                <td align="left" width="70%" style="background-color:#D2D2D2;border-bottom:none;">
                                                    <asp:DropDownList ID="ddlInvoiceNo" runat="server" AutoPostBack="true" class="listbox"
                                                        OnSelectedIndexChanged="ddlInvoiceNo_SelectedIndexChanged">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="height: 4px;background-color:#D2D2D2;border-bottom:none;">&nbsp;
                                                </td>
                                            </tr>
                                            <tr id="trinvstatus" runat="server" style="background-color:#D2D2D2;border-bottom:none;">
                                                <td class="input_form_caption_td" nowrap="nowrap" align="left">
                                                    Invoice Status:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                </td>
                                                <td align="left" width="70%" style="background-color:#D2D2D2;border-bottom:none;">
                                                    <asp:DropDownList ID="ddlInvoiceStatus" runat="server" AutoPostBack="false" class="listbox">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <table style="width: 80%" runat="server" id="tableTickets">
                            <tr runat="server" id="travailaible">
                                <td class="form_header">
                                    <span>
                                        <asp:Literal ID="Literal2" runat="server">Select your tickets</asp:Literal></span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span style="color: Red;">
                                        <asp:Label ID="lblTickets" runat="server" ForeColor="Red"></asp:Label></span>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:UpdatePanel ID="updpnlgrdTickets" runat="server">
                                        <ContentTemplate>
                                            <asp:GridView ID="grdTickets" runat="server" GridLines="None" AutoGenerateColumns="false"
                                                Width="100%" CellSpacing="3" CellPadding="0" BorderWidth="0" OnRowDataBound="grdTickets_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                        <HeaderTemplate>
                                                            <asp:CheckBox ID="chkAll" onclick="CheckAll(this,'aspnetForm','ctl00_ContentPlaceHolder1_grdTickets')"
                                                                runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chk" runat="server" />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="4%" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Quantity" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                                        ItemStyle-VerticalAlign="Middle">
                                                        <ItemTemplate>
                                                            <asp:TextBox CssClass="eventtextbox" ID="txtQuantity" onKeyPress="return numberKeypress(event);"
                                                                runat="server" MaxLength="4"></asp:TextBox>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="20%" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                        ItemStyle-VerticalAlign="Middle">
                                                        <ItemTemplate>
                                                            <asp:UpdatePanel ID="updpnlddlType" runat="server">
                                                                <ContentTemplate>
                                                                    <asp:DropDownList ID="ddlType" runat="server" Enabled='<%# Convert.ToInt32(Eval("IS_TABLE"))>0?true:false %>'
                                                                        OnSelectedIndexChanged="ddlType_SelectedIndexChanged" AutoPostBack="true">
                                                                        <asp:ListItem Text="Individual" Value="INDIVIDUAL" Selected="True"></asp:ListItem>
                                                                        <asp:ListItem Text="Group" Value="TABLE"></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                            <asp:UpdatePanel ID="updpnlddlTableType" runat="server">
                                                                <ContentTemplate>
                                                                    <asp:DropDownList ID="ddlTableType" Visible="false" OnSelectedIndexChanged="ddlTableType_SelectedIndexChanged"
                                                                        runat="server" AutoPostBack="true">
                                                                    </asp:DropDownList>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="20%" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Description" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                        ItemStyle-VerticalAlign="Middle">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litTicketTypeName" runat="server" Text='<%#Eval("TICKET_TYPE_NAME") %>' />
                                                            <asp:HiddenField ID="hdnTicketTypeID" runat="server" Value='<%#Eval("TICKET_TYPE_ID") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="20%" CssClass="textnormal" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Quantity Remaining" ItemStyle-HorizontalAlign="Center"
                                                        HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litRemQty" runat="server" Text='<%#Eval("TICKET_TYPE_NAME") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="10%" CssClass="textnormal" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Price" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                        ItemStyle-VerticalAlign="Middle">
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litSign" runat="server" Visible="true" Text="$"></asp:Literal><asp:Literal
                                                                ID="litPrice" runat="server" Text='<%#Eval("PRICE") %>' Visible="true"></asp:Literal>
                                                            <asp:HiddenField ID="hdnPrice" runat="server" Value='<%#Eval("PRICE") %>' />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="12%" CssClass="textnormal" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Group Price" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                        ItemStyle-VerticalAlign="Middle">
                                                        <ItemTemplate>
                                                            <%--<asp:Literal ID="litTablePrice" runat="server" Text='<%#Eval("TABLE_PRICE") %>'></asp:Literal>--%>
                                                            <asp:Literal ID="litTablePrice" runat="server"></asp:Literal>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="13%" CssClass="textnormal" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Seats" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                        ItemStyle-VerticalAlign="Middle">
                                                        <ItemTemplate>
                                                            <%--<asp:Literal ID="litTablePrice" runat="server" Text='<%#Eval("TABLE_PRICE") %>'></asp:Literal>--%>
                                                            <asp:Literal ID="litTableSeats" runat="server"></asp:Literal>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="20%" CssClass="textnormal" />
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <center>
                                                        <strong>No Tickets Available Today</strong></center>
                                                </EmptyDataTemplate>
                                                <HeaderStyle CssClass="blacktext" />
                                                <RowStyle Height="27" />
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <asp:ImageButton ImageUrl="../../Images/inner/Add.jpg" ID="btnAdd" AlternateText="Add"
                                        runat="server" Style="margin-left: 2px" OnClick="imbAdd_Click" OnClientClick="return ConfirmSelection('aspnetForm','ctl00_ContentPlaceHolder1_grdTickets','Please select the Tickets you want to purchase!')" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="height: 267px">
                        <table style="width: 80%" id="tableCart" runat="server">
                            <tr id="trCartH" runat="server">
                                <td class="form_header">
                                    <span>
                                        <asp:Literal ID="Literal3" runat="server">Your Selected Tickets</asp:Literal></span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span style="color: Red;">
                                        <asp:Label ID="lblCart" runat="server" ForeColor="Red"></asp:Label></span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                
                                    <asp:GridView ID="grdTempCart" GridLines="None" AutoGenerateColumns="false" runat="server"
                                        Width="100%" CellSpacing="3" CellPadding="0" BorderWidth="0" OnRowEditing="grdTempCart_RowEditing"
                                        OnRowCancelingEdit="grdTempCart_RowCancelingEdit" DataKeyNames="TICKET_ID" OnRowDeleting="grdTempCart_RowDeleting"
                                        OnRowUpdating="grdTempCart_RowUpdating">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Event" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                                ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    <%#Eval("EVENT_NAME") %>
                                                </ItemTemplate>
                                                <ItemStyle Width="16%" CssClass="heading-txtnormal" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Ticket Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    <%#Eval("TICKET_TYPE_NAME") %>
                                                    <asp:HiddenField ID="hdnTicketID" runat="server" Value='<%#Eval("TICKET_TYPE_ID") %>' />
                                                    <asp:HiddenField ID="hdnTableType_Id" runat="server" Value='<%#Eval("TABLE_TYPE_ID") %>' />
                                                    <asp:HiddenField ID="hdnTableCapacity" runat="server" Value='<%#Eval("TABLE_CAPACITY") %>' />
                                                </ItemTemplate>
                                                <ItemStyle Width="13%" CssClass="textnormal" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Date and Time" ItemStyle-HorizontalAlign="Center"
                                                HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    <%#Eval("START_DATETIME") %>
                                                    <asp:HiddenField ID="hdnDateID" runat="server" Value='<%#Eval("DATE_ID") %>' />
                                                </ItemTemplate>
                                                <ItemStyle Width="20%" CssClass="textnormal" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Seating Area" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    <%#Eval("SEATING_AREA_NAME") %>
                                                    <asp:HiddenField ID="hdnSeatingID" runat="server" Value='<%#Eval("SEATING_AREA_ID")%>' />
                                                </ItemTemplate>
                                                <ItemStyle Width="16%" CssClass="textnormal" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Price" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    $<%#Eval("PRICE") %>
                                                </ItemTemplate>
                                                <ItemStyle Width="8%" CssClass="textnormal" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    <%#Eval("QUANTITY") %>(<%#Convert.ToBoolean(Eval("TYPE").ToString().ToLower()=="table")?"Group":"Individual" %>)
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtQuantity" CssClass="eventtextbox" onkeypress="return numberKeypress(event);"
                                                        Width="50" runat="server" Text='<%#Eval("QUANTITY") %>'></asp:TextBox>
                                                </EditItemTemplate>
                                                <FooterTemplate>
                                                    <strong>Total: </strong>
                                                </FooterTemplate>
                                                <ItemStyle Width="7%" CssClass="textnormal" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Total Price" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    $<asp:Literal ID="litTotal" runat="server" Text='<%#Eval("TOTAL_PRICE") %>' />
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    $<asp:Literal ID="litTotalPrice" runat="server" />
                                                </FooterTemplate>
                                                <ItemStyle Width="13%" CssClass="textnormal" />
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                                ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="imbEdit" runat="server" ImageUrl="../../images/index/editevent.jpg"
                                                        Width="15" Height="16" AlternateText="Edit" CommandName="edit" />
                                                    <asp:ImageButton ID="imbDelete" runat="server" ImageUrl="../../images/index/delete.jpg"
                                                        Width="14" Height="13" AlternateText="Delete" CommandName="delete" />
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:ImageButton ID="imbSave" runat="server" ImageUrl="../../images/index/update.png"
                                                        Width="15" Height="16" AlternateText="Save" CommandName="update" />
                                                    <asp:ImageButton ID="imbCancel" runat="server" ImageUrl="../../images/index/delete.png"
                                                        Width="14" Height="13" AlternateText="Cancel" CommandName="cancel" />
                                                </EditItemTemplate>
                                                <ItemStyle Width="20%" CssClass="textnormal" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <center>
                                                <strong>No tickets Added Yet.</strong></center>
                                        </EmptyDataTemplate>
                                        <HeaderStyle CssClass="blacktext" />
                                        <RowStyle Height="27" />
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <table style="width: 100%">
                                        <tr>
                                            <td class="form_header" align="left">
                                                <span>Select POS Payment Option</span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                            <asp:checkbox ID="chkIsApplyPosPayment" runat="server" />Apply Transaction Fee <asp:DropDownList ID="ddlPOSPaymentOption" class="listbox" runat="server" onchange="ShowHideCheckOutMethod()"
                                                    OnSelectedIndexChanged="ddlPOSPaymentOption_SelectedIndexChanged">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <table style="width: 100%" id="tblCheckoutMethod" runat="server">
                                        <tr id="tr1" runat="server">
                                            <td class="form_header" align="left">
                                                <span>
                                                    <asp:Literal ID="Literal4" runat="server">Select Checkout Method</asp:Literal></span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span style="color: Red;">
                                                    <asp:Label ID="lblCheckoutMethod" runat="server" ForeColor="Red"></asp:Label></span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                <span id="straightdiv">
                                                    <asp:RadioButton ID="rdoStraight" runat="server" Text="Straight" GroupName="checkoutmethod" />
                                                    &nbsp;&nbsp; </span>
                                                <asp:RadioButton ID="rdoDetailed" runat="server" Text="Detailed" Checked="true" GroupName="checkoutmethod" />
                                                &nbsp;&nbsp;
                                                <asp:RadioButton ID="rdoSimple" runat="server" Text="Simple" GroupName="checkoutmethod" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td height="30" align="right" valign="middle" style="padding-bottom: 3px; padding-left: 2px;">
                                    <asp:ImageButton ID="imbAddToCat" ImageUrl="../../Images/inner/addcartbtn.jpg" runat="server"
                                        Width="99" Height="30" BorderWidth="0" AlternateText="Add To Cart" OnClick="imbAddTocart_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <table id="tblcheckout" runat="server" width="80%" align="left" border="0" cellspacing="0"
                            cellpadding="0">
                            <tr>
                                <td height="15" align="left" valign="bottom">
                                    <asp:Literal ID="litErrorMsg" runat="server"></asp:Literal>
                                    <input type="hidden" id="hdnEventId" runat="server" />
                                    <input type="hidden" id="hdnNoOfTickets" runat="server" />
                                    <input type="hidden" id="hdnBarcodeType" runat="server" />
                                    <input type="hidden" id="hdnBarcodeDrawType" runat="server" />
                                    <input type="hidden" id="hdnEventAge" runat="server" />
                                    <input type="hidden" id="hdnAgentCommission" runat="server" />
                                    <input type="hidden" id="hdnSuperAdminCommission" runat="server" />
                                    <input type="hidden" id="hdnTotalCommission" runat="server" />
                                    <input type="hidden" id="hdnAdditionalSurcharge" runat="server" />
                                    <input type="hidden" id="hdnTotal" runat="server" />
                                    <input type="hidden" id="hdnTransactionFee" runat="server" />
                                    <input type="hidden" id="hdnGrandTotal" runat="server" />
                                    <input type="hidden" id="hdnIsAbsorb" runat="server" />
                                    <input type="hidden" id="hdnActualFee" runat="server" />
                                    <input type="hidden" id="hdnEventAdminAmount" runat="server" />
                                    <input type="hidden" id="hdnClubAdminAmount" runat="server" />
                                    <input type="hidden" id="hdnIsFoodBeverages" runat="server" />
                                    <asp:HiddenField ID="hdnCheckoutMethod" runat="server" Value="detailed" Visible="false" />
                                    <span style="display: none">
                                        <asp:Literal ID="litIsBasicEvent" runat="server"></asp:Literal></span>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" valign="top">
                                    <asp:MultiView ID="mvcheckout" runat="server" Visible="true">
                                        <asp:View ID="vwCompletePayment" runat="server">
                                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                <tr valign="middle">
                                                    <td align="left" class="form_header" height="25px">
                                                        <span>Complete Payment Details </span>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" valign="middle" class="required" style="padding: 5px 10px 5px 10px;">
                                                        <img src="../../Images/inner/requiredfield.gif" width="7" height="7" alt="" />
                                                        Required Fields
                                                    </td>
                                                </tr>
                                                <tr id="trInfo1" runat="server">
                                                    <td align="left" valign="middle" class="blacktextbig" style="padding: 5px 10px 5px 10px;
                                                        border-bottom: 1px solid #cdcdcd;">
                                                        <em>Personal and contact Information</em>
                                                    </td>
                                                </tr>
                                                <tr id="trInfo2" runat="server">
                                                    <td align="left" valign="middle" style="padding: 5px 10px 5px 10px;">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                            <tr>
                                                                <td width="15%" height="32" align="left" valign="middle" class="blacktext">
                                                                    First name
                                                                </td>
                                                                <td width="85%" height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtFirstName" runat="server" class="textbox" MaxLength="50" size="41"></asp:TextBox>
                                                                    <img src="../../Images/inner/requiredfield.gif" width="7" height="7" alt="" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Last name
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtLastName" runat="server" class="textbox" size="41" MaxLength="50"></asp:TextBox>
                                                                    <img src="../../Images/inner/requiredfield.gif" width="7" height="7" alt="" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Email Address
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtEmail" runat="server" class="textbox" size="41" MaxLength="100"></asp:TextBox>
                                                                    <img src="../../Images/inner/requiredfield.gif" width="7" height="7" alt="" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Phone
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtPhone" runat="server" class="textbox" size="41" MaxLength="25"></asp:TextBox>
                                                                    <img src="../../Images/inner/requiredfield.gif" width="7" height="7" alt="" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Mobile
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox MaxLength="25" ID="txtMobile" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Fax
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtFax" MaxLength="25" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Company name
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtCompany" runat="server" class="textbox" MaxLength="100" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    ABN/ACN
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtAbnAcn" runat="server" class="textbox" MaxLength="100" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr id="cmTrBillingInfo" runat="server">
                                                    <td align="left" valign="middle" class="blacktextbig" style="padding: 5px 10px 5px 10px;
                                                        border-bottom: 1px solid #cdcdcd;">
                                                        <em>Billing Information</em>
                                                    </td>
                                                </tr>
                                                <tr id="cmTrInfo" runat="server">
                                                    <td align="left" valign="middle" style="padding: 5px 10px 5px 10px;">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                            <tr>
                                                                <td colspan="2" class="blacktext">
                                                                    Physical Address:
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td width="15%" height="32" align="left" valign="middle" class="blacktext">
                                                                    Address line 1
                                                                </td>
                                                                <td width="85%" height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtAddress1" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Address line 2
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtAddress2" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    City
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtCity" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    State
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:DropDownList ID="ddlBillState" runat="server" CssClass="listbox" Width="235"
                                                                        Style="display: none">
                                                                    </asp:DropDownList>
                                                                    <asp:DropDownList ID="ddlAuStates" runat="server" CssClass="listbox" Width="212">
                                                                        <asp:ListItem Text="Select" Value=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:TextBox ID="txtBillState" runat="server" CssClass="textbox" MaxLength="50" Style="display: none"
                                                                        TabIndex="1" Width="235"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Postal Code
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtPostal" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                    <img src="../../Images/inner/requiredfield.gif" width="7" height="7" alt="" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Country
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:DropDownList onchange="ShowHideState(this,'bill')" ID="ddlCountry" runat="server"
                                                                        CssClass="listbox" Width="212">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" align="center">
                                                                    &nbsp;
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" class="blacktext">
                                                                    <asp:CheckBox ID="chkMailing" runat="server" OnClick="FillAddress(this)" Text="Mailing address same as physical address" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" align="center">
                                                                    &nbsp;
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2" class="blacktext">
                                                                    Mailing Address:
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td width="15%" height="32" align="left" valign="middle" class="blacktext">
                                                                    Address line 1
                                                                </td>
                                                                <td width="85%" height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtAddressMailing1" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Address line 2
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtAddressMailing2" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    City
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtCityMailing" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    State
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:DropDownList ID="ddlBillStateMailing" runat="server" CssClass="listbox" Width="235"
                                                                        Style="display: none">
                                                                    </asp:DropDownList>
                                                                    <asp:DropDownList ID="ddlAuStatesMailing" runat="server" CssClass="listbox" Width="212">
                                                                        <asp:ListItem Text="Select" Value=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:TextBox ID="txtBillStateMailing" runat="server" CssClass="textbox" MaxLength="50"
                                                                        Style="display: none" TabIndex="1" Width="235"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Postal Code
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:TextBox ID="txtPostalMailing" runat="server" class="textbox" size="41"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="32" align="left" valign="middle" class="blacktext">
                                                                    Country
                                                                </td>
                                                                <td height="32" align="left" valign="middle">
                                                                    <asp:DropDownList onchange="ShowHideStateMailing(this,'bill')" ID="ddlCountryMailing"
                                                                        runat="server" CssClass="listbox" Width="212">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" valign="middle" style="padding: 5px 0 0 110px;">
                                                        <asp:ImageButton ID="imbProceed" OnClick="imbProceed_Click" runat="server" AlternateText="Proceed"
                                                            Width="134" Height="30" BorderWidth="0" ImageUrl="../../Images/inner/proceedbtn1.jpg"
                                                            OnClientClick="return ValidateForm()" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:View>
                                        <asp:View ID="vwBilling" runat="server">
                                            <table id="tblInvoice" runat="server" width="100%" border="0" cellspacing="0" cellpadding="0"
                                                align="center">
                                                <tr>
                                                    <td colspan="2">
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <td class="form_header2 form_header" align="left">
                                                                    Tax Invoice
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <style type="text/css">
                                                            .form_header2
                                                            {
                                                                color: #FFFFFF;
                                                                font-family: Tahoma;
                                                                font-size: 14px;
                                                                font-weight: bold;
                                                                padding-left: 3px;
                                                            }
                                                            .inv_totle
                                                            {
                                                                color: #454545;
                                                                font-family: arial;
                                                                font-size: 20px;
                                                                font-weight: bold;
                                                                margin-top: 15px;
                                                                padding: 15px 0 0;
                                                            }
                                                            .inv_txtL
                                                            {
                                                                color: #434343;
                                                                font-family: Arial,Helvetica,sans-serif;
                                                                font-size: 12px;
                                                                font-weight: bold;
                                                            }
                                                            .inv_txtL2
                                                            {
                                                                font-family: Arial, Helvetica, sans-serif;
                                                                font-size: 12px;
                                                                color: #000000;
                                                                font-weight: bold;
                                                            }
                                                            .ln-ht
                                                            {
                                                                line-height: 20px;
                                                            }
                                                            .inv_col
                                                            {
                                                                font-family: Arial, Helvetica, sans-serif;
                                                                font-size: 13px;
                                                                color: #000000;
                                                                line-height: 25px;
                                                                font-weight: bold;
                                                                border-top: 2px solid #000000;
                                                                border-bottom: 2px solid #000000;
                                                            }
                                                            .inv_col2
                                                            {
                                                                font-family: Arial, Helvetica, sans-serif;
                                                                font-size: 15px;
                                                                color: #000000;
                                                                line-height: 25px;
                                                                font-weight: bold;
                                                            }
                                                            .inv_deposit
                                                            {
                                                                font-family: Arial, Helvetica, sans-serif;
                                                                font-size: 12px;
                                                                color: #000000;
                                                                line-height: 25px;
                                                                font-weight: bold;
                                                            }
                                                            .inv_deposit a
                                                            {
                                                                color: #000000;
                                                                text-decoration: none;
                                                            }
                                                        </style>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="50%" class="inv_totle">
                                                        Tax Invoice
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="inv_txtL">
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="inv_txtL ln-ht">
                                                        <asp:Literal ID="litUserName" runat="server" Text=""></asp:Literal>
                                                        <asp:Literal ID="litUserName2" runat="server" Text="" Visible="false"></asp:Literal>
                                                        <br />
                                                        <asp:Literal ID="litAddress" runat="server" Text=""></asp:Literal>
                                                    </td>
                                                    <td valign="top">
                                                        <table width="90%" border="0" cellspacing="2" cellpadding="2" align="right">
                                                            <tr>
                                                                <td class="inv_txtL ln-ht" align="right">
                                                                    ACCOUNT:
                                                                </td>
                                                                <td class="inv_txtL2">
                                                                    <asp:Literal ID="litUserName3" runat="server"></asp:Literal>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="inv_txtL  ln-ht" align="right">
                                                                    INVOICE NUMBER:
                                                                </td>
                                                                <td class="inv_txtL2">
                                                                    <asp:Literal ID="litInvoiceNumber" runat="server"></asp:Literal>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="inv_txtL  ln-ht" align="right">
                                                                    DATE OF INVOICE:
                                                                </td>
                                                                <td class="inv_txtL2">
                                                                    <asp:Literal ID="litDateInvoice" runat="server"></asp:Literal>
                                                                    <asp:Literal ID="litDate" runat="server"></asp:Literal>
                                                                </td>
                                                            </tr>
                                                            <%--<tr>
                                                            <td class="inv_txtL" valign="top">
                                                                PAGE:
                                                            </td>
                                                            <td class="inv_txtL2" valign="top">
                                                                1
                                                            </td>
                                                        </tr>--%>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="50" colspan="2" class="inv_txtL2" style="font-size: 14px;">
                                                        <img src="http://bcec.net.au/images/inner/logo_2.png" alt="" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="50" colspan="2" class="inv_txtL2" style="font-size: 14px;">
                                                        DETAILS:FINAL INVOICE / STATEMENT
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <table width="100%" border="0" cellspacing="1" cellpadding="1">
                                                            <tr>
                                                                <td colspan="4">
                                                                    <asp:Literal ID="ltrStart" runat="server"></asp:Literal>
                                                                    <asp:DataList Width="100%" RepeatLayout="Table" BorderWidth="0" CellSpacing="1" CellPadding="0"
                                                                        ID="dlCart" BackColor="#c0c0c0" runat="server" AutoGenerateColumns="false" DataKeyNames="TICKET_ID"
                                                                        GridLines="None">
                                                                        <FooterStyle BackColor="#E9ECEF" />
                                                                        <HeaderTemplate>
                                                                            <table width="100%" border="0" cellpadding="1" cellspacing="2">
                                                                                <tr>
                                                                                    <td width="15%" align="center" style="background-color: #eeeeee; font-weight: bold;
                                                                                        color: Black;">
                                                                                        Event
                                                                                    </td>
                                                                                    <td width="12%" align="center" style="background-color: #eeeeee; font-weight: bold;
                                                                                        color: Black;">
                                                                                        Ticket Type
                                                                                    </td>
                                                                                    <td width="12%" align="center" style="background-color: #eeeeee; font-weight: bold;
                                                                                        color: Black;">
                                                                                        Type
                                                                                    </td>
                                                                                    <td width="16%" align="center" style="background-color: #eeeeee; font-weight: bold;
                                                                                        color: Black;">
                                                                                        Seating Area
                                                                                    </td>
                                                                                    <td width="19%" align="center" style="background-color: #eeeeee; font-weight: bold;
                                                                                        color: Black;">
                                                                                        Date and Time
                                                                                    </td>
                                                                                    <td width="8%" align="center" style="background-color: #eeeeee; font-weight: bold;
                                                                                        color: Black;">
                                                                                        Qty
                                                                                    </td>
                                                                                    <td width="8%" align="center" style="background-color: #eeeeee; font-weight: bold;
                                                                                        color: Black;">
                                                                                        Price
                                                                                    </td>
                                                                                    <td width="8%" align="center" style="background-color: #eeeeee; font-weight: bold;
                                                                                        color: Black;">
                                                                                        Total
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </HeaderTemplate>
                                                                        <HeaderStyle BackColor="#FFFFFF" />
                                                                        <ItemTemplate>
                                                                            <table width="100%" border="0" cellpadding="1" cellspacing="2">
                                                                                <tr>
                                                                                    <td width="15%" align="center" valign="middle" style="height: 30px; background-color: #FFFFFF;
                                                                                        font-size: 11px; color: Black;">
                                                                                        <b>
                                                                                            <%#Eval("EVENT_NAME") %></b>
                                                                                    </td>
                                                                                    <td width="12%" align="center" style="background-color: #FFFFFF; font-size: 11px;
                                                                                        color: Black;">
                                                                                        <%#Eval("TICKET_TYPE_NAME") %>
                                                                                        <asp:Literal ID="litTicketTypeID" runat="server" Visible="false" Text='<%#Eval("TICKET_TYPE_ID") %>'></asp:Literal>
                                                                                    </td>
                                                                                    <td width="12%" align="center" style="background-color: #FFFFFF; font-size: 11px;
                                                                                        color: Black;">
                                                                                        <asp:Literal ID="litType" runat="server" Text='<%#Convert.ToString(Eval("TYPE")).ToLower()=="table"?"Group":"Individual"%>'></asp:Literal>
                                                                                    </td>
                                                                                    <td width="16%" align="center" style="background-color: #FFFFFF; font-size: 11px;
                                                                                        color: Black;">
                                                                                        <asp:Literal ID="litSeatingAreaID" Visible="false" runat="server" Text='<%#Eval("SEATING_AREA_ID") %>'></asp:Literal>
                                                                                        <%#Eval("SEATING_AREA_NAME") %>
                                                                                    </td>
                                                                                    <td width="19%" align="center" style="background-color: #FFFFFF; font-size: 11px;
                                                                                        color: Black;">
                                                                                        <asp:Literal ID="litDateID" runat="server" Text='<%#Eval("DATE_ID") %>'></asp:Literal>
                                                                                        <%#Eval("START_DATETIME") %>
                                                                                    </td>
                                                                                    <td width="8%" align="center" style="background-color: #FFFFFF; font-size: 11px;
                                                                                        color: Black;">
                                                                                        <asp:Literal ID="litQty" runat="server" Text='<%#Eval("QUANTITY") %>'></asp:Literal>
                                                                                    </td>
                                                                                    <td width="8%" align="center" style="background-color: #FFFFFF; font-size: 11px;
                                                                                        color: Black;">
                                                                                        $<asp:Literal ID="litPrice" runat="server" Text='<%#Eval("PRICE") %>'></asp:Literal>
                                                                                    </td>
                                                                                    <td width="8%" align="center" style="background-color: #FFFFFF; font-size: 11px;
                                                                                        color: Black;">
                                                                                        $<asp:Literal ID="litTotalPrice" runat="server" Text='<%#Eval("TOTAL_PRICE") %>'></asp:Literal>
                                                                                        <%-- <asp:HiddenField ID="hdnTableCapacity" Value='<%#Eval("TABLE_CAPACITY") %>' runat="server" />
                                                                                <asp:HiddenField ID="hdnTableTypeID" Value='<%#Eval("TABLE_TYPE_ID") %>' runat="server" />
                                                                                <asp:HiddenField ID="hdnTotal_Discount_Percent" Value='<%#Eval("TOTAL_DISCOUNT_PERCENT") %>'
                                                                                    runat="server" />--%>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </ItemTemplate>
                                                                    </asp:DataList>
                                                                    <asp:Literal ID="ltrEnd" runat="server"></asp:Literal>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="3">
                                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                                        <tr>
                                                                            <td width="19%" class="inv_deposit">
                                                                                <asp:Literal ID="litDirectDeposit" runat="server" Text="DirectDeposit"></asp:Literal>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                                <td>
                                                                    &nbsp;
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="4" align="right">
                                                                    <table width="100%" border="0" cellpadding="5" cellspacing="2">
                                                                        <tr>
                                                                            <td width="95%" style="font-weight: bold" align="right">
                                                                                Ticket Cost:
                                                                            </td>
                                                                            <td width="5%" align="right">
                                                                                <asp:Literal ID="litTotalCost" runat="server"></asp:Literal>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right" style="font-weight: bold">
                                                                                GST Included in Ticket Cost:
                                                                            </td>
                                                                            <td align="right">
                                                                                <asp:Literal ID="litIncludeGst" runat="server"></asp:Literal>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trSurcharge" runat="server">
                                                                            <td align="right" style="font-weight: bold">
                                                                                Surcharge (GST Inclusive):
                                                                            </td>
                                                                            <td align="right">
                                                                                &nbsp;$<asp:Literal ID="LitSurcharge" runat="server" Text="0.00"></asp:Literal>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trPaypal" runat="server">
                                                                            <td align="right" style="font-weight: bold">
                                                                             E-merchant transaction fee (GST Inclusive):
                                                                                <%--POS Charges (GST Inclusive):--%>
                                                                            </td>
                                                                            <td align="right">
                                                                                &nbsp;$<asp:Literal runat="server" ID="LitPaypalFee" Text="0.00"></asp:Literal>
                                                                            </td>
                                                                        </tr>
                                                                        <tr id="trTotal" runat="server">
                                                                            <td align="right" style="font-weight: bold">
                                                                                Grand Total:
                                                                            </td>
                                                                            <td align="right">
                                                                                &nbsp;$<asp:Literal ID="litTotal" runat="server" Text="0.00"></asp:Literal>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td width="95%" class="inv_col" colspan="3">
                                                                    INVOICE TOTAL
                                                                </td>
                                                                <td width="5%" class="inv_col">
                                                                   $<asp:Literal ID="litTotal1" runat="server" Text="0.00"></asp:Literal>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="30" colspan="2" class="inv_txtL">
                                                        Please Quote Invoice Details on Payment
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td align="left" valign="top" height="20px">
                                                        <table width="100%" border="0" align="left" cellpadding="0" cellspacing="0">
                                                            <tr id="cmTrAttendee" runat="server">
                                                                <td align="left" valign="top">
                                                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                                        <tr id="cmTrChkAttendee" runat="server">
                                                                            <td height="10" align="left" valign="top" class="textnormal">
                                                                                <asp:UpdatePanel ID="updpnlchkAtendeeList" runat="server">
                                                                                    <ContentTemplate>
                                                                                        <asp:CheckBox ID="chkAttendeeListLater" Checked="false" AutoPostBack="true" runat="server"
                                                                                            OnCheckedChanged="chkAttendeeListLater_CheckedChanged" />
                                                                                        &nbsp;&nbsp; Provide patron names at a later time.
                                                                                    </ContentTemplate>
                                                                                </asp:UpdatePanel>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td height="10" align="left" valign="top" class="textnormal">
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left" valign="top">
                                                                                <asp:UpdatePanel ID="updpnltd" runat="server">
                                                                                    <ContentTemplate>
                                                                                        <table width="100%" id="tblAttendeeOption" runat="server" border="0" cellspacing="0"
                                                                                            cellpadding="0">
                                                                                            <tr>
                                                                                                <td bgcolor="#c9c9c9">
                                                                                                    <table width="100%" border="0" align="center" cellpadding="0" cellspacing="1">
                                                                                                        <tr>
                                                                                                            <td bgcolor="#FFFFFF">
                                                                                                                <table width="100%" border="0" align="center" cellpadding="0" cellspacing="1">
                                                                                                                    <tr>
                                                                                                                        <td>
                                                                                                                            <table width="100%" border="0" align="center" cellpadding="5" cellspacing="0">
                                                                                                                                <tr>
                                                                                                                                    <td class="gray-bg">
                                                                                                                                        <strong>Attendee Options</strong>
                                                                                                                                    </td>
                                                                                                                                </tr>
                                                                                                                            </table>
                                                                                                                        </td>
                                                                                                                    </tr>
                                                                                                                </table>
                                                                                                            </td>
                                                                                                        </tr>
                                                                                                    </table>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="border-bottom: 1px solid #000;">
                                                                                                    &nbsp;
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="padding: 10px;">
                                                                                                    <asp:UpdatePanel ID="upAttendee" runat="server">
                                                                                                        <ContentTemplate>
                                                                                                            <asp:DataList ID="dlistAttendee" OnItemDataBound="dlistAttendee_ItemDataBound" OnItemCommand="dlistAttendee_ItemCommand"
                                                                                                                RepeatColumns="1" BorderWidth="0" CellPadding="0" CellSpacing="0" runat="server"
                                                                                                                DataKeyField="TICKET_ID">
                                                                                                                <ItemTemplate>
                                                                                                                    <table width="100%" border="0" cellspacing="2" cellpadding="2">
                                                                                                                        <tr>
                                                                                                                            <td align="left" valign="middle" class="blackbigtext" height="30">
                                                                                                                                <%#Eval("EVENT_NAME") %>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height="20" align="left" valign="middle" class="textnormal">
                                                                                                                                <%#Eval("TICKET_TYPE_NAME") %>
                                                                                                                                <asp:Literal ID="litTicketTypeID" runat="server" Text='<%#Eval("TICKET_TYPE_ID") %>'
                                                                                                                                    Visible="false"></asp:Literal>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td height="20" align="left" valign="middle" class="textnormal">
                                                                                                                                <asp:Literal ID="litDateID" runat="server" Visible="false" Text='<%#Eval("DATE_ID") %>'></asp:Literal>
                                                                                                                                <%#Eval("START_DATETIME") %>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td align="left" valign="middle">
                                                                                                                                <table width="100%" border="0" cellspacing="2" cellpadding="0">
                                                                                                                                    <tr class="blacktext">
                                                                                                                                        <td width="15%" align="left" valign="middle">
                                                                                                                                            First name<span>*</span>
                                                                                                                                        </td>
                                                                                                                                        <td width="15%" align="left" valign="middle">
                                                                                                                                            Last name<span>*</span>
                                                                                                                                        </td>
                                                                                                                                        <td width="15%" align="left" valign="middle">
                                                                                                                                            Company
                                                                                                                                        </td>
                                                                                                                                        <td width="25%" align="center" valign="middle" runat="server" id="tdDietHead">
                                                                                                                                            Dietary requirements
                                                                                                                                        </td>
                                                                                                                                        <td width="15%" align="left" valign="middle">
                                                                                                                                            Other Requirement
                                                                                                                                        </td>
                                                                                                                                        <td width="15%" align="left" valign="middle">
                                                                                                                                            Seating Preference
                                                                                                                                        </td>
                                                                                                                                    </tr>
                                                                                                                                    <tr>
                                                                                                                                        <td align="left" valign="middle" style="">
                                                                                                                                            <asp:TextBox ID="txtFirstName" MaxLength="80" CssClass="textbox2" runat="server"></asp:TextBox>
                                                                                                                                        </td>
                                                                                                                                        <td align="left" valign="middle" style="padding-left: 10px;">
                                                                                                                                            <asp:TextBox ID="txtLastName" MaxLength="80" CssClass="textbox2" runat="server"></asp:TextBox>
                                                                                                                                        </td>
                                                                                                                                        <td align="left" valign="middle" style="padding-left: 10px;">
                                                                                                                                            <asp:TextBox ID="txtCompany" MaxLength="80" CssClass="textbox2" runat="server"></asp:TextBox>
                                                                                                                                        </td>
                                                                                                                                        <td align="center" valign="middle" class="bluetext2" style="padding-left: 10px;"
                                                                                                                                            runat="server" id="tdDiet">
                                                                                                                                            <asp:LinkButton ID="lnkShowDietary" runat="server" Text="Click here to set dietary requirement"></asp:LinkButton>
                                                                                                                                            <asp:CheckBoxList ID="chkDietary" runat="server" Visible="false">
                                                                                                                                            </asp:CheckBoxList>
                                                                                                                                        </td>
                                                                                                                                        <td align="left" valign="middle" style="padding-top: 10px; padding-left: 10px;">
                                                                                                                                            <asp:TextBox ID="txtOtherRequirements" MaxLength="255" CssClass="textbox2" runat="server"></asp:TextBox>
                                                                                                                                        </td>
                                                                                                                                        <td align="left" valign="middle" style="padding-top: 10px; padding-left: 10px;">
                                                                                                                                            <asp:TextBox ID="txtSeatingPreference" MaxLength="100" CssClass="textbox2" runat="server"></asp:TextBox>
                                                                                                                                        </td>
                                                                                                                                    </tr>
                                                                                                                                </table>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td>
                                                                                                                                <asp:Literal ID="litFNBMsg" runat="server"></asp:Literal>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td>
                                                                                                                                &nbsp;
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                </ItemTemplate>
                                                                                                            </asp:DataList>
                                                                                                        </ContentTemplate>
                                                                                                    </asp:UpdatePanel>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr style="display: none">
                                                                                                <td>
                                                                                                    <asp:GridView Width="100%" RepeatLayout="Table" BorderWidth="0" CellSpacing="1" CellPadding="0"
                                                                                                        ID="grdCart" BackColor="#c0c0c0" runat="server" AutoGenerateColumns="false" DataKeyNames="TICKET_ID"
                                                                                                        GridLines="None">
                                                                                                        <Columns>
                                                                                                            <asp:TemplateField HeaderText="Event" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Middle">
                                                                                                                <ItemTemplate>
                                                                                                                    <b>
                                                                                                                        <%#Eval("EVENT_NAME") %></b>
                                                                                                                </ItemTemplate>
                                                                                                                <HeaderStyle BackColor="#eeeeee" />
                                                                                                                <ItemStyle Width="15%" BackColor="#FFFFFF" />
                                                                                                            </asp:TemplateField>
                                                                                                            <asp:TemplateField HeaderText="Ticket Type" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                                                                                <ItemTemplate>
                                                                                                                    <%#Eval("TICKET_TYPE_NAME") %>
                                                                                                                    <asp:Literal ID="litTicketTypeID" runat="server" Visible="false" Text='<%#Eval("TICKET_TYPE_ID") %>'></asp:Literal>
                                                                                                                </ItemTemplate>
                                                                                                                <HeaderStyle BackColor="#eeeeee" />
                                                                                                                <ItemStyle Width="12%" CssClass="textnormal" BackColor="#FFFFFF" />
                                                                                                            </asp:TemplateField>
                                                                                                            <asp:TemplateField HeaderText="Type" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                                                                                <ItemTemplate>
                                                                                                                    <asp:Literal ID="litType" runat="server" Text='<%#Convert.ToString(Eval("TYPE")).ToLower()=="table"?"Group":"Individual"%>'></asp:Literal>
                                                                                                                </ItemTemplate>
                                                                                                                <HeaderStyle BackColor="#eeeeee" />
                                                                                                                <ItemStyle Width="12%" CssClass="textnormal" BackColor="#FFFFFF" />
                                                                                                            </asp:TemplateField>
                                                                                                            <asp:TemplateField HeaderText="Seating Area" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                                                                                <ItemTemplate>
                                                                                                                    <asp:Literal ID="litSeatingAreaID" Visible="false" runat="server" Text='<%#Eval("SEATING_AREA_ID") %>'></asp:Literal>
                                                                                                                    <%#Eval("SEATING_AREA_NAME") %>
                                                                                                                </ItemTemplate>
                                                                                                                <HeaderStyle BackColor="#eeeeee" />
                                                                                                                <ItemStyle Width="16%" CssClass="textnormal" BackColor="#FFFFFF" />
                                                                                                            </asp:TemplateField>
                                                                                                            <asp:TemplateField HeaderText="Date and Time" ItemStyle-HorizontalAlign="Center"
                                                                                                                ItemStyle-VerticalAlign="Middle">
                                                                                                                <ItemTemplate>
                                                                                                                    <asp:Literal ID="litDateID" runat="server" Text='<%#Eval("DATE_ID") %>'></asp:Literal>
                                                                                                                    <%#Eval("START_DATETIME") %>
                                                                                                                </ItemTemplate>
                                                                                                                <HeaderStyle BackColor="#eeeeee" />
                                                                                                                <ItemStyle Width="19%" CssClass="textnormal" BackColor="#FFFFFF" />
                                                                                                            </asp:TemplateField>
                                                                                                            <asp:TemplateField HeaderText="Qty" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                                                                                <ItemTemplate>
                                                                                                                    <asp:Literal ID="litQty" runat="server" Text='<%#Eval("QUANTITY") %>'></asp:Literal>
                                                                                                                </ItemTemplate>
                                                                                                                <HeaderStyle BackColor="#eeeeee" />
                                                                                                                <ItemStyle Width="8%" CssClass="textnormal" BackColor="#FFFFFF" />
                                                                                                            </asp:TemplateField>
                                                                                                            <asp:TemplateField HeaderText="Price" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                                                                                <ItemTemplate>
                                                                                                                    $<asp:Literal ID="litPrice" runat="server" Text='<%#Eval("PRICE") %>'></asp:Literal>
                                                                                                                </ItemTemplate>
                                                                                                                <HeaderStyle BackColor="#eeeeee" />
                                                                                                                <ItemStyle Width="8%" BackColor="#FFFFFF" CssClass="textnormal" />
                                                                                                            </asp:TemplateField>
                                                                                                            <asp:TemplateField HeaderText="Total" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                                                                                <ItemTemplate>
                                                                                                                    $<asp:Literal ID="litTotalPrice" runat="server" Text='<%#Eval("TOTAL_PRICE") %>'></asp:Literal>
                                                                                                                    <asp:HiddenField ID="hdnTableCapacity" Value='<%#Eval("TABLE_CAPACITY") %>' runat="server" />
                                                                                                                    <asp:HiddenField ID="hdnTableTypeID" Value='<%#Eval("TABLE_TYPE_ID") %>' runat="server" />
                                                                                                                    <asp:HiddenField ID="hdnTotal_Discount_Percent" Value='<%#Eval("TOTAL_DISCOUNT_PERCENT") %>'
                                                                                                                        runat="server" />
                                                                                                                </ItemTemplate>
                                                                                                                <HeaderStyle BackColor="#eeeeee" />
                                                                                                                <ItemStyle Width="10%" Height="35" BackColor="#FFFFFF" CssClass="textnormal" />
                                                                                                            </asp:TemplateField>
                                                                                                        </Columns>
                                                                                                        <EmptyDataTemplate>
                                                                                                            <center>
                                                                                                                <strong>Shopping Cart Empty</strong></center>
                                                                                                        </EmptyDataTemplate>
                                                                                                        <HeaderStyle CssClass="blacktext" Height="23" HorizontalAlign="Center" VerticalAlign="Middle"
                                                                                                            BackColor="#eeeeee" />
                                                                                                    </asp:GridView>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr id="trBeveragesMessage" runat="server" visible="false">
                                                                                                <td>
                                                                                                    <asp:Literal ID="litBeverageMessage" runat="server"></asp:Literal>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="border-top: 1px solid #000;">
                                                                                                    &nbsp;
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </ContentTemplate>
                                                                                </asp:UpdatePanel>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="lightgreentext" align="left">
                                                                    <b>Comments:</b>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" class="lightgreentext">
                                                                    <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" Height="62px" Width="812px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    POS Code<span class="star">*</span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="txtPOS"
                                                                        MaxLength="50" CssClass="textbox2" runat="server"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <%-- <tr>
                                                            <td>
                                                                Payment Mode:<asp:RadioButton ID="rdbCash" runat="server" Checked="true" GroupName="payment"
                                                                    Text="Cash" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="rdbCredit" runat="server"
                                                                        GroupName="payment" Text="Credit Card" />
                                                            </td>
                                                        </tr>--%>
                                                            <tr>
                                                                <td align="left" valign="top" class="textnormal">
                                                                    <table width="45%" border="0" align="center" cellpadding="0" cellspacing="0">
                                                                        <tr>
                                                                            <td width="62%" align="left" valign="top">
                                                                                <asp:ImageButton ID="imbeditBilling" ImageUrl="../../images/inner/editbillingbtn.jpg"
                                                                                    OnClick="imbeditBilling_Click" AlternateText="Edit billing Information" runat="server"
                                                                                    Width="159" Height="30" BorderWidth="0" />
                                                                            </td>
                                                                            <td width="38%" align="left" valign="top">
                                                                                <asp:ImageButton ID="imbConfirm" runat="server" ImageUrl="../../images/inner/confirmbtn.jpg"
                                                                                    AlternateText="Confirm" Width="82" Height="30" BorderWidth="0" OnClick="imbConfirm_Click"
                                                                                    OnClientClick="return CheckPOS();" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" valign="top" class="textnormal">
                                                                    &nbsp;
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:View>
                                    </asp:MultiView>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" valign="top" align="right" class="copyrights">
                                    (<span class="star">*</span>) indicates required fields.
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div id="pnlPopup" class="PrProgress" style="display: none;">
                <div id="innerPopup" class="PrContainer">
                    <div class="PrHeader">
                        Processing, please wait...</div>
                    <div class="PrBody">
                        <img width="220px" height="19px" src="images/activity.gif" alt="loading..." />
                    </div>
                </div>
            </div>

            <script type="text/javascript">
                Sys.Application.add_load(applicationLoadHandler);
                Sys.Application.add_unload(applicationUnloadHandler);
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
                Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
            </script>

        </div>
        </form>
    </center>
</asp:Content>
