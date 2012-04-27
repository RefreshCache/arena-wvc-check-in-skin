/**********************************************************************
 * Description:	Lists Occurrence Type Attributes and allows add/edit/delete.
 * Created By:   Nick Airdo @ Central Christian Church of the East Valley
 * Date Created: 02/19/2008 11:46 AM
 *
 * $Workfile: OccurrenceTypeAttributeList.ascx.cs $
 * $Revision: 8 $ 
 * $Header: /trunk/Arena/UserControls/Custom/Cccev/Checkin/OccurrenceTypeAttributeList.ascx.cs   8   2009-10-08 10:18:31-07:00   JasonO $
 *  
 * $Log: /trunk/Arena/UserControls/Custom/Cccev/Checkin/OccurrenceTypeAttributeList.ascx.cs $
*  
*  Revision: 8   Date: 2009-10-08 17:18:31Z   User: JasonO 
*  Merging/updating to make changes for 1.2 release. 
*  
*  Revision: 7   Date: 2009-02-17 05:05:43Z   User: nicka 
*  removed old AbilityLevel drop down list 
*  
*  Revision: 6   Date: 2009-02-17 00:53:46Z   User: nicka 
*  Changed AbilityLevel property to a multiple value property 
*  (AbilityLevelLookupIDs) 
*  
*  Revision: 5   Date: 2009-01-22 20:31:03Z   User: nicka 
*  Changed module setting to use ListFromSqlSetting. Disable add button if 
*  there are no other attendance types available. 
*  
*  Revision: 4   Date: 2009-01-16 23:48:04Z   User: nicka 
*  
*  Revision: 3   Date: 2008-12-11 22:31:18Z   User: JasonO 
*  Removing age in months and grade level criteria. 
*  
*  Revision: 2   Date: 2008-09-23 16:08:53Z   User: nicka 
*  draft 2 
*  
*  Revision: 1   Date: 2008-02-19 23:46:22Z   User: nicka 
*  DRAFT 
 **********************************************************************/
using System;
using System.Data;
using System.Collections;
using System.Web.UI.WebControls;
using Arena.Core;
using Arena.Custom.Cccev.DataUtils;
using Arena.Portal;
using Arena.Security;
using Arena.Custom.Cccev.CheckIn.Entity;
using Arena.Custom.Cccev.CheckIn.DataLayer;
using System.Text;

namespace ArenaWeb.UserControls.Custom.Cccev.CheckIn
{
	public partial class OccurrenceTypeAttributeList : PortalControl
	{
		private bool _editEnabled;
		private int _groupID = Constants.NULL_INT;
		private OccurrenceTypeAttribute occurrenceTypeAttribute;
		private OccurrenceTypeCollection occurrenceTypes = new OccurrenceTypeCollection();
		private const string seperator = ", ";

		#region Module Settings

		[ListFromSqlSetting( "Ability Level Lookup Type", "Lookup type for the Checkin system 'Ability Levels'", true, "",
			"SELECT [lookup_type_id], [lookup_type_name] FROM [core_lookup_type] ORDER BY [lookup_type_name]" )]
		public string LookupTypeIDSetting { get { return Setting( "LookupTypeID", "", true ); } }

		#endregion

		protected void Page_Init( object sender, EventArgs e )
		{
			dgList.AddItem += dgList_AddItem;
			dgList.DeleteCommand += dgList_DeleteCommand;
			dgList.ReBind += dgList_ReBind;
			dgList.ItemCommand += dgList_ItemCommand;
			dgList.ItemDataBound += dgList_ItemDataBound;
			_groupID = Request.QueryString[ "Group" ] != null ? Convert.ToInt32( Request.QueryString[ "Group" ] ) : -1;
			occurrenceTypes = new OccurrenceTypeCollection( _groupID );
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			_editEnabled = CurrentModule.Permissions.Allowed( OperationType.Edit, CurrentUser );

			if ( !IsPostBack )
			{
				ShowList();
			}
		}

		private void ShowList()
		{
			// disable the add button if there are no more available occurrence types
			if ( GetAvailableOccurrenceTypes().Count == 0 )
			{
				dgList.AddEnabled = false;
			}
			else
			{
				dgList.AddEnabled = true;
			}

			pnlList.Visible = true;
			pnlEdit.Visible = false;
			dgList.EditEnabled = _editEnabled;
			OccurrenceTypeAttributeCollection table = new OccurrenceTypeAttributeCollection();
			table.GetOccurrenceTypeAttributeByGroupId( _groupID );
			dgList.DataSource = table;
			dgList.DataBind();
		}

		#region DataGrid Helper Methods

		protected string GetOccurrenceTypeName( int occurrenceTypeID )
		{
			return occurrenceTypes.FindByID( occurrenceTypeID ).TypeName;
		}

		protected string GetAbilityLevelName( int abilityLevelID )
		{
			string name = "any";

			if ( abilityLevelID != -1 )
			{
				Lookup abilityLevel = new Lookup( abilityLevelID, true );
				name = abilityLevel.Value;
			}
			return name;
		}

		#endregion

		#region Event Methods

		// Handle the click from the link button (when the grid is empty)
		protected void lbAdd_Click( object sender, EventArgs e )
		{
			ShowEdit( -1 );
		}

		// Add new item (from datagrid)
		void dgList_AddItem( object sender, EventArgs e )
		{
			ShowEdit( -1 );
		}

		// Needed for sorting.
		void dgList_ReBind( object sender, EventArgs e )
		{
			ShowList();
		}

		// Delete selected item
		void dgList_DeleteCommand( object source, DataGridCommandEventArgs e )
		{
			OccurrenceTypeAttribute.Delete( Int32.Parse( e.Item.Cells[ 0 ].Text ) );
			ShowList();
		}

		// Perform any custom actions on each row of datagrid
		private void dgList_ItemDataBound( object source, DataGridItemEventArgs e )
		{
			switch ( e.Item.ItemType )
			{
				case ListItemType.Item:
				case ListItemType.AlternatingItem:
				{
					OccurrenceTypeAttribute dataItem = (OccurrenceTypeAttribute)e.Item.DataItem;
					// Change the OccurrenceType ID to its name
					if ( dataItem.OccurrenceTypeAttributeId != Constants.NULL_INT )
					{
						e.Item.Cells[ 1 ].Text = GetOccurrenceTypeName( dataItem.OccurrenceTypeId );
					}

					StringBuilder sb = new StringBuilder();
					foreach ( int lookupID in dataItem.AbilityLevelLookupIDs )
					{
						sb.Append( GetAbilityLevelName( lookupID ) + seperator );
					}
					// clean off the trailing seperator
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - seperator.Length, seperator.Length);
                    }
				    e.Item.Cells[ 2 ].Text = sb.ToString();
					
					// Hide the checkbox if Is Special Needs is not true
					if (!dataItem.IsSpecialNeeds)
					{
						e.Item.Cells[ 3 ].Text = Constants.NULL_STRING;
					}

                    // Hide the checkbox if Is Room Blancing is not true
                    if (!dataItem.IsRoomBalancing)
                    {
                        e.Item.Cells[ 4 ].Text = Constants.NULL_STRING;
                    }

					break;
				}
			}
		}

		// Show edit dialog
		protected void dgList_ItemCommand( object source, DataGridCommandEventArgs e )
		{
			if ( e.CommandName == "EditItem" )
				ShowEdit( Int32.Parse( e.Item.Cells[ 0 ].Text ) );
		}

		/// <summary>
		/// Saves the item using the information from the form.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnUpdate_Click( object sender, EventArgs e )
		{
			SaveItem();
			ShowList();
		}

		// Cancel update/add
		protected void btnCancel_Click( object sender, EventArgs e )
		{
			ShowList();
		}

		#endregion

		#region Views

		// Displays add/edit dialog for an item.
		protected void ShowEdit( int occurrenceTypeAttributeID )
		{
			pnlList.Visible = false;
			pnlEdit.Visible = true;

			if ( occurrenceTypeAttributeID == -1 )
			{
				OccurrenceTypeCollection availableOccurrenceTypes = GetAvailableOccurrenceTypes();
				// exit if there are no more available occurrence types
				// (we should never get here)
				if ( availableOccurrenceTypes.Count == 0 )
					return;

				occurrenceTypeAttribute = new OccurrenceTypeAttribute();
				ddlOccurrenceType.Visible = true;
				ddlOccurrenceType.DataSource = availableOccurrenceTypes;
				ddlOccurrenceType.DataTextField = "TypeName";
				ddlOccurrenceType.DataValueField = "OccurrenceTypeId";
				ddlOccurrenceType.DataBind();
			}
			else
			{
				ddlOccurrenceType.Visible = false;
				LoadItem( occurrenceTypeAttributeID );
				lblOccurrenceType.Text = occurrenceTypes.FindByID( occurrenceTypeAttribute.OccurrenceTypeId ).TypeName;
			}

			// If the new object returns -1 as the id, then that means it was not found in the database.
			// If -1 was passed into this function, then that means "add new".
			// If the new object returns -1 but we passed in an id other than -1, then that means it was not found in the database.
			if ( occurrenceTypeAttributeID != -1 && occurrenceTypeAttribute.OccurrenceTypeAttributeId == -1 )
			{
				lblErrorMessage.Text = string.Format( "Could not find item with ID of {0}", occurrenceTypeAttributeID );
			}

			// Set defaults for add new
			if ( occurrenceTypeAttribute.OccurrenceTypeAttributeId == Constants.NULL_INT )
			{
				InitEditForm();
			}
			// Prepopulate values from database (edit mode)
			else
			{
				hfOccurrenceTypeAttributeID.Value = occurrenceTypeAttribute.OccurrenceTypeAttributeId.ToString();
				cbIsSpecialNeeds.Checked = occurrenceTypeAttribute.IsSpecialNeeds;
			    cbIsRoomBalancing.Checked = occurrenceTypeAttribute.IsRoomBalancing;
				txtLastnameStartingLetter.Text = occurrenceTypeAttribute.LastNameStartingLetter;
				txtLastnameEndingLetter.Text = occurrenceTypeAttribute.LastNameEndingLetter;
			}
			BindAbilityLevelsToDoubleListBox( occurrenceTypeAttribute );

			btnSave.Visible = _editEnabled;
			cbIsSpecialNeeds.Focus();
		}

		private void BindAbilityLevelsToDoubleListBox( OccurrenceTypeAttribute ota )
		{
			LookupCollection allAbilityLevels = new LookupCollection( Convert.ToInt32( LookupTypeIDSetting ) );
			LookupCollection availableAbilityLevels = new LookupCollection();
			LookupCollection selectedAbilityLevels = new LookupCollection();

			foreach ( Lookup abilityLevel in allAbilityLevels )
			{
				if ( ota.AbilityLevelLookupIDs.Contains( abilityLevel.LookupID ) )
				{
					selectedAbilityLevels.Add( abilityLevel );
				}
				else
				{
					availableAbilityLevels.Add( abilityLevel ); 
				}
			}

			dlbAbilityLevels.ListBoxRight.DataSource = selectedAbilityLevels;
			dlbAbilityLevels.ListBoxRight.DataTextField = "Value";
			dlbAbilityLevels.ListBoxRight.DataValueField = "LookupID";
			dlbAbilityLevels.ListBoxRight.DataBind();

			dlbAbilityLevels.ListBoxLeft.DataSource = availableAbilityLevels;
			dlbAbilityLevels.ListBoxLeft.DataTextField = "Value";
			dlbAbilityLevels.ListBoxLeft.DataValueField = "LookupID";
			dlbAbilityLevels.ListBoxLeft.DataBind();
		}

		private void InitEditForm()
		{
			lblOccurrenceType.Text = "";
			hfOccurrenceTypeAttributeID.Value = "-1";
			cbIsSpecialNeeds.Checked = false;
		    cbIsRoomBalancing.Checked = false;
			txtLastnameStartingLetter.Text = "";
			txtLastnameEndingLetter.Text = "";
		}

		/// <summary>
		/// Return an OccurrenceTypeCollection for the selected group after
		/// removing all the Occurrence Types that already have an OccurrenceTypeAttribute
		/// </summary>
		/// <returns>an OccurrenceTypeCollection</returns>
		private OccurrenceTypeCollection GetAvailableOccurrenceTypes()
		{
			DataTable table = new OccurrenceTypeAttributeData().GetOccurrenceTypeAttributeByGroupId_DT( _groupID );

			Hashtable check = new Hashtable( table.Rows.Count );
			foreach ( DataRow row in table.Rows )
			{
				if ( row[ "occurrence_type_id" ] != null )
				{
					check.Add( (int)row[ "occurrence_type_id" ], true );
				}
			}

			// Now get all the occurrenceTypes and create a filtered list
			// of all the ones that do not already have an OccurrenceTypeAttribute
			OccurrenceTypeCollection types = new OccurrenceTypeCollection( _groupID );
			OccurrenceTypeCollection listFiltered = new OccurrenceTypeCollection();

			foreach ( OccurrenceType type in types )
			{
				if ( ! check.ContainsKey( type.OccurrenceTypeId ) )
				{
					listFiltered.Add( type );
				}
			}

			return listFiltered;
		}

		#endregion

		private void LoadItem( int occurrenceTypeAttributeID )
		{
			occurrenceTypeAttribute = new OccurrenceTypeAttribute( occurrenceTypeAttributeID );
		}

		private void SaveItem()
		{
			if ( hfOccurrenceTypeAttributeID.Value == "-1" )
			{
				occurrenceTypeAttribute = new OccurrenceTypeAttribute();
				occurrenceTypeAttribute.OccurrenceTypeId = Convert.ToInt32( ddlOccurrenceType.SelectedValue );
			}
			else
			{
				LoadItem( Convert.ToInt32( hfOccurrenceTypeAttributeID.Value ) );
			}

			occurrenceTypeAttribute.IsSpecialNeeds = cbIsSpecialNeeds.Checked;
		    occurrenceTypeAttribute.IsRoomBalancing = cbIsRoomBalancing.Checked;
			occurrenceTypeAttribute.LastNameStartingLetter = txtLastnameStartingLetter.Text.ToUpper();
			occurrenceTypeAttribute.LastNameEndingLetter = txtLastnameEndingLetter.Text.ToUpper();

			// get the set ability levels
			occurrenceTypeAttribute.AbilityLevelLookupIDs.Clear();
			foreach ( ListItem item in dlbAbilityLevels.ListBoxRight.Items )
			{
				occurrenceTypeAttribute.AbilityLevelLookupIDs.Add( int.Parse( item.Value ) );
			}

			// save the object
			occurrenceTypeAttribute.Save( Page.User.Identity.Name );
		}

	}
}