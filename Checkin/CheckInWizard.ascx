<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckInWizard.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Checkin.CheckInWizard" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<input type="hidden" name="ihTimeoutError" id="ihTimeoutError" value="<%= AsyncTimeoutErrorMessageSetting %>" />
<input type="hidden" name="ihFamilyRegistrationPage" id="ihFamilyRegistrationPage" value="<%= FamilyRegistrationPageSetting %>" />
<input type="hidden" name="ihKioskManagementPage" id="ihKioskManagementPage" value="<%= _KioskManagementPageSetting %>" />

<script type="text/javascript">
    var autoConfirmCancel = '<%= AutoCancelConfirmSetting %>' == 'true';
    var longInterval = parseInt('<%= LongRefreshTimeSetting %>');
    var shortInterval = parseInt('<%= ShortRefreshTimeSetting %>');
    var interval;

    var startTime;
    var rightNow;
    var refreshSeconds = 0;
    var secondsSinceLastRefresh = 0;
    var state;
    var shouldRefresh = true;

    var StartTime;
    var NowTime;

    var txtPhone;
    var searchButton;
    var messageLabel;
    var maxDigits = parseInt('<%= PhoneLengthMaxSetting %>');

    function setupCountdown() {
        var lblTimer = $get('<%= lblTimeRemaining.ClientID %>');

        if (lblTimer) {
            StartTime = $get('<%= ihStartTime.ClientID %>').value;
            NowTime = $get('<%= ihNowTime.ClientID %>').value;

            if ($get('CountDown').innerHTML == '') {
                StartTimer('<%= CurrentPortalPage.PortalPageID %>');
            }
        }
    }

    $(function () {
        if (window.Touch) {
            var meta = $('<meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=no;" />');
            var js = $('<script type="text/javascript" src="UserControls/Custom/Cccev/CheckIn/misc/ios.js"></' + 'script>');
            var css = $('<link type="text/css" rel="stylesheet" href="UserControls/Custom/Cccev/CheckIn/misc/ios.css" />');
            var appCapable = $('<meta name="apple-mobile-web-app-capable" content="yes" />');
            var icon = $('<link rel="apple-touch-icon" href="usercontrols/custom/cccev/checkin/images/retina-icon.png" />');
            var status = $('<meta name="apple-mobile-web-app-status-bar-style" content="black" />');

            $('head').append(meta)
                .append(js)
                .append(css)
                .append(appCapable)
                .append(icon)
                .append(status);
        }
    });
</script>

<asp:UpdatePanel ID="upCheckin" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <div class="container" onclick="resetTimer()" onkeypress="resetTimer()">
            <asp:Panel ID="pnlError" runat="server" Visible="false">
                <asp:Label ID="lblError" runat="server" CssClass="errorText" />
            </asp:Panel>
                  
            <!-- Begin Views -->
            
            <asp:Panel ID="pnlInit" runat="server" DefaultButton="btnScan" CssClass="initView view">
            <!-- Init State -->
                <div id="divPanicButton" onclick="reloadBrowser();"></div>
                <asp:Panel ID="pnlSwipeCard" runat="server" CssClass="footer">
                    <div id="divLeftFooter" runat="server" class="footerLeft">
                        <div id="divScanBox" runat="server" visible="false" class="divScanBox">
                            <asp:textbox id="tbScan" tabIndex="0" runat="server" Font-Size="1pt" BackColor="#222222" ForeColor="#222222" BorderStyle="None" MaxLength="12" Width="1pt" />
                            <asp:Button ID="btnScan" runat="server" OnClick="btnScan_Click" Width="1" />
                        </div>
                        <div id="divScanNow" runat="server" visible="false" class="divScanNow">
	                        <asp:Label ID="lblScanNow" runat="server" />
	                    </div>
	                </div>
	                <div id="divRightFooter" runat="server" class="footerRight">
                        <asp:Button id="btnSearchByPhone" CssClass="searchButton" runat="server" Visible="false" Text="Search By Phone" 
                            OnClick="btnSearchByPhone_Click" OnClientClick="disableButton(this);" />
                    </div>
                    <div id="divWideFooter" runat="server" visible="false" class="footerWide">
                        <p><asp:Label ID="lblWideFooter" runat="server" /></p>
                    </div>
                    <div id="divTimer" runat="server" visible="false" class="footerWide">
                            <asp:Label id="lblTimeRemaining" Runat="server" CssClass="footerText countdown" />
                        </div>
                </asp:Panel>
            </asp:Panel>
            
            
            <asp:Panel ID="pnlFamilySearch" runat="server" DefaultButton="btnFamilySearch" CssClass="view">
            <!-- Family Search State -->
                <asp:Panel ID="pnlPhoneSearch" runat="server" CssClass="container">
                    <div class="phonePanel">
                        <div class="heading">
                            <h3 class="checkinText">Enter Phone #</h3>
                        </div>
	                    <table class="phone-search">
		                    <tr>
			                    <td>
				                    <table id="keypad">
					                    <tr>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '1' )" type="button" value="1" /></td>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '2' )" type="button" value="2" /></td>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '3' )" type="button" value="3" /></td>
					                    </tr>
					                    <tr>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '4' )" type="button" value="4" /></td>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '5' )" type="button" value="5" /></td>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '6' )" type="button" value="6" /></td>
					                    </tr>
					                    <tr>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '7' )" type="button" value="7" /></td>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '8' )" type="button" value="8" /></td>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '9' )" type="button" value="9" /></td>
					                    </tr>
					                    <tr>
						                    <td><input class="phoneButton" onclick="return ClearDigit( )" type="button" value="&lt;"></td>
						                    <td><input class="phoneButton" onclick="return ClickDigit( '0' )" type="button" value="0" /></td>
						                    <td><input class="phoneButton" onclick="return ClearAll()" type="button" value="clr" /></td>
					                    </tr>
				                    </table>
			                    </td>
			                    <td class="search">
				                    <div class="family-search-controls">
                                        <div class="controls">
				                            <div id="phone-textbox" onkeypress="javascript:return FireDefaultButton(event,'btnFamilySearch')">
				                                <asp:textbox id="txtPhone" runat="server" CssClass="phoneText" Width="334" Height="75" MaxLength="10" />
				                            </div>
				                            <div id="phone-search-button">
				                                <asp:button id="btnFamilySearch" runat="server" CssClass="dataButton" Text="Search" OnClick="btnFamilySearch_Click" OnClientClick="disableButton(this);" />
				                            </div>
				                        </div>
				                        <br />
					                    <asp:Label id="lblMessage" runat="server" CssClass="checkinCaption" />
                                    </div>
				                    <div class="scrollArea scroll-pane" id="ScrollArea">
				                        <asp:datalist CssClass="centeredList" GridLines="None" id="dgFamilies" runat="server" RepeatColumns="1" CellSpacing="5" DataKeyField="FamilyID" 
				                            OnSelectedIndexChanged="dgFamilies_SelectedIndexChanged" OnItemDataBound="dgFamilies_ItemDataBound">
						                    <ItemTemplate>
							                    <asp:Button runat="server" ID="FamilyID" CommandName="Select" CausesValidation="false" CssClass="nameButton" />
						                    </ItemTemplate>
					                    </asp:datalist>
					                </div>
			                    </td>
		                    </tr>
	                    </table>
                    </div>
                    <div class="footer">
	                    <div class="footerLeft">
		                        <asp:button id="btnFamilySearchCancel" CssClass="cancelButton" runat="server" Text="Cancel" OnClick="Cancel_Click" OnClientClick="disableButton(this);" />
	                        </div>
                        </div>
                </asp:Panel>
                <script type="text/javascript">
                    txtPhone = document.getElementById( '<%= txtPhone.ClientID %>' );
                    txtPhone.focus();
                </script>
            </asp:Panel>

            
            <asp:Panel ID="pnlSelectFamilyMember" runat="server" CssClass="view">
            <!-- Select Family Member State -->
                <div class="content">
                    <input type="hidden" id="ihAttendeeIDs" runat="server" />
                    <div class="heading">
                        <p class="checkinCaption"><asp:Label ID="lblFamilyName" runat="server" /></p>
                        <p class="checkinText">Select All People Attending Today</p>
                    </div>
                    <div class="resultScrollArea scroll-pane">
                        <!-- #349 --><asp:DataList id="dgFamilyMembers" runat="server" DataKeyField="PersonID" CssClass="centeredList"
		                    CellSpacing="5" RepeatColumns="1" BorderColor="Black" BorderWidth="0" Width="353"
		                    OnItemDataBound="dgFamilyMembers_ItemDataBound" ItemStyle-HorizontalAlign="Left" GridLines="None">
                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
		                    <ItemTemplate>
		                        <div class="family-member">
		                            <div class="family-member-button">
			                            <asp:Button runat="server" ID="btnPerson" Text='<%# DataBinder.Eval(Container, "DataItem.NickName") %>' 
			                                CommandArgument='<%# DataBinder.Eval(Container, "DataItem.PersonID") %>' CausesValidation="false"  CssClass="dataButton" />
			                            <input type="hidden" id="ihPersonID" runat="server" value='<%# DataBinder.Eval(Container, "DataItem.PersonID") %>' />
			                        </div>
			                        <div class="family-member-checkbox"><asp:ImageButton id="imgChecked" runat="server" CssClass="dataStar" ImageUrl="images/empty_checkbox.png" /></div>
			                    </div>
		                    </ItemTemplate>
	                    </asp:DataList>
                    </div>
                </div>

                <div class="footer">
                    <div class="footerLeft">
                        <asp:Button ID="btnSelectFamilyMemberCancel" runat="server" Text="Cancel" 
		                    CssClass="cancelButton" onclick="Cancel_Click" OnClientClick="disableButton(this);" />
		            </div>
		            <div class="footerRight">
                        <asp:Button ID="btnSelectFamilyMemberContinue" runat="server" Text="Next" CssClass="nextButton" 
                            onclick="btnSelectFamilyMemberContinue_Click" OnClientClick="disableButton(this);" />
		            </div>
                </div>
            </asp:Panel>
            
            
            <asp:Panel ID="pnlNoEligiblePeople" runat="server">
            <!-- No Young Children State -->
                <asp:Label ID="lblNoEligiblePeople" runat="server" CssClass="errorText" Visible="false" />
                <div class="footer">
	                <div class="footerLeft">
		                <asp:button id="btnNoPeopleCancel" CssClass="cancelButton" runat="server" Text="Cancel" OnClick="Cancel_Click" OnClientClick="disableButton(this);" />
	                </div>
                </div>
            </asp:Panel>

            
            <asp:Panel ID="pnlSelectAbility" runat="server" CssClass="view">
            <!-- Select Ability State -->
                <div class="content">
                    <div class="heading">
	                    <h3 class="checkinText"><asp:Label ID="lblPersonName" runat="server" /></h3>
	                </div>
	                <asp:DataList CssClass="centeredList" GridLines="None" id="dgAbilities" runat="server" DataKeyField="LookupID" 
		                CellSpacing="5" RepeatColumns="1" BorderColor="Black" 
		                onselectedindexchanged="dgAbilities_SelectedIndexChanged" ItemStyle-HorizontalAlign="Left">
		                <ItemTemplate>
			                <asp:Button runat="server" ID="lookupID" Text='<%# DataBinder.Eval(Container, "DataItem.Value") %>' 
			                    CommandArgument='<%# DataBinder.Eval(Container, "DataItem.LookupID") %>' CommandName="Select" 
			                    CausesValidation="false" CssClass="nameButton" OnClientClick="disableButton(this);" />
		                </ItemTemplate>
	                </asp:DataList>
                </div>

                <div class="footer">
                    <div class="footerLeft">
                        <asp:Button ID="btnAbilityCancel" runat="server" Text="Cancel" CssClass="cancelButton" onclick="Cancel_Click" OnClientClick="disableButton(this);" />
                    </div>
                </div>
                <input type="hidden" id="ihAttendeesToProcess" runat="server" />
            </asp:Panel>

            
            <asp:Panel ID="pnlSelectService" runat="server" CssClass="view">
            <!-- Select Service State -->
                <div class="content">
                    <div class="heading">
                        <h3 class="checkinText">Select Services</h3>
                    </div>
                    <asp:DataList CssClass="centeredList" GridLines="None" ID="dgEventTimes" runat="server" CellSpacing="5" RepeatColumns="1" BorderColor="Black" BorderWidth="0" Width="350" 
                        OnSelectedIndexChanged="dgEventTimes_SelectedIndexChanged" OnItemDataBound="dgEventTimes_ItemDataBound" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <div class="item">
                                <asp:Button runat="server" ID="btnService" CommandName="Select" CausesValidation="false" CssClass="dataButton" />
                                <asp:ImageButton id="imgChecked" runat="server" CommandName="Select" CssClass="star" ImageUrl="images/empty_checkbox.png" />
                            </div>
                        </ItemTemplate>
                    </asp:DataList>
                </div>

                <div class="footer">
                    <div class="footerLeft">
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="cancelButton" onclick="Cancel_Click" OnClientClick="disableButton(this);" />
                    </div>
                    <div class="footerRight">
                        <asp:Button ID="btnServicesContinue" runat="server" Text="Next" CssClass="nextButton" onclick="btnServicesContinue_Click" OnClientClick="disableButton(this);" />
                    </div>
                </div>
            </asp:Panel>

            
            <asp:Panel ID="pnlConfirm" runat="server" CssClass="view">
            <!-- Confirm State -->
                <div class="content">
                    <div class="heading">
                        <h3 class="checkinText">Confirm</h3>
                    </div>
                    <div class="scrollAreaBig scroll-pane"><asp:PlaceHolder ID="phConfirm" runat="server" /></div>
                    <div id="divConfirmError" runat="server" visible="false">
                        <p><asp:Label ID="lblConfirmError" runat="server" CssClass="errorCaption" /></p>
                    </div>
                    <div class="footer">
                        <div class="footerLeft">
                            <asp:Button ID="btnConfirmCancel" runat="server" Text="Cancel" CssClass="cancelButton" onclick="Cancel_Click" OnClientClick="disableButton(this);" />
                        </div>
                        <div class="footerRight">
                            <asp:Button ID="btnConfirmContinue" runat="server" Text="Next" CssClass="nextButton" onclick="btnConfirmContinue_Click" OnClientClick="disableButton(this);" />
		                </div>
                    </div>
                </div>
            </asp:Panel>

            
            <asp:Panel ID="pnlResults" runat="server" CssClass="view">
            <!-- Result State -->
                <div class="content">
                    <div class="heading">
                        <h3 class="checkinText">Thank You!</h3>
                    </div>
                    <div class="resultScrollArea scroll-pane"><asp:PlaceHolder ID="phResults" runat="server" /></div>
                </div>
                <div class="footer">
                    <div class="footerRight">
                        <asp:Button ID="btnResultsContinue" runat="server" Text="Finish" CssClass="nextButton" onclick="btnResultsContinue_Click" OnClientClick="disableButton(this);" />
                    </div>
                </div>
            </asp:Panel>
            
            
            <asp:Panel ID="pnlBadKiosk" runat="server" CssClass="view">
            <!-- Bad Kiosk State -->
                <div class="heading">
                    <h3 class="checkinText">Error!</h3>
                    <asp:Label ID="lblBadKiosk" runat="server" CssClass="errorText" />
                </div>
                <div class="footer">
                    <div class="footerLeft">
                        <asp:Button ID="btnTryAgain" runat="server" Text="Retry" CssClass="cancelButton" onclick="btnTryAgain_Click" />
		            </div>
                </div>
            </asp:Panel>


            <!-- End Views -->
            <asp:Button ID="btnRedirect" runat="server" OnClick="Redirect_Click" Width="1" Height="1" />
            <input type="hidden" id="ihStartTime" runat="server" />
            <input type="hidden" id="ihNowTime" runat="server" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
