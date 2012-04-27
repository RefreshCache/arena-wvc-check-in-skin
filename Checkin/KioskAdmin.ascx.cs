/**********************************************************************
 * Description:	A simple module to allow the kiosk to be registered and
 *				edited.  It is expected that only kiosks would navigate
 *				to this page and when they do, it will attempt to find
 *				the corresponding "kiosk" record in Arena otherwise it
 *				will auto-fill the form using the information from the
 *				client/Request object.
 *				
 *				This module will navigate back to a "returnURL" encoded
 *				value in the QueryString, or if non was given it will
 *				return back to the configured Module Setting URL, or
 *				if non was set, it will navigate back to the original
 *				Referring URL.
 *				
 * Created By:   Nick Airdo @ Central Christian Church of the East Valley
 * Date Created: 11/09/2010 10:41 AM
 *
 * $Workfile: KioskAdmin.ascx.cs $
 * $Revision: 1 $ 
 * $Header: /trunk/Arena/UserControls/Custom/Cccev/Checkin/KioskAdmin.ascx.cs   1   2010-11-10 16:00:39-07:00   nicka $
 *  
 * $Log: /trunk/Arena/UserControls/Custom/Cccev/Checkin/KioskAdmin.ascx.cs $
*  
*  Revision: 1   Date: 2010-11-10 23:00:39Z   User: nicka 
*  A simple module to allow the kiosk to be registered and edited. 
 **********************************************************************/
using System;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;

using ComponentArt.Web.UI;

using Arena.Computer;
using Arena.Custom.Cccev.CheckIn;
using Arena.DataLayer.ComputerSystem;
using Arena.Organization;
using Arena.Portal;
using Arena.Security;

namespace ArenaWeb.UserControls.Custom.Cccev.CheckIn
{
	public partial class KioskAdmin : PortalControl
	{
		private bool EditEnabled;
		private ComputerSystem kiosk = null;

		#region Module Settings

		[TextSetting( "Return URL", "URL to return to after kiosk management.", false )]
		public string ReturnUrlSetting { get { return Setting( "ReturnUrl", "", false ); } }

		#endregion

		protected void Page_Load( object sender, EventArgs e )
		{
			EditEnabled = CurrentModule.Permissions.Allowed( OperationType.Edit, CurrentUser );

			if ( !IsPostBack )
			{
				pnlEdit.Visible = true;
				string ipAddress = IPNetworking.GetIP4Address( HttpContext.Current.Request );
				kiosk = CheckInController.GetCurrentKiosk( ipAddress );

				if ( kiosk != null )
				{
					litLegend.Text = string.Format( "Edit '{0}' Kiosk", kiosk.SystemName );
					btnSave.Text = "Save";
					EditKiosk( kiosk, ipAddress );
				}
				else
				{
					litLegend.Text = "Register New Kiosk";
					btnSave.Text = "Add New";
					EditKiosk( IPNetworking.GetHostName( ipAddress ), ipAddress );
				}
			}
			else
			{
				ViewState["returnURL"] = Request.UrlReferrer.ToString();
			}
		}

		#region Event Methods

		protected void lbCancel_Click( object sender, EventArgs e )
		{
			ReturnControl();
		}

		protected void btnSave_Click( object sender, EventArgs e )
		{
			if ( hfSystemID.Value != "-1" )
			{
				kiosk = new ComputerSystem( Convert.ToInt32( hfSystemID.Value ) );
			}
			else
			{
				kiosk = new ComputerSystem();
			}

			kiosk.SystemName = txtName.Text;
			kiosk.DNSName = txtDnsName.Text;
			kiosk.Notes = txtDescription.Text;
			kiosk.Active = true;
			kiosk.Kiosk = true;

			ComputerSystemPrinter printer = null;
			if ( ddlPrinters.SelectedValue != "-1" )
			{
				printer = new ComputerSystemPrinter( Convert.ToInt32( ddlPrinters.SelectedValue ) );
			}

			kiosk.Printer = printer;
			kiosk.Save();

			// Save IP address
			ComputerSystemData data = new ComputerSystemData();
			data.SaveKioskIpAddress( kiosk.SystemId, txtIPAddress.Text );

			// save Locations
			data.DeleteSystemLocations( kiosk.SystemId );
			foreach ( TreeViewNode node in tvLocations.CheckedNodes )
			{
				if ( node.Depth == 1 )
				{
					data.SaveSystemLocation( kiosk.SystemId, Convert.ToInt32( node.ID ) );
				}
			}

			ReturnControl();
		}

		#endregion

		#region Views

		private void EditKiosk( string dnsName, string ipAddress )
		{
			kiosk = new ComputerSystem();
			kiosk.Kiosk = true;
			kiosk.DNSName = dnsName;
			kiosk.SystemName = dnsName.Split( '.' )[0];
			kiosk.Notes = "A kiosk for use with Arena check-in.";
			EditKiosk( kiosk, ipAddress );
		}

		private void EditKiosk( ComputerSystem kiosk, string ipAddress )
		{
			hfSystemID.Value = kiosk.SystemId.ToString();

			LoadPrinters();
			LoadLocations();

			CurrentPortalPage.TemplateControl.Title = kiosk.SystemName;
			txtName.Text = kiosk.SystemName;
			txtDnsName.Text = kiosk.DNSName;
			txtIPAddress.Text = ipAddress;
			txtDescription.Text = kiosk.Notes;

			if ( kiosk.SystemId != -1 )
			{
				if ( kiosk.Printer != null )
				{
					ddlPrinters.Items.FindByValue( kiosk.Printer.PrinterId.ToString() ).Selected = true;
					lblPrinterDescription.Text = kiosk.Printer.PrinterDescription;
				}
				else
				{
					ddlPrinters.Text = "-1";
				}

				foreach ( Location location in kiosk.Locations )
				{
					tvLocations.FindNodeById( location.LocationId.ToString() ).Checked = true;
				}
			}
			btnSave.Visible = EditEnabled;
		}

		#endregion

		#region Misc View Helper Methods

		/// <summary>
		/// Bind the available locations for this organization to the Tree View.
		/// This code is almost an exact copy of the code found in Arena's KioskList
		/// module.
		/// </summary>
		private void LoadLocations()
		{
			LocationCollection locations = new LocationCollection( int.Parse( WebConfigurationManager.AppSettings["Organization"] ) );
			ArenaTreeUtility.Setup( tvLocations, "locationCheckAll" );

			tvLocations.Nodes.Clear();
			int buildingID = 0;
			TreeViewNode node = null;
			TreeViewNode node2 = null;
			foreach ( Location location in locations )
			{
				if ( location.BuildingID != buildingID )
				{
					buildingID = location.BuildingID;
					node = new TreeViewNode();
					if ( location.BuildingID == -1 )
					{
						node.Text = "[No Building]";
					}
					else
					{
						node.Text = location.Building.BuildingName;
					}
					node.ShowCheckBox = true;
					node.Expanded = true;
					node.ImageUrl = "~/include/ComponentArt/images/facilities.gif";
					node.ID = "Build" + location.LocationId.ToString();
					tvLocations.Nodes.Add( node );
				}
				node2 = new TreeViewNode();
				node2.Text = location.LocationName;
				node2.ID = location.LocationId.ToString();
				node2.ShowCheckBox = true;
				node2.ImageUrl = "~/include/ComponentArt/images/location.gif";
				node.Nodes.Add( node2 );
			}
		}

		/// <summary>
		/// Bind the available printers for this organization to the drop down list.
		/// This code is almost an exact copy of the code found in Arena's KioskList
		/// module.
		/// </summary>
		private void LoadPrinters()
		{
			ddlPrinters.Items.Clear();
			ddlPrinters.Items.Add( new ListItem( "[Use Kiosk's Default Windows Printer]", "-1" ) );
			ComputerSystemPrinterCollection printers = new ComputerSystemPrinterCollection( int.Parse( WebConfigurationManager.AppSettings["Organization"] ) );
			foreach ( ComputerSystemPrinter printer in printers )
			{
				ddlPrinters.Items.Add( new ListItem( printer.PrinterName, printer.PrinterId.ToString() ) );
			}
		}

		#endregion

		private void ReturnControl()
		{
			string returnURL = null;
			if ( Request.QueryString["returnURL"] != null )
			{
				returnURL = Server.UrlDecode( Request.QueryString["returnURL"] );
			}
			else if ( !ReturnUrlSetting.Equals( "" ) )
			{
				returnURL = ReturnUrlSetting;
			}
			else
			{
				returnURL = ViewState["returnURL"].ToString();
			}

			if ( returnURL != null )
			{
				Response.Redirect( returnURL );
			}
		}
	}
}

/// <summary>
/// A utility class for setting up a standard TreeView...
/// to possibly be moved into our Framework or Data utils library.
/// </summary>
public class ArenaTreeUtility
{
	public static void Setup( ComponentArt.Web.UI.TreeView treeView, string clientSideNodeChangedCallback )
	{
		treeView.DragAndDropEnabled = false;
		treeView.NodeEditingEnabled = false;
		treeView.KeyboardEnabled = true;
		treeView.CssClass = "TreeView";
		treeView.NodeCssClass = "TreeNode";
		treeView.SelectedNodeCssClass = "SelectedTreeNode";
		treeView.NodeEditCssClass = "NodeEdit";
		treeView.LineImageWidth = 13;
		treeView.LineImageHeight = 20;
		treeView.DefaultImageWidth = 10;
		treeView.DefaultImageHeight = 10;
		treeView.ItemSpacing = 0;
		treeView.NodeLabelPadding = 3;
		treeView.ParentNodeImageUrl = "~/include/ComponentArt/TreeView/images/page.gif";
		treeView.LeafNodeImageUrl = "~/include/ComponentArt/TreeView/images/page.gif";
		treeView.ShowLines = true;
		treeView.LineImagesFolderUrl = "~/include/ComponentArt/TreeView/images/lines";
		treeView.EnableViewState = true;
		treeView.ClientSideOnNodeCheckChanged = clientSideNodeChangedCallback;
	}
}

/// <summary>
/// A utility class for dealing with IIS7 IPv6 vs IPv4 address issues...
/// to possibly be moved into our Framework or Data utils library.
/// </summary>
public class IPNetworking
{
	public static string GetHostName( string ipAddress )
	{
		IPHostEntry host = Dns.GetHostEntry( ipAddress );
		if ( host != null )
		{
			return host.HostName;
		}

		return "";
	}

	public static string GetIP4Address( HttpRequest request )
	{
		string IP4Address = String.Empty;

		foreach ( IPAddress IPA in Dns.GetHostAddresses( request.UserHostAddress ) )
		{
			if ( IPA.AddressFamily.ToString() == "InterNetwork" )
			{
				IP4Address = IPA.ToString();
				break;
			}
		}

		if ( IP4Address != String.Empty )
		{
			return IP4Address;
		}

		foreach ( IPAddress IPA in Dns.GetHostAddresses( Dns.GetHostName() ) )
		{
			if ( IPA.AddressFamily.ToString() == "InterNetwork" )
			{
				IP4Address = IPA.ToString();
				break;
			}
		}

		return IP4Address;
	}
}