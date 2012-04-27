<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OccurrenceTypeAttributeList.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.CheckIn.OccurrenceTypeAttributeList" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<%@ Register Src="~/UserControls/DoubleListBox.ascx" TagName="DoubleListBox" TagPrefix="arenaUC" %>
<br />
<asp:UpdatePanel ID="upAttendanceTypeAttributeList" runat="server" UpdateMode="Always">
    <ContentTemplate>
		<h3>Extended Attributes</h3>
		<asp:Panel id="pnlList" Runat="server">
			<asp:LinkButton ID="lbAdd" Runat="server" CssClass="smallText" Visible="False" OnClick="lbAdd_Click">Add</asp:LinkButton>
			<Arena:DataGrid id=dgList Runat="server" AllowSorting="false" AddImageUrl="~/images/add_attribute_group.gif">
			<Columns>
				<asp:boundcolumn HeaderText="ID" datafield="OccurrenceTypeAttributeId" ReadOnly="true" 
					Visible="False"></asp:boundcolumn>

				<asp:boundcolumn
 					HeaderText="Name" 
					ItemStyle-Wrap="False"
					ItemStyle-VerticalAlign="Top">
				 </asp:boundcolumn>
				
				<asp:boundcolumn
					HeaderText="Ability Level"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
				</asp:boundcolumn>
				
				<asp:boundcolumn 
 					HeaderText="Special Needs" 
					datafield="IsSpecialNeeds"
					DataFormatString="<img src='images/check.gif'>"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
				</asp:boundcolumn>
				
				<asp:BoundColumn 
				    HeaderText="Room Balancing" 
				    DataField="IsRoomBalancing" 
				    DataFormatString="<img src='images/check.gif' />" 
				    HeaderStyle-HorizontalAlign="Center" 
				    ItemStyle-HorizontalAlign="Center">
				</asp:BoundColumn>
					
				<asp:boundcolumn
					HeaderText="Starting Letter"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center"
					datafield="LastNameStartingLetter">
				</asp:boundcolumn>
					
				<asp:boundcolumn
					HeaderText="Ending Letter"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center"
					datafield="LastNameEndingLetter">
				</asp:boundcolumn>
				
				<asp:ButtonColumn
					CommandName="EditItem" 
					ButtonType="LinkButton" 
					Text="<img src='images/edit.gif' border='0'>"
					ItemStyle-Wrap="False"
					ItemStyle-VerticalAlign="Top"></asp:ButtonColumn>
					
			</Columns>
		</Arena:DataGrid>

		</asp:Panel>

		<asp:Panel ID="pnlEdit" runat="server">
			<asp:Label ID="lblErrorMessage" runat="server" CssClass="errorText" />
			<asp:HiddenField ID="hfOccurrenceTypeAttributeID" runat="server" />
			<table border="0" cellpadding="0" cellspacing="0" style="border: solid 1px #DDDDDD; padding-left: 5px; padding-right: 5px;" class="listItem">
				<tr style="background-image: url(images/datagrid_header_background.jpg);" class="listItem">
					<td class="heading2" colspan="2" style="height: 19px">
						Extended Attributes</td>
				</tr>
				<tr>
					<td valign="top">
						<table width="100%">
							<tr>
								<td class="formLabel" nowrap="nowrap" valign="top" width="20%">
									Type &nbsp;&nbsp;</td>
								<td valign="top"><asp:Label ID="lblOccurrenceType" CssClass="formItem" runat="server"></asp:Label><asp:DropDownList ID="ddlOccurrenceType" runat="server" Visible="false" CssClass="formItem"></asp:DropDownList>
								</td>
							</tr>
							<tr>
								<td class="formLabel" nowrap="nowrap" valign="top" width="20%">
									Is special needs &nbsp;&nbsp;</td>
								<td valign="top">
									<asp:CheckBox id="cbIsSpecialNeeds" runat="server" />
								</td>
							</tr>
		                    <tr>
		                        <td class="formLabel" nowrap="nowrap" valign="top" width="20%">
		                            Room Balancing &nbsp;&nbsp;
		                        </td>
		                        <td valign="top">
		                            <asp:CheckBox ID="cbIsRoomBalancing" runat="server" />
		                        </td>
		                    </tr>
							<tr >
								<td class="formLabel" nowrap="nowrap" valign="top">
									Ability level &nbsp;&nbsp;</td>
								<td valign="top">
								<arenaUC:DoubleListBox id="dlbAbilityLevels" runat="server" Width="150" Rows="5" EnableMoveAll="false" EnableAddEditDelete="false"
									ButtonCssClass="smallText" ListBoxCssClass="formItem"
									HeaderLabelLeft-Text="Available" HeaderLabelRight-Text="Selected" 
									HeaderLabelLeft-CssClass="smallText" HeaderLabelRight-CssClass="smallText" />
								</td>
							</tr>
		                    
							<tr >
								<td class="formLabel" nowrap="nowrap" valign="top">
									Lastname starting letter &nbsp;&nbsp;</td>
								<td valign="top">
									<asp:TextBox ID="txtLastnameStartingLetter" runat="server" Columns="1" CssClass="formItem" Rows="1"></asp:TextBox></td>
							</tr>
							<tr >
								<td class="formLabel" nowrap="nowrap" valign="top">
									Lastname ending letter &nbsp;&nbsp;</td>
								<td valign="top">
									<asp:TextBox ID="txtLastnameEndingLetter" runat="server" Columns="1" CssClass="formItem" Rows="1"></asp:TextBox></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td align="right" colspan="2" style="padding-right: 15px; padding-top: 8px; padding-bottom:8px; border-top: #cccccc 1px solid">
						<asp:Button ID="btnSave" runat="server" CssClass="smallText" Text="Save" OnClick="btnUpdate_Click" /> &nbsp;
						<asp:Button ID="btnCancel" runat="server" CssClass="smallText" Text="Cancel" OnClick="btnCancel_Click" CausesValidation="false" />
					</td>
				</tr>
			</table>
		</asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>