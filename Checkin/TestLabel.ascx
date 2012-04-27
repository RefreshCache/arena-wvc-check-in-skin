<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TestLabel.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Checkin.TestLabel" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<script type="text/javascript" src="Include/scripts/dimmingdiv.js"></script>

<Arena:ModalPopup ID="mpSelectLocations" runat="server" CancelControlID="btnCancelLocationSelect"
	Title="Select Printers at Locations" DefaultFocusControlID="tvLocations">
	<Content>
		<ComponentArt:TreeView ID="tvLocations" DragAndDropEnabled="true" NodeEditingEnabled="true"
			KeyboardEnabled="true" CssClass="formItem" NodeCssClass="formItem" LineImageWidth="19"
			LineImageHeight="20" DefaultImageWidth="16" DefaultImageHeight="16" ItemSpacing="0"
			NodeLabelPadding="3" ParentNodeImageUrl="" LeafNodeImageUrl="" ShowLines="true"
			LineImagesFolderUrl="~/include/ComponentArt/TreeView/images/lines" EnableViewState="true"
			ClientSideOnNodeCheckChanged="locationCheckAll" runat="server">
		</ComponentArt:TreeView>
	</Content>
	<Buttons>
		<asp:Button runat="server" ID="btnDone" CssClass="smallText" Text="Done" CausesValidation="False" />
		<asp:Button runat="server" ID="btnCancelLocationSelect" CssClass="smallText" Text="Cancel"
			CausesValidation="false" />
	</Buttons>
</Arena:ModalPopup>
<asp:UpdatePanel runat="server" UpdateMode="Always">
	<ContentTemplate>
		<table style="background-color: #f5f5f5">
			<tr>
				<td style="vertical-align: top" class="formLabel">
					
					<fieldset style="font: messagebox; width: 370px;">
						<legend>Location / Printer Selection</legend>
						<br />
						<table style="width: 100%" cellpadding="0" cellspacing="0">
							<tr>
								<td valign="top" style="width: 50px; padding-left: 10px"><img src="UserControls/Custom/Cccev/Checkin/images/printer_preferences.png" /></td>
								<td valign="top" >Select one or more printers from the locations.<br /><br />
									<asp:TextBox ID="txtDisplayLocations" runat="server" Width="300" TextMode="MultiLine"
										Rows="2" CssClass="smallText" style="background-color: #e5e5e5" ReadOnly="True"></asp:TextBox><br />
									<asp:Button runat="server" ID="btnSelectLocations" CssClass="smallButton" Text="Select..."
										OnClientClick="showModal()" CausesValidation="False" />
								</td>
							</tr>
						</table>
					</fieldset>
					<br />
					
					<fieldset style="FONT: messagebox; WIDTH: 370px;"><legend>Simple Test</legend>
						<table style="width: 100%; padding-top: 8px" cellpadding="0" cellspacing="0">
							<tr>
								<td valign="top" style="width: 50px; padding-left: 10px"><img src="UserControls/Custom/Cccev/Checkin/images/test_label_icon.png" /></td>
								<td valign="top" style="font-weight: normal" >Print a single, sample "test" label to the selected printers using the standard (CCCEV Print Label Provider) print provider.<br /><br />
									<asp:Button ID="btnPrint" runat="server" Text="Print Test" Width="80px" OnClick="btnPrint_Click"
									 CssClass="smallButton" CausesValidation="False"></asp:Button>
								</td>
							</tr>
						</table>
					</fieldset>
					<br />
					
					<fieldset style="font: messagebox; width: 370px; height: 350px">
						<legend>Create a Sample Label for Testing</legend>
						<table style="width: 100%" cellpadding="0" cellspacing="0">
							<tr>
								<td>
									Start Times:
								</td>
								<td>
									<asp:TextBox ID="txtServices" runat="server" Width="208px">4:30 PM, 6:15 PM</asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									First Name:
								</td>
								<td>
									<asp:TextBox ID="txtFirstName" runat="server" Width="208px">TEST</asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									Last Name:
								</td>
								<td>
									<asp:TextBox ID="txtLastname" runat="server" Width="208px">LABEL</asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									Birthday:
								</td>
								<td>
									<asp:TextBox ID="txtBirthday" runat="server" Width="208px" />
								</td>
							</tr>
							<tr>
								<td>
									Allowed to Self Check-out:
								</td>
								<td>
									<asp:RadioButtonList ID="rblSelfCheckOut" runat="server" Font-Names="Verdana" Font-Size="XX-Small">
										<asp:ListItem Value="true" Selected="True">Yes</asp:ListItem>
										<asp:ListItem Value="false">No</asp:ListItem>
									</asp:RadioButtonList>
								</td>
							</tr>
							<tr>
								<td>
									Legal Notes:
								</td>
								<td>
									<asp:RadioButtonList ID="rblLegalNotes" runat="server" Font-Names="Verdana" Font-Size="XX-Small"
										Width="48px" Height="48px">
										<asp:ListItem Value="true" Selected="True">Yes</asp:ListItem>
										<asp:ListItem Value="false">No</asp:ListItem>
									</asp:RadioButtonList>
								</td>
							</tr>
							<tr>
								<td>
									Security Code:
								</td>
								<td>
									<asp:TextBox ID="txtSecurityCode" runat="server" Width="208px">AB1234</asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									Age Group:
								</td>
								<td>
									<asp:TextBox ID="txtAgeGroup" runat="server" Width="208px">6th Grade</asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									Health Notes:
								</td>
								<td>
									<asp:TextBox ID="txtHealthNotes" runat="server" Width="228px" TextMode="MultiLine"
										Height="45px">This is a test...health notes (if any) would be pulled from a child's record and placed here.</asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									&nbsp;
								</td>
								<td align="center">
									&nbsp;
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<asp:Button ID="btnPrintAttendance" runat="server" Text="Attendance Label" Width="112px"
										OnClick="btnPrintAttendance_Click" CssClass="smallButton" CausesValidation="False">
									</asp:Button>
									<asp:Button ID="btnPrintClaimCard" runat="server" Text="Claim Card" Width="112px"
										OnClick="btnPrintClaimCard_Click" CssClass="smallButton" CausesValidation="False">
									</asp:Button>
									<asp:Button ID="btnPrintNametag" runat="server" Text="Nametag" Width="112px" OnClick="btnPrintNametag_Click"
										CssClass="smallButton" CausesValidation="False"></asp:Button><br />
									<asp:Button ID="btnPrintAll" runat="server" Text="All" Width="112px" OnClick="btnPrintAll_Click"
										CssClass="smallButton" CausesValidation="False"></asp:Button>

								</td>
							</tr>
						</table>
						<br />
						<br />
					</fieldset>
					
					<br />
					
					<fieldset style="font: messagebox; width: 370px;">
						<legend>Miscellaneous</legend>&nbsp;
						<input type="button" id="btnRefresh" runat="server" causesvalidation="false" onserverclick="btnRefresh_Click"
							style="visibility: hidden; display: none;" />
						<input type="hidden" id="ihPersonList" runat="server" name="ihPersonList">
						<table style="width: 100%" cellpadding="0" cellspacing="0">
							<tr>
								<td valign="top" style="width: 50px; padding-left: 10px"><img src="UserControls/Custom/Cccev/Checkin/images/data_gear.png" /></td>
								<td valign="top"><span style="font-weight: normal">Test printing a real label using your configured IPrintLabel provider.</span><br /><br />
									Provider: <Arena:LookupDropDown ID="providerLookup" runat="server" SearchEnabled="false" AddEnabled="false" IsValueRequired="true" /><br />
									PersonID:
									<asp:TextBox ID="txtPersonID" runat="server"></asp:TextBox><asp:Button runat="server"
										ID="btnSelectPerson" CssClass="smallButton" OnClientClick="openSearchWindow();return false;"
										Text="Select..." />
									<asp:RequiredFieldValidator runat="server" ID="rfvPerson" ControlToValidate="txtPersonID"
										ErrorMessage="<br>You must select a person"></asp:RequiredFieldValidator>
									<br />
									<asp:Button ID="btnTest" runat="server" Text="Test IPrintLabel" Width="112px" OnClick="btnTest_Click"
										CssClass="smallButton"></asp:Button>
								</td>
							</tr>
						</table>
					</fieldset>
				</td>
				<td style="vertical-align: top">
					<fieldset style="font: messagebox; width: 450px; height: 547px">
						<legend>Output</legend>
						<div style="border-right: 2px inset; padding-right: 0px; border-top: 2px inset; padding-left: 3px;
							padding-bottom: 0px; border-left: 2px inset; width: 99%; padding-top: 0px; border-bottom: 2px inset;
							height: 97%; background-color: #e5e5e5; overflow: auto">
							<asp:Label ID="lblSuccessMessage" runat="server" ForeColor="Black" Visible="False">
							</asp:Label><asp:Label
								ID="lblErrorMessage" runat="server" ForeColor="Red" Visible="False"></asp:Label></div>
					</fieldset>
				</td>
			</tr>
		</table>
	</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	function locationCheckAll(node)
	{
		if (node.Checked == true)
			node.CheckAll();
		else
			node.UnCheckAll();
		node.ParentTreeView.Render();
		return true;
	}

	function enableValidation()
	{
		var rfv = $find('<%= rfvPerson.ClientID %>');
		ValidatorEnable(rfv, true);
	}

	function showModal()
	{
		var modal = $find('<%= mpSelectLocations.ClientID %>');
		if (modal)
		{
			modal.show();
		}
	}
</script>

