/**********************************************************************
* Description:	Moves Occurrence Attendance records to a selected
*				Occurrence.
* Created By:	Nick Airdo
* Date Created:	2/18/2009 10:47:45 AM
*
* $Workfile: MoveAttendance.ascx.cs $
* $Revision: 3 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Checkin/MoveAttendance.ascx.cs   3   2009-02-19 10:06:29-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Checkin/MoveAttendance.ascx.cs $
*  
*  Revision: 3   Date: 2009-02-19 17:06:29Z   User: nicka 
*  Remember last Occurrence Type selection via Session 
*  
*  Revision: 2   Date: 2009-02-19 15:51:31Z   User: nicka 
*  
*  Revision: 1   Date: 2009-02-18 23:00:45Z   User: nicka 
*  Used to move attendance records from one occurrence to another. 
**********************************************************************/

namespace ArenaWeb.UserControls.Custom.Cccev.Checkin
{
	using System;
	using System.Data;
	using System.Configuration;
	using System.Collections;
	using System.Web;
	using System.Web.Security;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.WebControls.WebParts;
	using System.Web.UI.HtmlControls;

	using Arena.Core;
	using Arena.Portal;
	using Arena.Portal.UI;

	public partial class MoveAttendance : PortalControl
	{
		private int occurrenceID = -1;
		private Occurrence occurrence = null;
		private const string SESS_LASTOCCTYPE = "CccevMoveAttendance.LastSelectedOccType";

		#region Event Handlers

		protected void Page_Init( object sender, System.EventArgs e )
		{
			occurrenceID = Request.QueryString[ "OccurrenceID" ] != null ? Convert.ToInt32( Request.QueryString[ "OccurrenceID" ] ) : -1;
		}

		private void Page_Load( object sender, System.EventArgs e )
		{
			lblMessage.Text = "";
			if ( !IsPostBack )
			{
				BindOccurrenceTypes();
			}
		}

		protected void ddlOccurrenceTypes_SelectedIndexChanged( object sender, EventArgs e )
		{
			BindOccurrencesByType( int.Parse( ddlOccurrenceTypes.SelectedValue ) );

			Session[ SESS_LASTOCCTYPE ] = ddlOccurrenceTypes.SelectedValue;
		}


		protected void btnCancel_Click( object sender, EventArgs e )
		{
			upMoveAttendance.Visible = false;
			btnMoveAttendance.Visible = true;
		}

		protected void btnMoveRecords_Click( object sender, EventArgs e )
		{
			if ( ddlOccurrences.SelectedItem != null && ddlOccurrences.SelectedItem.Value != "-1" )
			{
				OccurrenceAttendanceCollection attendanceRecords = new OccurrenceAttendanceCollection( occurrenceID );
				int newOccurrenceID = int.Parse( ddlOccurrences.SelectedItem.Value );
				int count = 0;
				int errors = 0;
				foreach ( OccurrenceAttendance singleRecord in attendanceRecords )
				{
					try
					{
						singleRecord.OccurrenceID = newOccurrenceID;
						singleRecord.Save( CurrentUser.Identity.Name );
						count++;
					}
					catch
					{
						errors++;
					}
				}

				upMoveAttendance.Visible = false;
				btnMoveAttendance.Visible = true;

				lblMessage.Visible = true;
				lblMessage.Text = count.ToString() + " attendance records moved. ";
				if ( errors > 0 )
					lblMessage.Text += "<b>" + errors.ToString() + " were unable to be moved.</b>";
			}
		}

		protected void btnMoveAttendance_Click( object sender, EventArgs e )
		{
			lblMessage.Text = "";
			upMoveAttendance.Visible = true;
			btnMoveAttendance.Visible = false;
			BindOccurrenceTypes();
		}

		#endregion

		private void BindOccurrenceTypes()
		{
			occurrence = new Occurrence( occurrenceID );
			OccurrenceType type = new OccurrenceType( occurrence.OccurrenceTypeID );
			OccurrenceTypeCollection types = new OccurrenceTypeCollection( type.GroupId );

			ddlOccurrenceTypes.DataSource = types;
			ddlOccurrenceTypes.DataBind();
			ddlOccurrenceTypes.Items.Insert (0, new ListItem("- select -", "-1"  ) );

			if ( Session[ SESS_LASTOCCTYPE ] != null )
			{
				int occurrenceTypeID = int.Parse( (string) Session[ SESS_LASTOCCTYPE ] );
				ddlOccurrenceTypes.SelectedValue = occurrenceTypeID.ToString();
				BindOccurrencesByType( occurrenceTypeID );
			}
		}

		private void BindOccurrencesByType( int occurrenceTypeID )
		{
			occurrence = new Occurrence( occurrenceID );
			string bestFitValue = "-1";

			ddlOccurrences.Items.Clear();

			if ( occurrenceTypeID != -1 )
			{
				ListItemCollection items = new ListItemCollection();
				OccurrenceCollection occurrences = new OccurrenceCollection( occurrenceTypeID );
				ListItem item;

				foreach ( Occurrence occ in occurrences )
				{
					item = new ListItem();
					item.Value = occ.OccurrenceID.ToString();
					item.Text = string.Format( "{0} ({1} {2})", occ.Name, occ.StartTime.ToShortDateString(), occ.StartTime.ToShortTimeString() );
					if ( occurrence.StartTime.CompareTo( occ.StartTime ) == 0 )
						bestFitValue = occ.OccurrenceID.ToString();

					ddlOccurrences.Items.Add( item );
				}
				ddlOccurrences.Items.Insert( 0, new ListItem( "- select -", "-1" ) );

				ddlOccurrences.SelectedValue = bestFitValue;
				if ( bestFitValue != "-1" )
				{
					ddlOccurrences.SelectedItem.Text += " * best match";
					ddlOccurrences.SelectedItem.Attributes.Add( "style", "color: blue;" );
				}
			}
		}

} // end class
} // end namespace