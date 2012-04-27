/**********************************************************************
* Description:	Used to test the check-in system labels
* Created By:	Nick Airdo
* Date Created:	1/5/2009 12:48:03 PM
*
* $Workfile: TestLabel.ascx.cs $
* $Revision: 8 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Checkin/TestLabel.ascx.cs   8   2010-01-21 09:30:03-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Checkin/TestLabel.ascx.cs $
*  
*  Revision: 8   Date: 2010-01-21 16:30:03Z   User: nicka 
*  Added provider selection as a module setting. 
*  
*  Revision: 7   Date: 2009-07-01 16:06:18Z   User: nicka 
*  Updated to work with PrintLabel provider changes as discussed here: 
*  http://checkinwizard.codeplex.com/Thread/View.aspx?ThreadId=57675 
*  
*  Revision: 6   Date: 2009-02-10 18:42:59Z   User: nicka 
*  changed test label to say "test test test" 
*  
*  Revision: 5   Date: 2009-02-08 14:24:21Z   User: nicka 
*  now allows multiple locations to be selected 
*  
*  Revision: 4   Date: 2009-01-22 21:31:39Z   User: nicka 
*  
*  Revision: 3   Date: 2009-01-07 02:45:55Z   User: nicka 
*  Added ability to test using the IPrintLabel provider. 
*  
*  Revision: 2   Date: 2009-01-06 18:24:05Z   User: nicka 
*  Changed to CheckinLabel 
*  
*  Revision: 1   Date: 2009-01-06 16:09:56Z   User: nicka 
*  to test printing labels 
**********************************************************************/
using System;
using Arena.Organization;
using Arena.Portal;
using Arena.Custom.Cccev.CheckIn;
using Arena.DataLayer.Organization;
using Arena.Custom.Cccev.CheckIn.Entity;
using Arena.Core;
using ComponentArt.Web.UI;
using System.Text;
using Arena.Custom.Cccev.FrameworkUtils.FrameworkConstants;

namespace ArenaWeb.UserControls.Custom.Cccev.Checkin
{
	public partial class TestLabel : PortalControl
	{
		#region Module Settings
		[LookupSetting( "Label Print Provider", "The default print provider for printing name tags.", true, SystemGuids.CHECKIN_PRINT_PROVIDER_LOOKUP_TYPE_STRING )]
		public string PrintProviderSetting { get { return Setting( "PrintProvider", "", true ); } }
		#endregion

		protected override void OnLoad( EventArgs e )
		{
			providerLookup.LookupTypeGUID = new Guid( SystemGuids.CHECKIN_PRINT_PROVIDER_LOOKUP_TYPE_STRING );
			if ( !Page.IsPostBack )
			{
				providerLookup.DataBind();
				providerLookup.SelectedLookup = new Lookup( int.Parse( PrintProviderSetting ), true );
			}

			base.OnLoad( e );
		}

		public enum LabelType
		{
			All,
			Attendance,
			Nametag,
			ClaimCard, 
			Test,
			IPrintLabels
		}

		private void Page_Load( object sender, System.EventArgs e )
		{
			lblSuccessMessage.Text = "";
			lblErrorMessage.Text = "";
			if ( ! IsPostBack )
			{
				RegisterScripts();
				BindPrinters();
				txtBirthday.Text = DateTime.Now.ToShortDateString();
			}
			DisplaySelectedLocations();
		}

		private void DisplaySelectedLocations()
		{
			System.Collections.Generic.List<string> locations = new System.Collections.Generic.List<string>();

			foreach ( TreeViewNode node in tvLocations.CheckedNodes )
			{
				// skip all but level 1
				if ( node.Depth == 1 )
					locations.Add( node.Text );
			}
			txtDisplayLocations.Text = string.Join( ", ", locations.ToArray() );
		}

		private void BindLocationPrintersToTreeView(LocationCollection locations)
		{
			tvLocations.Nodes.Clear();
			string buildingName = string.Empty;
			TreeViewNode node = null;
			TreeViewNode node2 = null;
			foreach ( Location location in locations )
			{
				if ( location.Printer == null || location.Printer.PrinterName == string.Empty )
					continue;

				if ( location.BuildingName != buildingName )
				{
					buildingName = location.BuildingName;
					node = new TreeViewNode();
					if ( location.BuildingName.Trim() == "" )
					{
						node.Text = "[No Building]";
					}
					else
					{
						node.Text = location.BuildingName;
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
				node2.ImageUrl = "~/UserControls/Custom/Cccev/Checkin/images/printer.png";
				node.Nodes.Add( node2 );
				node.CollapseAll();
			}

		}
		private void BindPrinters()
		{
			LocationCollection locations = new LocationCollection( this.CurrentOrganization.OrganizationID );
			BindLocationPrintersToTreeView( locations );
		}

		#region Event Handlers

		/// <summary>
		/// Cooking up a method that will simulate checking in a person to a Occurrence
		/// to print out their check-in labels but without actually checking them in.
		/// </summary>
		protected void btnTest_Click( object sender, EventArgs e )
		{
			if ( tvLocations.CheckedNodes.Length > 0 && txtPersonID.Text.Trim() != string.Empty )
			{
				System.Collections.Generic.List<string> success = new System.Collections.Generic.List<string>();
				System.Collections.Generic.List<string> fails = new System.Collections.Generic.List<string>();
				foreach ( TreeViewNode node in tvLocations.CheckedNodes )
				{
					// skip all but level 1
					if ( node.Depth != 1 )
						continue;

					Location location = new Location( int.Parse( node.ID ) );
					try
					{
						IPrintLabel pl = PrintLabelHelper.GetPrintLabelClass( int.Parse( providerLookup.SelectedLookup.Value ) );

						// The IPrintLabel provider prints labels based on a given:
						// attendee, a collection of occurrences, and an attendance record.
						// Our particular provider will print to the location
						// that is set in the first occurrence.
						//
						// We will create multiple occurrences for the collection based
						// on the user's comma delimited start times from the form.
						OccurrenceCollection occurrences = new OccurrenceCollection();
						foreach ( string time in txtServices.Text.Split( ',' ) )
						{
							Occurrence occurrence = new Occurrence();
							occurrence.LocationID = location.LocationId;
							occurrence.StartTime = DateTime.Parse( DateTime.Now.ToShortDateString() + " " + time );
							occurrences.Add( occurrence );
						}

						OccurrenceAttendance attendance = new OccurrenceAttendance();
						attendance.SecurityCode = txtSecurityCode.Text;
						attendance.CheckInTime = DateTime.Now;

						FamilyMember attendee = new FamilyMember( int.Parse( txtPersonID.Text ) );

						pl.Print( attendee, occurrences, attendance, new Arena.Computer.ComputerSystem() );
						success.Add( string.Format( "{0} ({1})", location.LocationName, location.Printer.PrinterName ));
					}
					catch ( System.Exception ex )
					{
						fails.Add( string.Format( "{0} ({1})<BR>{2}", location.LocationName, location.Printer.PrinterName, ex.Message ) + "<BR>StackTrace: " + ex.StackTrace );
					}
				}

				if ( success.Count > 0 )
				{
					lblSuccessMessage.Text = "IPrintLabels Provider successfully printed to room (printer): " + string.Join( ", ", success.ToArray() ) + "<hr>";
					lblSuccessMessage.Visible = true;
				}
				if ( fails.Count > 0 )
				{
					lblErrorMessage.Text = string.Join( "<hr>Failure to print in room (printer):<br>", fails.ToArray() );
					lblErrorMessage.Visible = true;
				}
			}
		}

		protected void PrintType( CheckinLabel labelOrig, LabelType labelType )
		{
			if ( tvLocations.CheckedNodes.Length > 0 )
			{
				System.Collections.Generic.List<string> success = new System.Collections.Generic.List<string>();
				System.Collections.Generic.List<string> fails = new System.Collections.Generic.List<string>();
				foreach ( TreeViewNode node in tvLocations.CheckedNodes )
				{
					if ( node.Depth == 1 )
					{
						Location location = new Location( int.Parse( node.ID ) );
						try
						{
							CheckinLabel label = labelOrig.ShallowCopy();
							if ( LabelType.All == labelType  )
							{
								label.PrintAllLabels( location.Printer.PrinterName );
							}
							else if ( LabelType.Attendance == labelType )
							{
								label.PrintAttendanceLabel( location.Printer.PrinterName );
							}
							else if ( LabelType.ClaimCard == labelType )
							{
								label.PrintClaimCard( location.Printer.PrinterName );
							}
							else if ( LabelType.Nametag == labelType )
							{
								label.PrintNametag( location.Printer.PrinterName );
							}
							else if ( LabelType.Test == labelType )
							{
								label.FullName = location.LocationName;
								label.PrintAttendanceLabel( location.Printer.PrinterName );
							}
							success.Add( string.Format( "<li>{0} ({1})</li>", location.LocationName, location.Printer.PrinterName ) );
						}
						catch ( System.Exception ex )
						{
							fails.Add( string.Format( "{0} ({1})<BR>{2}", location.LocationName, location.Printer.PrinterName, ex.Message ) + "<BR>StackTrace: " + ex.StackTrace );
						}
					}
				}
				if ( success.Count > 0 )
				{
					lblSuccessMessage.Text = labelType.ToString() + " successfully printed in room (printer):<br /><ul>" + string.Join( "", success.ToArray() ) + "</ul><hr>";
					lblSuccessMessage.Visible = true;
				}
				if ( fails.Count > 0 )
				{
					lblErrorMessage.Text = string.Join( "<hr>Failure to print in room (printer):<br>", fails.ToArray() );
					lblErrorMessage.Visible = true;
				}
			}
		}

		protected void btnPrintAll_Click( object sender, EventArgs e )
		{
			CheckinLabel label = InitPrinterLabels();
			PrintType( label, LabelType.All );
		}

		protected void btnPrintAttendance_Click( object sender, EventArgs e )
		{
			CheckinLabel label = InitPrinterLabels();
			PrintType( label, LabelType.Attendance );
		}

		protected void btnPrintClaimCard_Click( object sender, EventArgs e )
		{
			CheckinLabel label = InitPrinterLabels();
			PrintType( label, LabelType.ClaimCard );
		}

		protected void btnPrintNametag_Click( object sender, EventArgs e )
		{
			CheckinLabel label = InitPrinterLabels();
			PrintType( label, LabelType.Nametag );
		}

		protected void btnPrint_Click( object sender, EventArgs e )
		{
			CheckinLabel label = InitPrinterLabels();
			label.AgeGroup = "TEST TEST TEST";
			label.AttendanceLabelTitle = "Arena Label Test";
			label.SecurityToken = "XX1243";
			label.ServicesTitle = "";
			label.Services = "";
			label.HealthNoteFlag = true;
			label.LegalNoteFlag = true;
			label.SelfCheckOutFlag = true;
			label.ParentsInitialsTitle = "system test...system test...system test";

			PrintType( label, LabelType.Test );
		}

		// Script for person search
		private void RegisterScripts()
		{
			StringBuilder sbScript = new StringBuilder();
			sbScript.Append( "\n\n<script language=\"javascript\">\n" );
			sbScript.Append( "\tfunction openSearchWindow()\n" );
			sbScript.Append( "\t{\n" );
			sbScript.Append( "\t\tvar tPos = window.screenTop + 100;\n" );
			sbScript.Append( "\t\tvar lPos = window.screenLeft + 100;\n" );
			sbScript.AppendFormat( "\t\tdocument.frmMain.ihPersonListID.value = '{0}';\n", ihPersonList.ClientID ); sbScript.AppendFormat( "\t\tdocument.frmMain.ihRefreshButtonID.value = '{0}';\n", btnRefresh.ClientID );
			sbScript.Append( "\t\tvar searchWindow = window.open('Default.aspx?page=16','Search','height=400,width=600,resizable=yes,scrollbars=yes,toolbar=no,location=no,directories=no,status=no,menubar=no,top=' + tPos + ',left=' + lPos);\n" );
			sbScript.Append( "\t\tsearchWindow.focus();\n" );
			sbScript.Append( "\t}\n" );
			sbScript.Append( "</script>\n\n" );
			Page.ClientScript.RegisterClientScriptBlock( this.GetType(), "OpenSearchWindow", sbScript.ToString() );
		}

		// Called automatically by the search dialog after a person id has been selected.
		// The selected id will be in 'ihPersonList'.
		protected void btnRefresh_Click( object sender, EventArgs e )
		{
			// The search can possibly return multiple ids based on the module setting.
			if ( ihPersonList.Value != "" )
			{
				string[] personIds = ihPersonList.Value.Split( ',' );
				foreach ( string id in personIds )
				{
					txtPersonID.Text = id;
				}
				ihPersonList.Value = "";
			}
		}

		#endregion

		#region helper methods
		private CheckinLabel InitPrinterLabels()
		{
			CheckinLabel printerLabel = new CheckinLabel();

			OrganizationSettingData orgSettings = new OrganizationSettingData();

			printerLabel.ServicesTitle = orgSettings.GetOrganizationSetting( CurrentOrganization.OrganizationID, "Cccev.ServicesLabel" ); 
			printerLabel.Footer = orgSettings.GetOrganizationSetting( CurrentOrganization.OrganizationID, "Cccev.ClaimCardFooter" ); 
			printerLabel.AttendanceLabelTitle = orgSettings.GetOrganizationSetting( CurrentOrganization.OrganizationID, "Cccev.AttendanceLabelTitle" );
			printerLabel.ClaimCardTitle = orgSettings.GetOrganizationSetting( CurrentOrganization.OrganizationID, "Cccev.ClaimCardTitle" );
			printerLabel.HealthNotesTitle = orgSettings.GetOrganizationSetting( CurrentOrganization.OrganizationID, "Cccev.HealthNotesTitle" );
			printerLabel.ParentsInitialsTitle = orgSettings.GetOrganizationSetting( CurrentOrganization.OrganizationID, "Cccev.ParentsInitialsTitle" ); 
			printerLabel.BirthdayImageFile = orgSettings.GetOrganizationSetting( CurrentOrganization.OrganizationID, "Cccev.BirthdayImageFile" );
			printerLabel.LogoImageFile = orgSettings.GetOrganizationSetting( CurrentOrganization.OrganizationID, "Cccev.LogoImageFile" );

			printerLabel.CheckInDate = DateTime.Now;
			printerLabel.Services = txtServices.Text;
			printerLabel.FirstName = txtFirstName.Text;
			printerLabel.LastName = txtLastname.Text;
			printerLabel.FullName = txtFirstName.Text + " " + txtLastname.Text;
			printerLabel.SecurityToken = txtSecurityCode.Text;

			// Nametag stuff
			printerLabel.AgeGroup = txtAgeGroup.Text;
			printerLabel.BirthdayDate = DateTime.Parse( this.txtBirthday.Text );

			printerLabel.LegalNoteFlag = ( rblLegalNotes.SelectedValue.Equals( "true" ) ) ? true : false;
			printerLabel.SelfCheckOutFlag = ( rblSelfCheckOut.SelectedValue.Equals( "true" ) ) ? true : false;

			if ( txtHealthNotes.Text.Trim().Equals( "" ) )
			{
				printerLabel.HealthNoteFlag = false;
			}
			else
			{
				printerLabel.HealthNoteFlag = true;
				printerLabel.HealthNotes = txtHealthNotes.Text;
			}

			return printerLabel;
		}
		#endregion

	} // end class
} // end namespace