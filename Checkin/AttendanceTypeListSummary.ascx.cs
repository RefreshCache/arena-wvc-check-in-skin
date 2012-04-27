/**********************************************************************
* Description:	Shows Attendance Types for a given GroupID
* Created By:   Nick Airdo @ Central Christian Church of the East Valley
* Date Created: 01/23/2009 08:42 PM
*
* $Workfile: AttendanceTypeListSummary.ascx.cs $
* $Revision: 7 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Checkin/AttendanceTypeListSummary.ascx.cs   7   2009-02-19 11:30:45-07:00   nicka $
*  
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Checkin/AttendanceTypeListSummary.ascx.cs $
*  
*  Revision: 7   Date: 2009-02-19 18:30:45Z   User: nicka 
*  Only show up to X weeks of OneTime frequency start times 
*  
*  Revision: 6   Date: 2009-02-19 18:23:14Z   User: nicka 
*  
*  Revision: 5   Date: 2009-02-17 00:53:46Z   User: nicka 
*  Changed AbilityLevel property to a multiple value property 
*  (AbilityLevelLookupIDs) 
*  
*  Revision: 4   Date: 2009-01-31 00:41:28Z   User: nicka 
*  Show summary by default if passed "show=true" in the querystring 
*  
*  Revision: 3   Date: 2009-01-30 23:59:53Z   User: nicka 
*  Show closed locations with red strike-through 
*  
*  Revision: 2   Date: 2009-01-26 01:50:03Z   User: nicka 
*  clear filter after hide/show and added refresh link button 
*  
*  Revision: 1   Date: 2009-01-25 00:13:04Z   User: nicka 
*  first working version 
 **********************************************************************/
using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Arena.Core;
using Arena.Portal;
using Arena.Portal.UI;
using Arena.Custom.Cccev.CheckIn.Entity;
using Arena.DataLayer.Core;
using Arena.Organization;
using System.Text;

namespace ArenaWeb.UserControls.Custom.Cccev.CheckIn
{
	public partial class AttendanceTypeListSummary : PortalControl
	{
		private int groupID = -1;
		private string timeFilter = string.Empty;
		private Dictionary<int, OccurrenceTypeAttribute> cachedOTAs = new Dictionary<int, OccurrenceTypeAttribute>();
		private string[] Days = { "Su", "Mo", "Tu", "We", "Th", "Fr", "Sa" };
		private const string seperator = ", ";
		private List<DataRowView> filteredList = new List<DataRowView>();
		private int OneTimeWindowWeeks = 3;

		protected void Page_Init( object sender, System.EventArgs e )
		{
			dgList.ReBind += new DataGridReBindEventHandler( dgList_ReBind );
			dgList.ItemDataBound += new DataGridItemEventHandler( dgList_ItemDataBound );
			groupID = Request.QueryString[ "Group" ] != null ? Convert.ToInt32( Request.QueryString[ "Group" ] ) : -1;
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( ! IsPostBack )
			{
				lblOneTimeFreqNotice.Text = string.Format( "note: only one-time frequency items that are within the upcoming {0} weeks are listed.<br/>", OneTimeWindowWeeks );
				BindStartTimes();
				// show summary automatically if given in the QueryString.
				if ( Request.QueryString[ "show" ] != null )
				{
					btnShowSummary_Click( sender, e );
				}

			}
		}

		/// <summary>
		/// Bind a list of "start times" (OccurrenceTypeTemplate's StartTime values)
		/// for all the AttendanceTypes (OccurrenceTypes) for this group (Attendance Type Category).
		/// </summary>
		private void BindStartTimes()
		{
			OccurrenceTypeCollection occurrenceTypes = new OccurrenceTypeCollection( groupID );
			OccurrenceTypeTemplateCollection frequencies;

			Dictionary<string, string> times = new Dictionary<string, string>();
			times.Add( "*", string.Empty );

			foreach ( OccurrenceType type in occurrenceTypes )
			{
				frequencies = new OccurrenceTypeTemplateCollection( type.OccurrenceTypeId );
				foreach ( OccurrenceTypeTemplate frequency in frequencies )
				{
					string time = frequency.StartTime.ToShortTimeString();

					if ( !times.ContainsKey( time ) )
						times.Add( time, time );
				}
			}

			ddlStartTimes.DataSource = times;
			ddlStartTimes.DataBind();
		}

		/// <summary>
		/// Show the Summary of all Attendance Types for the selected group.
		/// </summary>
		private void ShowList()
		{
			dgList.DataSource = new OccurrenceTypeData().GetOccurrenceTypeByGroupId_DT( groupID );
			dgList.DataBind();
		}

		/// <summary>
		/// Cache the OccurrenceTypeAttributes for the given attendanceTypeID
		/// </summary>
		/// <param name="attendanceTypeID"></param>
		private void CacheOTAs( int attendanceTypeID )
		{
			OccurrenceTypeAttributeCollection otas = new OccurrenceTypeAttributeCollection( attendanceTypeID );
			foreach ( OccurrenceTypeAttribute ota in otas )
			{
				cachedOTAs.Add( ota.OccurrenceTypeId, ota );
			}
		}

		#region Event Methods
		protected void ddlStartTimes_Changed( object sender, EventArgs e )
		{
			timeFilter = ddlStartTimes.SelectedValue;
			dgList.DataSource = new OccurrenceTypeData().GetOccurrenceTypeByGroupId_DT( groupID );
			dgList.DataBind();
		}

		protected void btnShowSummary_Click( object sender, EventArgs e )
		{
			if ( !pnlList.Visible )
			{
				pnlList.Visible = true;
				btnShowSummary.Text = "Hide Summary";
				ShowList();
			}
			else
			{
				btnShowSummary.Text = "Show Summary";
				pnlList.Visible = false;
				ddlStartTimes.SelectedIndex = -1;
				timeFilter = string.Empty;
			}
		}

		protected void btnRefreshSummary_Click( object sender, EventArgs e )
		{
			ddlStartTimes_Changed( sender, e );
		}

		// Needed for sorting.
		void dgList_ReBind( object sender, EventArgs e )
		{
			ShowList();
		}

		/// <summary>
		/// Used to custom bind related stuff such as the OccurrenceTypeAttributes, locations, etc.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void dgList_ItemDataBound( object source, DataGridItemEventArgs e )
		{
			switch ( e.Item.ItemType )
			{
				case ListItemType.Item:
				case ListItemType.AlternatingItem:
				{
					DataRowView dataItem = (DataRowView)e.Item.DataItem;
					int attendanceTypeID = (int)dataItem["occurrence_type_id"];
					CacheOTAs( attendanceTypeID  );

					// set the start time column value(s)
					OccurrenceTypeTemplateCollection startTimes = new OccurrenceTypeTemplateCollection( attendanceTypeID );
					StringBuilder sb = new StringBuilder();
					bool matchTime = false;
					bool oneTimeFlag = false;
					foreach ( OccurrenceTypeTemplate ott in startTimes )
					{
						switch (ott.OccurrenceFreqType)
						{
							case OccurrenceFrequencyType.Daily:
								sb.Append( "* " );
								sb.Append( ott.StartTime.ToShortTimeString() + seperator );
								break;
							case OccurrenceFrequencyType.Monthly:
								sb.Append( "/" + ott.FreqQualifier + "/ ");
								sb.Append( ott.StartTime.ToShortTimeString() + seperator );
								break;
							case OccurrenceFrequencyType.OneTime:
								oneTimeFlag = true;
								DateTime ottDate = DateTime.Parse( ott.FreqQualifier );
								if ( DateTime.Now.AddDays( -1 ) <= ottDate && ottDate <= DateTime.Now.AddDays( 7 * OneTimeWindowWeeks ) )
								{
									sb.Append( ott.FreqQualifier + " " );
									sb.Append( ott.StartTime.ToShortTimeString() + seperator );
								}
								break;
							case OccurrenceFrequencyType.Undefined:
								sb.Append( ott.StartTime.ToShortTimeString() + seperator );
								break;
							case OccurrenceFrequencyType.Weekly:
								sb.Append( Days[int.Parse( ott.FreqQualifier) ] + " ");
								sb.Append( ott.StartTime.ToShortTimeString() + seperator );
								break;
							default:
								break;
						}

						// Filter by time if the filter is set
						if ( matchTime != true && ott.StartTime.ToShortTimeString() == timeFilter )
						{
							matchTime = true;
						}

						lblOneTimeFreqNotice.Visible = oneTimeFlag;

					}
					// clean off the trailing seperator
					if ( sb.Length > 0 )
						sb.Remove( sb.Length - seperator.Length, seperator.Length );
					e.Item.Cells[ 2 ].Text = sb.ToString();

					// Filter by time if the FILTER IS SET and we did not find a matching time
					// above when we were looping through all the occurrence type templates
					if ( !timeFilter.Equals( string.Empty ) && !matchTime )
						e.Item.Visible = false;
					else
						filteredList.Add( dataItem );

					// set the age column value
					if ( (decimal)dataItem[ "max_age" ] != 0 )
					{
						e.Item.Cells[ 3 ].Text = String.Format( "{0}-{1}", (decimal)dataItem[ "min_age" ], (decimal)dataItem[ "max_age" ] );
					}

					// set the grade column value
					if ( (int)dataItem[ "min_grade" ] != -1 && (int)dataItem[ "max_grade" ] != -1 )
					{
						if ( (int)dataItem[ "min_grade" ] == (int)dataItem[ "max_grade" ] )
							e.Item.Cells[ 4 ].Text = String.Format( "{0}", (int)dataItem[ "min_grade" ] ).Replace( '0', 'K' );
						else
							e.Item.Cells[ 4 ].Text = String.Format( "{0}-{1}", (int)dataItem[ "min_grade" ], (int)dataItem[ "max_grade" ] ).Replace( '0', 'K');
					}

					// set the extended, Attribute column values
					if ( cachedOTAs.ContainsKey( attendanceTypeID ) )
					{
						OccurrenceTypeAttribute ota = cachedOTAs[ attendanceTypeID ];

						// set the Ability level column value
						if ( ota.AbilityLevelLookupIDs.Count == 0 )
						{
							e.Item.Cells[ 5 ].Text = "";
						}
						else
						{
							sb = new StringBuilder();
							foreach( int abilityLevelID in ota.AbilityLevelLookupIDs )
							{
								sb.Append( new Lookup( abilityLevelID, true ) + seperator );
							}
							// clean off the trailing seperator
							if ( sb.Length > 0 )
								sb.Remove( sb.Length - seperator.Length, seperator.Length );

							e.Item.Cells[ 5 ].Text = sb.ToString();
						}

						// set the Special Needs column value
						if ( !ota.IsSpecialNeeds )
						{
							e.Item.Cells[ 6 ].Text = "";
						}

						// set the lastname letter range
						string lastNameRange = String.Format( "{0}-{1}", ota.LastNameStartingLetter, ota.LastNameEndingLetter );
						if ( lastNameRange.Length > 1 )
							e.Item.Cells[ 8 ].Text = lastNameRange;
						else
							e.Item.Cells[ 8 ].Text = "";
					}
					else
					{
						e.Item.Cells[ 6 ].Text = "";
					}

					// set the Gender preference column
					if ( (int)dataItem[ "gender_preference" ] != 2  )
					{
						e.Item.Cells[ 7 ].Text = (int)dataItem[ "gender_preference" ] == 1 ? "F" : "M";
					}

					// set the Location/Rooms column value
					sb = new StringBuilder();
					LocationCollection locations = new LocationCollection();
					locations.LoadByOccurrenceTypeID( attendanceTypeID );
					foreach ( Location location in locations )
					{
						if ( location.RoomClosed )
						{
							sb.Append( "<del style='text-decoration: line-through; color: red;' title='closed'><span style='color:#000'>" + location.LocationName + "</span></del>" + seperator );
						}
						else
						{
							sb.Append( location.LocationName + seperator );
						}
					}
					if ( sb.Length > 0 )
						sb.Remove( sb.Length - seperator.Length, seperator.Length );

					e.Item.Cells[ 9 ].Text = sb.ToString();
					break;
				}
			}
		}

		/// <summary>
		/// Here we will attempt to look for check-in gaps.
		/// </summary>
		private void Page_PreRender()
		{
			//if ( !timeFilter.Equals( string.Empty ) )
			//{
			//    lblWarning.Text = "warning";
			//    lblWarning.Visible = true;
			//    lblWarning.BackColor = System.Drawing.Color.Yellow;
				
			//    // Find age gaps
			//    foreach ( DataRowView item in filteredList )
			//    {
			//        //
			//    }
			//}
			//else
			//{
			//    lblWarning.Visible = false;
			//}
		}
		#endregion

	}
}