<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MoveAttendance.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Checkin.MoveAttendance" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<asp:ScriptManagerProxy ID="smpScripts" runat="server">
    <Scripts>
        <asp:ScriptReference Path="http://code.jquery.com/jquery-latest.js" />
    </Scripts>
</asp:ScriptManagerProxy>
<asp:UpdatePanel runat="server" ID="upMoveAttendanceModule">
	<ContentTemplate>
		<asp:Button runat="server" ID="btnMoveAttendance" CssClass="smallText" Text="Move Attendance..." 
			onclick="btnMoveAttendance_Click"  />
		<asp:UpdatePanel ID="upMoveAttendance" runat="server" UpdateMode="Always" Visible="false">
			<ContentTemplate>
				<fieldset style="font: messagebox; width: 550px;">
					<legend>Move Attendance Records...</legend>
					<span class="smallText" style="margin-left: 20px;">Select where you want to move the attendance records.</span>
					<table style="margin-left: 20px;">
						<tr>
							<td class="formLabel" >Attendance Type: </td>
							<td><asp:DropDownList runat="server" ID="ddlOccurrenceTypes" CssClass="formItem" DataTextField="TypeName" 
								DataValueField="OccurrenceTypeID" AutoPostBack="true" 
								onselectedindexchanged="ddlOccurrenceTypes_SelectedIndexChanged"></asp:DropDownList></td>
						</tr>
						<tr>
							<td class="formLabel" >Occurrence:</td>
							<td><asp:DropDownList runat="server" ID="ddlOccurrences" CssClass="formItem" ></asp:DropDownList></td>
						</tr>
						<tr>
						<td></td>
						<td><asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="smallText" 
								onclick="btnCancel_Click" /> &nbsp; <asp:Button runat="server" 
								ID="btnMoveRecords" Text=" Go " CssClass="smallText" 
								onclick="btnMoveRecords_Click" /></td>
						</tr>
					</table>
				</fieldset>
				<br />
				<br />
			</ContentTemplate>
		</asp:UpdatePanel>
		<asp:Label runat="server" ID="lblMessage" CssClass="errorText" Visible="false" Text=""></asp:Label>
	</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	function pageLoad()
	{
		var message = $get("<%= lblMessage.ClientID %>");

		if ( message )
		{
			window.setTimeout("fadeAndClear()", 5000);
		}
	}

	function fadeAndClear()
	{
		$('span[id$=lblMessage]').fadeOut('slow')
	}

</script>