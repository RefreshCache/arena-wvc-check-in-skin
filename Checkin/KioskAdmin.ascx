<%@ Control Language="C#" AutoEventWireup="true" CodeFile="KioskAdmin.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.CheckIn.KioskAdmin" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<style>
fieldset 
{
    margin: 1em 0;
    padding: 1em;
    border: 1px solid #CCC;
}

fieldset p 
{
    margin: 2px 12px 10px 10px;
}

legend 
{
    font-size: 1.1em;
    font-weight: 600;
    padding: 2px 4px 8px 4px;
}
</style>
<script type="text/javascript">
	function locationCheckAll(node)
	{
		if (node.Checked == true)
		{
			node.CheckAll();
		}
		else
		{
			node.UnCheckAll();
		}

		node.ParentTreeView.Render();
		return true;
	}
</script>

<asp:UpdatePanel ID="upKioskAdmin" runat="server" UpdateMode="Always">
	<ContentTemplate>
		<asp:Panel ID="pnlEdit" runat="server">
		<asp:HiddenField ID="hfSystemID" runat="server"/>
			<fieldset>
				<legend><asp:Literal ID="litLegend" runat="server"></asp:Literal></legend>
				<p class="formLabel">Name
				<br />
				<asp:TextBox ID="txtName" runat="server" CssClass="formItem"></asp:TextBox>
				</p>
				<p class="formLabel">DNS Name
				<br />
				<asp:TextBox ID="txtDnsName" runat="server" CssClass="formItem"></asp:TextBox>
				</p>
				<p class="formLabel">Description
				<br />
				<asp:TextBox ID="txtDescription" runat="server" CssClass="formItem"></asp:TextBox>
				</p>
				<p class="formLabel">
				IP Address
				<br />
				<asp:TextBox ID="txtIPAddress" runat="server" CssClass="formItem"></asp:TextBox>
				</p>
				
				<br />

				<p class="formLabel">
				Default Printer
				<br />
				<asp:DropDownList ID="ddlPrinters" Runat="server" AutoPostBack="true" CssClass="formItem"></asp:DropDownList>
				<asp:Label ID="lblPrinterDescription" runat="server"></asp:Label>
				</p>

				<fieldset>
				<legend>Locations</legend>
					<p class="formLabel">
					The locations for which this kiosk can perform check-in.
					<br />
					<ComponentArt:TreeView ID="tvLocations" runat="server" CssClass="formItem"></ComponentArt:TreeView>
					</p>
				</fieldset>

				<asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" /> &nbsp; 
				<asp:LinkButton ID="lbCancel"
					runat="server" onclick="lbCancel_Click">Cancel</asp:LinkButton>
			</fieldset>

		</asp:Panel>

	</ContentTemplate>
</asp:UpdatePanel>
