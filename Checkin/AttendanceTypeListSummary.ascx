<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AttendanceTypeListSummary.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.CheckIn.AttendanceTypeListSummary" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<!--
$Id: $
-->
<asp:UpdatePanel ID="upAttendanceTypeList" runat="server" UpdateMode="Always">
    <ContentTemplate>
		
		<asp:LinkButton runat="server" CssClass="smallText"  ID="btnShowSummary" Text="Show Summary" Width="100" ToolTip="press to view the summary" OnClick="btnShowSummary_Click"/>

		<asp:Panel id="pnlList" Runat="server" Visible="false">
		<div class="listFilter">
			<table cellpadding="0" cellspacing="3" border="0" width="100%">
			<tr>
				<td valign="top" rowspan="2" align="left" style="padding-left:10px;padding-top:2px"><img src="images/filter.gif" border="0"></td>
				<td  valign="middle" class="formLabel" nowrap="nowrap">
					Filter by Service</td>
				<td valign="middle"><asp:DropDownList ID="ddlStartTimes" runat="server" CssClass="formItem" AutoPostBack="true" OnSelectedIndexChanged="ddlStartTimes_Changed" DataTextField="Value" DataMember="Key">
					</asp:DropDownList></td>
				<td style="width:100%"></td>
			</tr>
			<tr>
				<td><asp:LinkButton runat="server" CssClass="smallText"  ID="btnRefreshSummary" Text="refresh" bToolTip="refresh the data" OnClick="btnRefreshSummary_Click"/></td>
				<td></td>
				<td></td>
			</tr>
			</table>
		</div>

			<Arena:DataGrid id="dgList" Runat="server" AllowSorting="true" DeleteEnabled="false" EditEnabled="false" ExportEnabled="false">
			<Columns>

				<asp:boundcolumn 
 					HeaderText="ID" 
					datafield="occurrence_type_id" 
					visible="false"></asp:boundcolumn>

				<asp:boundcolumn
 					HeaderText="Name" 
					datafield="type_name"
					ItemStyle-Wrap="False"
					ItemStyle-VerticalAlign="Top">
				 </asp:boundcolumn>
				
				<asp:boundcolumn
					HeaderText="Start Time(s)"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
				</asp:boundcolumn>
				
				<asp:boundcolumn
					HeaderText="Age"
					ItemStyle-Wrap="False"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
				</asp:boundcolumn>
				
				<asp:boundcolumn
					HeaderText="Grade"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
				</asp:boundcolumn>
				
				<asp:boundcolumn
					HeaderText="Ability Level"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
				</asp:boundcolumn>
				
				<asp:TemplateColumn HeaderText="Special Needs"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
					<ItemTemplate><img alt="yes" src='images/check.gif'></ItemTemplate>
				</asp:TemplateColumn>

				<asp:boundcolumn
					HeaderText="Gender"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
				</asp:boundcolumn>
				
				<asp:boundcolumn
					HeaderText="Last Name"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
				</asp:boundcolumn>
				
				<asp:boundcolumn
					HeaderText="Location(s)"
					HeaderStyle-HorizontalAlign="Center"
					ItemStyle-HorizontalAlign="Center">
				</asp:boundcolumn>
			</Columns>
			</Arena:DataGrid>
			<asp:Label runat="server" ID="lblOneTimeFreqNotice" Text="" Visible="false" CssClass="smallText"></asp:Label>
			<asp:Label ID="lblWarning" runat="server" Visible="false" CssClass="smallText"></asp:Label>
			<br />
		</asp:Panel>
	</ContentTemplate>
</asp:UpdatePanel>
