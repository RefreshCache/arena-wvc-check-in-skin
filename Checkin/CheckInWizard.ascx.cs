/**********************************************************************
* Description:	Manages Checkin process
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	11/12/2008
*
* $Workfile: CheckInWizard.ascx.cs $
* $Revision: 68 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Checkin/CheckInWizard.ascx.cs   68   2010-11-17 14:17:00-07:00   JasonO $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Checkin/CheckInWizard.ascx.cs $
*  
*  Revision: 68   Date: 2010-11-17 21:17:00Z   User: JasonO 
*  
*  Revision: 67   Date: 2010-11-17 21:06:50Z   User: JasonO 
*  
*  Revision: 66   Date: 2010-11-09 18:52:24Z   User: JasonO 
*  
*  Revision: 65   Date: 2010-11-09 00:27:50Z   User: JasonO 
*  Committing iOS changes. 
*  
*  Revision: 64   Date: 2010-11-08 18:25:23Z   User: JasonO 
*  Cleaning up client-side code. 
*  
*  Revision: 63   Date: 2010-11-05 00:50:17Z   User: nicka 
*  Made session variables constants, fixed exception on first use bug (#351), 
*  centered all lists/grids. 
*  
*  Revision: 62   Date: 2010-11-04 20:04:37Z   User: JasonO 
*  More cleanup. 
*  
*  Revision: 61   Date: 2010-11-04 20:03:41Z   User: JasonO 
*  Cleaning up BuildResultString method. 
*  
*  Revision: 60   Date: 2010-11-04 20:02:54Z   User: JasonO 
*  Cleaning up BuildResultString code. 
*  
*  Revision: 59   Date: 2010-11-03 22:22:07Z   User: JasonO 
*  Refactoring to bring more data regarding results of check in process out to 
*  UI level. 
*  
*  Revision: 58   Date: 2010-11-02 19:17:42Z   User: JasonO 
*  Implementing post-checkin page redirect from redmine case 336 
*  (http://bit.ly/aFgtyl). 
*  
*  Revision: 57   Date: 2010-11-02 00:37:43Z   User: nicka 
*  Update for issues #343 #344 #349. 
*  
*  Revision: 56   Date: 2010-09-23 20:54:02Z   User: JasonO 
*  Implementing changes suggested by HDC. 
*  
*  Revision: 55   Date: 2010-06-28 17:28:51Z   User: JasonO
*  Bug Fix: If an occurrence attendance somehow existst (via multiple 
*  checkins?) we'll now check whether the "Attended" bit is set to false 
*  before allowing the family member to continue. 
*  
*  Revision: 54   Date: 2010-01-20 22:43:25Z   User: JasonO 
*  Adding support for declaring print-provider at the module level.
*  
*  Revision: 53   Date: 2010-01-19 23:18:04Z   User: JasonO 
*  
*  Revision: 52   Date: 2009-11-16 20:17:54Z   User: JasonO 
*  Refactoring 
*  
*  Revision: 51   Date: 2009-11-12 18:17:57Z   User: JasonO 
*  
*  Revision: 50   Date: 2009-11-11 17:43:32Z   User: JasonO 
*  Moving jQuery include code to into Page_Init event. 
*  
*  Revision: 49   Date: 2009-11-05 19:59:15Z   User: JasonO 
*  
*  Revision: 48   Date: 2009-10-28 17:00:32Z   User: JasonO 
*  Merging changes from HDC 
*  
*  Revision: 47   Date: 2009-10-27 22:48:50Z   User: JasonO 
*  Fixing jQuery issue. 
*  
*  Revision: 46   Date: 2009-10-27 16:56:42Z   User: JasonO 
*  
*  Revision: 45   Date: 2009-10-21 16:47:14Z   User: JasonO 
*  Adding module setting to ignore checkin start time. 
*  
*  Revision: 44   Date: 2009-10-08 17:18:31Z   User: JasonO 
*  Merging/updating to make changes for 1.2 release. 
*  
*  Revision: 43   Date: 2009-09-23 22:37:42Z   User: JasonO 
*  
*  Revision: 42   Date: 2009-09-09 16:17:35Z   User: JasonO 
*  Fixing potential lexical closure issues with Linq. 
*  
*  Revision: 41   Date: 2009-09-08 22:58:54Z   User: JasonO 
*  Updating class/object names to fix ambiguity issues. 
*  
*  Revision: 40   Date: 2009-09-01 21:46:17Z   User: nicka 
*  Fixed bug #24512 whereby LINQ query was executed for each reference. 
*  
*  Revision: 39   Date: 2009-07-15 18:17:09Z   User: JasonO 
*  
*  Revision: 38   Date: 2009-06-26 00:20:43Z   User: JasonO 
*  
*  Revision: 37   Date: 2009-06-25 22:28:28Z   User: JasonO 
*  Fixing ambiguous reference between Checkin.Constants and 
*  DataUtils.Constants. 
*  
*  Revision: 36   Date: 2009-06-18 18:38:20Z   User: nicka 
*  Changes to handle new IPrintLabel that requires kiosk as discussed here: 
*  http://checkinwizard.codeplex.com/Thread/View.aspx?ThreadId=57675 
*  
*  Revision: 35   Date: 2009-06-08 18:38:43Z   User: JasonO 
*  Implementing reSharper recommendations. 
*  
*  Revision: 34   Date: 2009-06-05 00:12:46Z   User: JasonO 
*  Updating SetOccurrences() method to use CheckInController.GetCurrentKiosk(). 
**********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Arena.Custom.Cccev.FrameworkUtils.FrameworkConstants;
using Arena.Portal;
using Arena.Computer;
using Arena.Core;
using Arena.Custom.Cccev.CheckIn;
using Arena.Custom.Cccev.CheckIn.Entity;
using Arena.Custom.Cccev.DataUtils;

namespace ArenaWeb.UserControls.Custom.Cccev.Checkin
{
    public partial class CheckInWizard : PortalControl
    {
        #region Module Settings

        [BooleanSetting("Auto Cancel/Confirm", "Automatically cancels or confirms after the set time intervals elapse.", false, true)]
        public string AutoCancelConfirmSetting { get { return Setting("AutoCancelConfirm", "true", false); } }

        [TextSetting("CSS Relative Path", "Relative path to custom CSS file for Check-In module.", false)]
        public string CssPathSetting { get { return Setting("CssPath", "UserControls/Custom/Cccev/Checkin/Misc/checkin.css", false); } }

        [TextSetting("Background Image Relative Path", "Relative path to background image for Init State.", false)]
        public string BackgroundImagePathSetting { get { return Setting("BackgroundImagePath", "UserControls/Custom/Cccev/Checkin/images/default_checkin_splash.png", false); } }

        [NumericSetting("Auto Refresh Time (Long)", "Time in seconds for page to refresh if left inactive. Defaults to 120 seconds.", false)]
        public string LongRefreshTimeSetting { get { return Setting("LongRefreshTime", "120", false); } }

        [NumericSetting("Auto Refresh Time (Short)", "Time in seconds for page to refresh if left inactive (Specific to Init/Scan and Confirm screens). Defaults to 10 seconds.", false)]
        public string ShortRefreshTimeSetting { get { return Setting("ShortRefreshTime", "10", false); } }

        [ListFromSqlSetting("Relationship Type List", "Allowable relationship types to check in.", false, "",
            "SELECT [relationship_type_id], [relationship] FROM [core_relationship_type] ORDER BY [relationship_order]",
            ListSelectionMode.Multiple)]
        public string RelationshipTypeIDsSetting { get { return Setting("RelationshipTypeIDs", "", false); } }

        [NumericSetting("Look Ahead Hours", "Number of hours to look ahead for occurrences (defaults to 2).", false)]
        public string LookAheadHoursSetting { get { return Setting("LookAheadHours", "2", false); } }

        [NumericSetting("Look Ahead Minutes", "Number of minutes to look ahead for occurrences (defaults to 0).", false)]
        public string LookAheadMinutesSetting { get { return Setting("LookAheadMinutes", "0", false); } }

        [TextSetting("Event is Closed Message", "Message to show when event is closed.", false)]
        public string EventIsClosedMessageSetting { get { return Setting("EventIsClosedMessage", "The event is closed.", false); } }

        [TextSetting("No Registered Occurrences Message", "Message to show when no occurrences are available to check in.", false)]
        public string NoOccurrenceMessageSetting { get { return Setting("NoOccurrenceMessage", "No registered occurrences.", false); } }

        [TextSetting("Scan Now Message", "Message to show it is OK for people to try scanning their code.", false)]
        public string ScanNowMessageSetting { get { return Setting("ScanNowMessage", "Scan Now", false); } }

        [TextSetting("Search By Phone Message", "Message to show on button to search by phone.", false)]
        public string SearchByPhoneMessageSetting { get { return Setting("SearchByPhoneMessage", "Search By Phone", false); } }

        [BooleanSetting("Allow Scan By Phone", "Controls whether or not a person can search by phone.  Defaults to true.", true, true)]
        public string AllowScanByPhoneSetting { get { return Setting("AllowScanByPhone", "true", false); } }

        [NumericSetting("Minimum Phone Number Length", "Minimum length for phone number searches (defaults to 10).", false)]
        public string PhoneLengthSetting { get { return Setting("PhoneLength", "10", false); } }
        
        // #343
        [NumericSetting("Maximum Phone Number Length", "Maximum length for phone number searches (defaults to 10).", false)]
        public string PhoneLengthMaxSetting { get { return Setting("PhoneLengthMax", "10", false); } }

        // #344
        [NumericSetting( "Minimum Age", "Minimum age of child who can check in. Note: Grade check supersedes the age check.  Leave blank for none.", false )]
        public string MinimumAgeSetting { get { return Setting( "MinimumAge", "-1", false ); } }

        [NumericSetting( "Maximum Age", "Maximum age of child who can check in.  Note: Grade check supersedes the age check.  Leave blank for none.", false )]
        public string MaximumAgeSetting { get { return Setting("MaximumAge", "-1", false); } }

        // #344
        [NumericSetting( "Minimum Grade", "Minimum grade of child who can check in. Leave blank for none.", false )]
        public string MinimumGradeSetting { get { return Setting( "MinimumGrade", "-1", false ); } }

        [NumericSetting("Maximum Grade", "Maximum grade of child who can check in. Default is 6.", false)]
        public string MaximumGradeSetting { get { return Setting("MaximumGrade", "6", false); } }

        [BooleanSetting("Require Attendee Abilities", "Determines whether or not to show view to set ability person attribute.", true, true)]
        public string GetAttendeeAbilitiesSetting { get { return Setting("GetAttendeeAbilities", "true", true); } }

        [TextSetting("No Eligible People for CheckIn", "Message to display when there are no family members eligible for check in.", false)]
        public string NoEligibleCheckInPeopleSetting { get { return Setting("NoEligibleCheckInPeople", "No Eligible Family Members For Check In", false); } }

        [ListFromSqlSetting("Ability Level Lookup Type", "Sets lookup type of ability person attribute.", true, "",
            "SELECT [lookup_type_id], [lookup_type_name] FROM [core_lookup_type] ORDER BY [lookup_type_name]")]
        public string AbilityLevelLookupTypeIDSetting { get { return Setting("AbilityLevelLookupTypeID", "", true); } }

        [ListFromSqlSetting("Ability Level Attribute", "Sets ability level person attribute.", true, "",
            "SELECT [attribute_id], [attribute_name] FROM [core_attribute] WHERE [attribute_type] = 3 AND [attribute_group_id] = 16 ORDER BY [attribute_name]")]
        public string AbilityLevelAttributeIDSetting { get { return Setting("AbilityLevelAttributeID", "", true); } }

        [NumericSetting("Max Ability Level Age", "Max Age to display Ability Level selector (defaults to 3.5).", false)]
        public string MaxAbilityLevelAgeSetting { get { return Setting("MaxAbilityLevelAge", "3.5", false); } }

        [ListFromSqlSetting("Special Needs Attribute", "Sets special needs person attribute.", true, "",
            "SELECT [attribute_id], [attribute_name] FROM [core_attribute] WHERE [attribute_type] = 4 AND [attribute_group_id] = 16 ORDER BY [attribute_name]")]
        public string SpecialNeedsAttributeIDSetting { get { return Setting("SpecialNeedsAttributeID", "", true); } }

        [TextSetting("Bad Kiosk Message", "Message to display if kiosk has not been registered in Arena.", false)]
        public string BadKioskTextSetting { get { return Setting("BadKioskText", "This kiosk is not currently registered.<br /> Please contact an Arena Administrator.", false); } }

        [TextSetting("Unavailable Occurrences Message", "Message to display if somebody is attempting to check into a service time with no available classes.", false)]
        public string UnavailableOccurrencesTextSetting { get { return Setting("UnavailableOccurrencesText", "Please see Children's Adminstrator at the Children's Center.", false); } }

        [TextSetting("Asynchronous Timeout Error Message", "JavaScript error message to display if asynchronous post times out on the server.", false)]
        public string AsyncTimeoutErrorMessageSetting { get { return Setting("AsyncTimeoutErrorMessage", "Request timed out on the server. Please proceed to your classrooms for assistance.", false); } }

        [PageSetting("Family Registration Page", "The location of the family registration wizard you wish to use. Activated when CTRL-SHIFT-R is pressed.", false)]
        public string FamilyRegistrationPageSetting { get { return Setting("FamilyRegistrationPage", "", false); } }

        [SmartPageSetting("Kiosk Management Page", "The location of the kiosk management page. Activated when CTRL-SHIFT-M is pressed.", "UserControls/Custom/Cccev/Checkin/KioskAdmin.ascx", RelatedModuleLocation.Beneath, false)]
        public string KioskManagementPageSetting { get { return _KioskManagementPageSetting; } set { _KioskManagementPageSetting = value; } }
        protected string _KioskManagementPageSetting;

        [BooleanSetting("Ignore Check-In Start", "If set to 'true', the system will allow people to check into future services (ie: services for which the Check-In Start Time has not yet occurred).", false, true)]
        public string IgnoreCheckInStartSetting { get { return Setting("IgnoreCheckInStart", "true", false); } }

        [NumericSetting("Page Timeout", "Time in seconds it takes for a request to the server to time out (defaults to 60).", false)]
        public string PageTimeoutSetting { get { return Setting("PageTimeout", "60", false); } }

        [LookupSetting("Label Print Provider", "Overrides the default print provider for printing name tags.", true, SystemGuids.CHECKIN_PRINT_PROVIDER_LOOKUP_TYPE_STRING)]
        public string PrintProviderSetting { get { return Setting("PrintProvider", "", true); } }

		[PageSetting("Post-Check In Redirect Page", "If set, the system will redirect to this page once the Check In process completes.", false)]
		public string PostCheckinRedirectPageSetting { get { return Setting("PostCheckinRedirectPage", "", false); } }

        #endregion

        #region Private Members

        private CheckInStates state = CheckInStates.Init;
        private List<Occurrence> occurrences;
        private int timeout;

        protected const string EMPTY_CHECK_BOX_IMG_URL = "images/empty_checkbox.png";
        protected const string CHECK_BOX_IMG_URL = "images/checkbox.png";

        #endregion

        #region Page Events

        protected void Page_Init(object sender, EventArgs e)
        {
#if !DEBUG
            // Set page timeout independently of server
            // NOTE: To test this, make sure debug is disabled in web.config
            if (PageTimeoutSetting.Trim() != Constants.NULL_STRING)
            {
                timeout = Server.ScriptTimeout;
                Server.ScriptTimeout = int.Parse(PageTimeoutSetting);
            }
#endif

            smpScripts.Scripts.Add(new ScriptReference(string.Format("~/{0}", BasePage.JQUERY_INCLUDE)));
            smpScripts.Scripts.Add(new ScriptReference("~/UserControls/Custom/Cccev/CheckIn/misc/jquery.jscrollpane.js"));
            smpScripts.Scripts.Add(new ScriptReference("~/UserControls/Custom/Cccev/CheckIn/misc/checkin-core.js"));
            Page.Header.Controls.Add(new LiteralControl("<link type=\"text/css\" rel=\"stylesheet\" href=\"UserControls/Custom/Cccev/CheckIn/misc/jquery.jscrollpane.css\" />"));
            Page.Header.Controls.Add(new LiteralControl(string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\" />", CssPathSetting)));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GetState();

            // These method calls depend on state.
            ResetSession();
            SetOccurrences();

            if (!Page.IsPostBack)
            {
                ShowView();
                Session[CheckInConstants.SESS_AUTO_ADVANCE] = true;
                btnRedirect.Style.Add("visibility", "hidden");
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
#if !DEBUG
            // Reset server timeout to prevent the rest of the application's timeout
            // from being changed permanently
            if (PageTimeoutSetting.Trim() != Constants.NULL_STRING)
            {
                Server.ScriptTimeout = timeout;
            }
#endif
        }

        protected void Redirect_Click(object sender, EventArgs e)
        {
            state = CheckInStates.Init;
            Session[CheckInConstants.SESS_STATE] = state;
            ResetError();
            ShowView();
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            switch (state)
            {
                case CheckInStates.Init:
                case CheckInStates.FamilySearch:
                case CheckInStates.SelectFamilyMember:
                case CheckInStates.NoEligiblePeople:
                    Session[CheckInConstants.SESS_AUTO_ADVANCE] = true;
                    state = CheckInStates.Init;
                    Session[CheckInConstants.SESS_STATE] = state;

					if (!string.IsNullOrEmpty(PostCheckinRedirectPageSetting))
					{
						Response.Redirect(string.Format("default.aspx?page={0}&sessionid=-1", PostCheckinRedirectPageSetting));
					}

                    break;
                case CheckInStates.SelectAbility:
                case CheckInStates.SelectService:
                case CheckInStates.Confirm:
                    Session[CheckInConstants.SESS_AUTO_ADVANCE] = false;
                    ResetFamily();
                    state = CheckInStates.SelectFamilyMember;
                    Session[CheckInConstants.SESS_STATE] = state;
                    break;
                default:
                    Session[CheckInConstants.SESS_AUTO_ADVANCE] = true;
                    ResetFamily();
                    state = CheckInStates.SelectFamilyMember;
                    Session[CheckInConstants.SESS_STATE] = state;
                    break;
            }

            ResetError();
            ShowView();
        }

        #endregion

        #region Private Methods

        private void GetState()
        {
            if (Session[CheckInConstants.SESS_STATE] != null && Page.IsPostBack)
                state = (CheckInStates)Enum.Parse(typeof(CheckInStates), Session[CheckInConstants.SESS_STATE].ToString());
            else
                state = CheckInStates.Init;
        }

        /// <summary>
        /// Resets session variables so invalid information is not passed between checkin attendees
        /// </summary>
        private void ResetSession()
        {
            if (state == CheckInStates.Init)
            {
                // Reset session values for next person to check in.
                // Only done if in init state.
				Session[CheckInConstants.SESS_AUTO_ADVANCE] = true;		// #351
                Session[CheckInConstants.SESS_LIST_OCCURRENCES_CHECKIN] = null;
                Session[CheckInConstants.SESS_FAMILY] = null;
                Session[CheckInConstants.SESS_LIST_CHECKIN_FAMILYMEMBERS] = null;

                ResetFamily();
            }
        }

        private void ResetFamily()
        {
            Session[CheckInConstants.SESS_ATTENDEES] = null;
            Session[CheckInConstants.SESS_SERVICE_TIMES] = null;
            Session[CheckInConstants.SESS_KEY_PEOPLEMAP] = null;
            Session[CheckInConstants.SESS_RESULTS] = null;
            Session[CheckInConstants.SESS_UNAVAILABLE_OCCURRENCES] = null;
            Session[CheckInConstants.SESS_TOTAL_OCCURRENCES] = null;
            ihAttendeesToProcess.Value = Constants.NULL_STRING;
            ihAttendeeIDs.Value = Constants.NULL_STRING;
        }

        /// <summary>
        /// Sets the occurrences session variable based on the occurrences available to the host kiosk.
        /// </summary>
        private void SetOccurrences()
        {
            if (state == CheckInStates.Init || state == CheckInStates.BadKiosk)
            {
                DateTime lookAhead = DateTime.Now;
                lookAhead = lookAhead.AddHours(double.Parse(LookAheadHoursSetting));
                lookAhead = lookAhead.AddMinutes(double.Parse(LookAheadMinutesSetting));
                ComputerSystem computer = CheckInController.GetCurrentKiosk(Request.ServerVariables["REMOTE_ADDR"]);
				
                if (computer != null)
                {
					Session[CheckInConstants.SESS_KIOSK] = computer;
                    occurrences = CheckInController.GetOccurrences(lookAhead, DateTime.Now, computer);
                    Session[CheckInConstants.SESS_LIST_OCCURRENCES_CHECKIN] = occurrences;
                }
                else
                {
                    state = CheckInStates.BadKiosk;
                    Session[CheckInConstants.SESS_STATE] = state;
                }
            }
            else
                occurrences = (List<Occurrence>)Session[CheckInConstants.SESS_LIST_OCCURRENCES_CHECKIN];
        }

        /// <summary>
        /// Hides all views then shows the appropriate view based on state information stored in session.
        /// </summary>
        private void ShowView()
        {
            HideViews();

            switch (state)
            {
                case CheckInStates.FamilySearch:
                    ShowFamilySearch();
                    break;
                case CheckInStates.SelectFamilyMember:
                    ShowSelectFamilyMember();
                    break;
                case CheckInStates.NoEligiblePeople:
                    ShowNoEligiblePeople();
                    break;
                case CheckInStates.SelectAbility:
                    ShowSelectAbility();
                    break;
                case CheckInStates.SelectService:
                    ShowSelectService();
                    break;
                case CheckInStates.Confirm:
                    ShowConfirm();
                    break;
                case CheckInStates.Result:
                    ShowResult();
                    break;
                case CheckInStates.BadKiosk:
                    ShowBadKiosk();
                    break;
                default:
                    ShowInit();
                    break;
            }
        }

        /// <summary>
        /// Hides all panels except for the title.
        /// </summary>
        private void HideViews()
        {
            foreach (Control control in upCheckin.ContentTemplateContainer.Controls)
            {
                if (control is Panel && control.ID != "pnlError")
                    control.Visible = false;
            }
        }

        private void ShowError(string errorText)
        {
            lblError.Text = errorText;
            lblError.CssClass = "errorText";
            lblError.Visible = true;
            pnlError.Style.Add("text-align", "center");
            pnlError.Visible = true;
        }

        private void ResetError()
        {
            lblError.Text = Constants.NULL_STRING;
            lblError.Visible = false;
            pnlError.Style.Clear();
        }

        #endregion

        #region Init View

        private void ShowInit()
        {
            pnlInit.Visible = true;
            pnlInit.Style.Add("vertical-align", "bottom");
            pnlInit.Style.Add("background-image", BackgroundImagePathSetting);
            btnScan.Style.Add("visibility", "hidden");
            const string errorText = "Invalid Scan!";

            if (tbScan.Text.Trim() != Constants.NULL_STRING)
            {
                try
                {
                    // Assuming valid barcode is read, access factory method in Controller to instantiate family
                    FamilyCollection families = CheckInController.GetFamily(CheckInSearchTypes.Scanner, tbScan.Text);
                    Session[CheckInConstants.SESS_FAMILY] = families[0];

                    if (families[0].FamilyID != Constants.NULL_INT)
                    {
                        state = CheckInStates.SelectFamilyMember;
                        Session[CheckInConstants.SESS_STATE] = state;
                        ShowView();
                    }
                    else
                    {
                        lblScanNow.Text = errorText;
                        lblScanNow.CssClass = "footerError";
                        lblScanNow.Visible = true;
                        pnlSwipeCard.Visible = true;
                    }
                }
                catch (FormatException)
                {
                    lblScanNow.Text = errorText;
                    lblScanNow.CssClass = "footerError";
                    lblScanNow.Visible = true;
                    pnlSwipeCard.Visible = true;
                }
            }
            else
            {
                SetCountDownTimer();
            }

            tbScan.Focus();
            tbScan.Text = Constants.NULL_STRING;
        }

        private void SetCountDownTimer()
        {
            try
            {
                Occurrence occurrence = occurrences.First();
                divWideFooter.Visible = false;

                if (CheckInController.ReadyForCheckIn(occurrence))
                {
                    divTimer.Visible = false;
                    divLeftFooter.Visible = true;
                    lblScanNow.Text = ScanNowMessageSetting;
                    lblScanNow.CssClass = "footerText";
                    divScanNow.Visible = true;
                    divScanBox.Visible = true;

                    if (bool.Parse(AllowScanByPhoneSetting))
                    {
                        divRightFooter.Visible = true;
                        btnSearchByPhone.Visible = true;
                        btnSearchByPhone.Text = SearchByPhoneMessageSetting;
                    }
                }
                else
                {
                    divTimer.Visible = true;
                    divLeftFooter.Visible = false;
                    divRightFooter.Visible = false;
                    divScanNow.Visible = false;
                    divScanBox.Visible = false;

                    if (occurrences.Count > 0)
                    {
                        DateTime nextStart = occurrences.First().CheckInStart;
                        DateTime waitTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, nextStart.Hour, nextStart.Minute, nextStart.Second);
                        lblTimeRemaining.Text = "Check-In Start: <span id=\"CountDown\"></span>";
                        ihNowTime.Value = DateTime.Now.ToString();
                        ihStartTime.Value = waitTime.ToString();
                    }
                    else
                    {
                        lblTimeRemaining.Text = EventIsClosedMessageSetting;
                    }
                }
            }
            catch (InvalidOperationException)
            {
                divLeftFooter.Visible = false;
                divRightFooter.Visible = false;
                divWideFooter.Visible = true;
                lblWideFooter.Text = NoOccurrenceMessageSetting;
                lblWideFooter.CssClass = "footerText";
            }
            catch (ArgumentNullException)
            {
                state = CheckInStates.BadKiosk;
                Session[CheckInConstants.SESS_STATE] = state;
                ShowView();
            }
        }

        protected void btnSearchByPhone_Click(object sender, EventArgs e)
        {
            state = occurrences.Any() ? CheckInStates.FamilySearch : CheckInStates.Init;
            Session[CheckInConstants.SESS_STATE] = state;
            ResetError();
            ShowView();
        }

        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (!occurrences.Any())
            {
                tbScan.Text = Constants.NULL_STRING;
            }

            state = CheckInStates.Init;
            Session[CheckInConstants.SESS_STATE] = state;
            ResetError();
            ShowView();
        }

        #endregion

        #region Family Search View

        private void ShowFamilySearch()
        {
            pnlFamilySearch.Visible = true;
            dgFamilies.DataSource = null;
            dgFamilies.DataBind();
            txtPhone.Text = Constants.NULL_STRING;
            txtPhone.Focus();
            lblMessage.Text = Constants.NULL_STRING;
        }

        protected void btnFamilySearch_Click(object sender, EventArgs e)
        {
            if (txtPhone.Text != Constants.NULL_STRING)
            {
                FindMatchingByPhone(txtPhone.Text);
            }
            else
            {
                lblMessage.Text = "Please enter a phone number.";
                lblMessage.Style.Add("color", "#CC0000");
            }
        }

        protected void dgFamilies_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = int.Parse(dgFamilies.DataKeys[dgFamilies.SelectedItem.ItemIndex].ToString());
            GoToSelectFamilyMember(id);
        }

        protected void dgFamilies_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            ListItemType li = e.Item.ItemType;

            if (li == ListItemType.AlternatingItem ||
                li == ListItemType.Item ||
                li == ListItemType.SelectedItem)
            {
                Button button = (Button)e.Item.FindControl("FamilyID");
                Family family = (Family)e.Item.DataItem;
                button.Text = CheckInController.TruncateText(family.ToString(false));
            }
        }

        private void FindMatchingByPhone(string phoneNumber)
        {
            if (txtPhone.Text.Length >= int.Parse(PhoneLengthSetting))
            {
                FamilyCollection families = CheckInController.GetFamily(CheckInSearchTypes.PhoneNumber, phoneNumber);

                switch (families.Count)
                {
                    case 0:
                        txtPhone.Text = Constants.NULL_STRING;
                        lblMessage.Text = "Phone number not found.";
                        lblMessage.Style.Add("color", "#CC0000");
                        dgFamilies.DataSource = families;
                        dgFamilies.DataBind();
                        txtPhone.Focus();
                        break;
                    case 1:
                        GoToSelectFamilyMember(families[0]);
                        break;
                    default:
                        txtPhone.Text = "";
                        lblMessage.Text = "Please select your family:";
                        lblMessage.Style.Add("color", "black");
                        dgFamilies.DataSource = families;
                        dgFamilies.DataBind();
                        break;
                }
            }
            else
            {
                txtPhone.Text = Constants.NULL_STRING;
                lblMessage.Text = string.Format("Please enter at least {0} digits.", PhoneLengthSetting);
                lblMessage.Style.Add("color", "#CC0000");
                dgFamilies.DataSource = null;
                dgFamilies.DataBind();
                txtPhone.Focus();
            }
        }

        private void GoToSelectFamilyMember(int familyID)
        {
            GoToSelectFamilyMember(new Family(familyID));
        }

        private void GoToSelectFamilyMember(Family family)
        {
            Session[CheckInConstants.SESS_FAMILY] = family;

            state = CheckInStates.SelectFamilyMember;
            Session[CheckInConstants.SESS_STATE] = state;
            ShowView();
        }

        #endregion

        #region Select Family Member View

        private void ShowSelectFamilyMember()
        {
            pnlSelectFamilyMember.Visible = true;
            dgFamilyMembers.RepeatColumns = 1;
            dgFamilyMembers.Width = Unit.Pixel(353);
            Family family = (Family)Session[CheckInConstants.SESS_FAMILY];
            lblFamilyName.Text = string.Format("Hello, {0}!", family.FamilyName);
            FamilyMemberCollection familyMembers;

            if (RelationshipTypeIDsSetting.Trim() != Constants.NULL_STRING)
            {
                string[] ids = RelationshipTypeIDsSetting.Split(new[] { ',' });
                int[] relationshipTypeIDs = Array.ConvertAll(ids, new Converter<string, int>(Convert.ToInt32));
                familyMembers = CheckInController.GetRelatives(family, relationshipTypeIDs);
            }
            else
                familyMembers = CheckInController.GetRelatives(family);

            if (familyMembers.Count > 0)
            {
                try
                {
                    // Filter out any relatives who are too young (#344) or too old or by grade
                    List<FamilyMember> pplToCheckIn = (from FamilyMember fm in familyMembers
                                       where CheckInController.CanCheckIn(fm, int.Parse(MinimumAgeSetting), int.Parse(MaximumAgeSetting), int.Parse(MinimumGradeSetting), int.Parse(MaximumGradeSetting) )
                                       select fm).ToList();

                    if (pplToCheckIn.Count > 0)
                    {
                        Session[CheckInConstants.SESS_LIST_CHECKIN_FAMILYMEMBERS] = SetCheckInPeople(pplToCheckIn);
                        bool autoAdvance = (bool)Session[CheckInConstants.SESS_AUTO_ADVANCE];

                        // Will auto advance view if we've set autoAdvance to "true" and there is only one child to check in.
                        if (pplToCheckIn.Count == 1 && autoAdvance)
                        {
                            OccurrenceAttendance oa = CheckInController.GetAttendance(occurrences.First().StartTime, pplToCheckIn[0].PersonID);

                            // Want to ensure that auto-advance only happens if the child has NOT checked in yet.
                            if ( oa == null || !oa.Attended )
                            {
                                ihAttendeeIDs.Value = pplToCheckIn[0].PersonID.ToString();
                                btnSelectFamilyMemberContinue_Click(Constants.NULL_STRING, EventArgs.Empty);
                                return;
                            }
                        }

                        if (pplToCheckIn.Count > 4)
                        {
                            dgFamilyMembers.RepeatColumns = 2;
                            dgFamilyMembers.Width = Unit.Pixel(706);
                            pnlSelectFamilyMember.Style.Add("text-align", "center");
                        }

                        btnSelectFamilyMemberContinue.Enabled = false;
                        btnSelectFamilyMemberContinue.CssClass = "nextButtonInactive";
                        dgFamilyMembers.DataSource = pplToCheckIn;
                        dgFamilyMembers.DataBind();
                    }
                    else
                    {
                        state = CheckInStates.NoEligiblePeople;
                        Session[CheckInConstants.SESS_STATE] = state;
                        ShowView();
                    }
                }
                catch (InvalidOperationException)
                {
                    ShowError(NoOccurrenceMessageSetting);
                }
            }
            else
            {
                state = CheckInStates.NoEligiblePeople;
                Session[CheckInConstants.SESS_STATE] = state;
                ShowView();
            }
        }

        private static Dictionary<int, FamilyMember> SetCheckInPeople(IEnumerable<FamilyMember> family)
        {
            Dictionary<int, FamilyMember> peopleForCheckIn = new Dictionary<int, FamilyMember>();

            foreach (FamilyMember member in family)
            {
                if (!peopleForCheckIn.ContainsKey(member.PersonID))
                {
                    peopleForCheckIn.Add(member.PersonID, member);
                }
            }

            return peopleForCheckIn;
        }

        protected void dgFamilyMembers_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            ListItemType li = e.Item.ItemType;

            if (li == ListItemType.AlternatingItem ||
                li == ListItemType.SelectedItem ||
                li == ListItemType.Item)
            {
                FamilyMember fm = (FamilyMember)e.Item.DataItem;
				OccurrenceAttendance oa = CheckInController.GetAttendance( occurrences.First().StartTime, fm.PersonID );
                Button button = (Button)e.Item.FindControl("btnPerson");
                ImageButton image = (ImageButton)e.Item.FindControl("imgChecked");

                if (oa == null)
                {
                    button.Enabled = true;
                    button.CssClass = "dataButton";
                    image.Enabled = true;
                }
                else
                {
                    button.Enabled = false;
                    button.CssClass = "dataButtonInactive";
                    image.Enabled = false;
                }
            }
        }

        protected void btnSelectFamilyMemberContinue_Click(object sender, EventArgs e)
        {
            Dictionary<int, FamilyMember> attendees = new Dictionary<int, FamilyMember>();
            int[] ids = Array.ConvertAll(ihAttendeeIDs.Value.Split(new[] { ',' }), new Converter<string, int>(Convert.ToInt32));

            foreach (int i in ids)
            {
                attendees.Add(i, new FamilyMember(i));
            }

            Session[CheckInConstants.SESS_ATTENDEES] = attendees;
            state = bool.Parse(GetAttendeeAbilitiesSetting) ? CheckInStates.SelectAbility : CheckInStates.SelectService;
            Session[CheckInConstants.SESS_STATE] = state;
            ihAttendeeIDs.Value = Constants.NULL_STRING;
            ShowView();
        }

        #endregion

        #region No Young Eligible People

        private void ShowNoEligiblePeople()
        {
            pnlNoEligiblePeople.Visible = true;
            pnlNoEligiblePeople.Style.Add("text-align", "center");
            lblNoEligiblePeople.CssClass = "errorText";
            lblNoEligiblePeople.Visible = true;
            lblNoEligiblePeople.Text = NoEligibleCheckInPeopleSetting;
        }

        #endregion

        #region Select Ability View

        private void ShowSelectAbility()
        {
            pnlSelectAbility.Visible = true;
            BuildIDList();
        }

        private void BuildIDList()
        {
            int[] attendeesToProcess;

            if (ihAttendeesToProcess.Value != Constants.NULL_STRING)
            {
                string[] ids = ihAttendeesToProcess.Value.Split(new[] { ',' });
                attendeesToProcess = Array.ConvertAll(ids, new Converter<string, int>(Convert.ToInt32));
            }
            else
            {
                // This should only happen the first time we hit this screen for a family.
                List<FamilyMember> children = GetAttendeeList().ToList();
                attendeesToProcess = new int[children.Count()];
                StringBuilder ids = new StringBuilder();

                for (int i = 0; i < children.Count(); i++)
                {
                    attendeesToProcess[i] = children[i].PersonID;

                    if (i > 0)
                    {
                        ids.Append(",");
                    }

                    ids.Append(attendeesToProcess[i]);
                }

                ihAttendeesToProcess.Value = ids.ToString();
            }

            ProcessAttendees(attendeesToProcess);
        }

        private void ProcessAttendees(int[] ids)
        {
            if (ids.Length > 0)
            {
                LookupCollection abilityLookups = new LookupCollection(int.Parse(AbilityLevelLookupTypeIDSetting));
                int maxAbilityLookupID = abilityLookups[abilityLookups.Count - 1].LookupID;
                FamilyMember child = new FamilyMember(ids[0]);
                PersonAttribute personAttribute = new PersonAttribute(child.PersonID, int.Parse(AbilityLevelAttributeIDSetting));

                if (DateUtils.GetFractionalAge(child.BirthDate) >= decimal.Parse(MaxAbilityLevelAgeSetting))
                {
                    if (personAttribute.IntValue != maxAbilityLookupID)
                    {
                        CheckInController.SetChildToMaxAbility(personAttribute, maxAbilityLookupID);
                    }

                    int[] newIDs = RemoveChild(ids, child.PersonID);
                    PersistChildren(newIDs);
                    ProcessAttendees(newIDs);
                }
                else if (personAttribute.IntValue != maxAbilityLookupID)
                {
                    BuildAbilitySelector(child, abilityLookups, personAttribute.IntValue);
                }
                else
                {
                    int[] newIDs = RemoveChild(ids, child.PersonID);
                    PersistChildren(newIDs);
                    ProcessAttendees(newIDs);
                }
            }
            else
            {
                state = CheckInStates.SelectService;
                Session[CheckInConstants.SESS_STATE] = state;
                ShowView();
            }
        }

        private IEnumerable<FamilyMember> GetAttendeeList()
        {
            Dictionary<int, FamilyMember> familyMembers = (Dictionary<int, FamilyMember>)Session[CheckInConstants.SESS_ATTENDEES];

            foreach (KeyValuePair<int, FamilyMember> fm in familyMembers)
            {
                yield return fm.Value;
            }
        }

        private static int[] RemoveChild(IEnumerable<int> idList, int personID)
        {
            List<int> newList = idList.ToList();

            // Move to next person on the list by removing their ID from the hidden field's value.  This should
            // only be done if the person's ID is currently in the list, to avoid removing ID's that have not 
            // been processed yet, due to a user clicking more than once on the same person's ability level.
            if (idList.Contains(personID))
            {
                newList.Remove(personID);
            }

            return newList.ToArray();
        }

        private void PersistChildren(IEnumerable<int> ids)
        {
            // Rebuild the string of ID's and put it back into the hidden field value.
            StringBuilder idList = new StringBuilder();

            foreach (int i in ids)
            {
                if (idList.Length > 0)
                {
                    idList.Append(",");
                }

                idList.Append(i);
            }

            ihAttendeesToProcess.Value = idList.ToString();
        }

        protected void dgAbilities_SelectedIndexChanged(object sender, EventArgs e)
        {
            Button button = (Button)dgAbilities.SelectedItem.FindControl("lookupID");
            int abilityID = int.Parse(button.CommandArgument);
            int[] ids = Array.ConvertAll(ihAttendeesToProcess.Value.Split(new[] { ',' }), new Converter<string, int>(Convert.ToInt32));
            FamilyMember child = new FamilyMember(ids[0]);
            PersonAttribute attribute = new PersonAttribute(child.PersonID, int.Parse(AbilityLevelAttributeIDSetting))
                                            {
                                                IntValue = abilityID
                                            };

            attribute.Save(CurrentOrganization.OrganizationID, CurrentUser.Identity.Name);
            int[] newList = RemoveChild(ids, child.PersonID);
            PersistChildren(newList);
            ProcessAttendees(newList);
        }

        private void BuildAbilitySelector(Person child, LookupCollection abilityLookups, int abilityID)
        {
            LookupCollection abilities = new LookupCollection();
            bool abilityMatch = false;

            foreach (Lookup ability in abilityLookups)
            {
                if ((!abilityMatch && ability.LookupID == abilityID) || abilityID == Constants.NULL_INT)
                {
                    abilityMatch = true;
                }

                if (abilityMatch)
                {
                    abilities.Add(ability);
                }
            }

            lblPersonName.Text = child.NickName;
            dgAbilities.DataSource = abilities;
            dgAbilities.DataBind();
        }

        #endregion

        #region Select Service View

        private void ShowSelectService()
        {
            pnlSelectService.Visible = true;
            btnServicesContinue.Enabled = false;
            btnServicesContinue.CssClass = "nextButtonInactive";

            // All occurrences at location within the selected time range
            var serviceTimes = (from o in occurrences
                                select o.StartTime).Distinct();
            bool autoAdvance = (bool)Session[CheckInConstants.SESS_AUTO_ADVANCE];

            if (serviceTimes.Count() == 1 && autoAdvance)
            {
                List<DateTime> service = new List<DateTime> { serviceTimes.First() };
                Session[CheckInConstants.SESS_SERVICE_TIMES] = service;
                state = CheckInStates.Confirm;
                Session[CheckInConstants.SESS_STATE] = state;
                ShowView();
                return;
            }

            dgEventTimes.DataSource = serviceTimes;
            dgEventTimes.DataBind();
        }

        protected void dgEventTimes_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            ListItemType li = e.Item.ItemType;

            if (li == ListItemType.Item ||
                li == ListItemType.AlternatingItem ||
                li == ListItemType.SelectedItem)
            {
                DateTime serviceTime = (DateTime)e.Item.DataItem;
                Button b = (Button)e.Item.FindControl("btnService");
                b.Text = serviceTime.ToShortTimeString();
                b.CommandArgument = serviceTime.ToString();

                if (e.Item.ItemIndex == 0)
                {
                    b.Enabled = true;
                    b.CssClass = "dataButton";
                }
                else
                {
                    b.Enabled = false;
                    b.CssClass = "dataButtonInactive";
                }
            }
        }

        /// <summary>
        /// This event is called each time a service time is selected.  Two consecutive items may be selected
        /// but the earliest time in the list must be selected first.  The available times are stored in the 
        /// session variable SESS_SERVICE_TIMES.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dgEventTimes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Button button = (Button)dgEventTimes.SelectedItem.FindControl("btnService");
            Image imgChecked = (Image)dgEventTimes.SelectedItem.FindControl("imgChecked");
            int selectedIndex = dgEventTimes.SelectedIndex;
            btnServicesContinue.Enabled = false;
            btnServicesContinue.CssClass = "nextButtonInactive";

            if (button.Enabled)
            {
                if (imgChecked.ImageUrl == CHECK_BOX_IMG_URL)
                {
                    imgChecked.ImageUrl = EMPTY_CHECK_BOX_IMG_URL;
                    button.CssClass = "dataButton";

                    // disable all items that follow the current item in the datalist
                    foreach (DataListItem item in dgEventTimes.Items)
                    {
                        Button b = (Button)item.FindControl("btnService");
                        Image i = (Image)item.FindControl("imgChecked");

                        if (item.ItemIndex > selectedIndex)
                        {
                            b.Enabled = false;
                            b.CssClass = "dataButtonInactive";

                            imgChecked = (Image)item.FindControl("imgChecked");
                            imgChecked.ImageUrl = EMPTY_CHECK_BOX_IMG_URL;
                        }
                        else
                        {
                            if (b.Enabled && i.ImageUrl == CHECK_BOX_IMG_URL)
                            {
                                btnServicesContinue.Enabled = true;
                                btnServicesContinue.CssClass = "nextButton";
                            }
                        }
                    }
                }
                else
                {
                    imgChecked.ImageUrl = CHECK_BOX_IMG_URL;
                    button.CssClass = "dataButtonSelected";
                    btnServicesContinue.Enabled = true;
                    btnServicesContinue.CssClass = "nextButton";

                    // enable the next item in the list
                    foreach (DataListItem item in dgEventTimes.Items)
                    {
                        if (item.ItemIndex > selectedIndex)
                        {
                            Button b = (Button)item.FindControl("btnService");
                            b.Enabled = true;
                            b.CssClass = "dataButton";
                            break;
                        }
                    }
                }
            }
        }

        protected void btnServicesContinue_Click(object sender, EventArgs e)
        {
            List<DateTime> serviceTimes = new List<DateTime>();

            foreach (DataListItem item in dgEventTimes.Items)
            {
                ListItemType li = item.ItemType;

                if (li == ListItemType.SelectedItem ||
                    li == ListItemType.Item ||
                    li == ListItemType.AlternatingItem)
                {
                    Image checkBox = (Image)item.FindControl("imgChecked");
                    Button button = (Button)item.FindControl("btnService");

                    if (checkBox.ImageUrl == CHECK_BOX_IMG_URL)
                    {
                        serviceTimes.Add(DateTime.Parse(button.CommandArgument));
                    }
                }
            }

            if (serviceTimes.Count > 0)
            {
                Session[CheckInConstants.SESS_AUTO_ADVANCE] = true;
                Session[CheckInConstants.SESS_SERVICE_TIMES] = serviceTimes;
                state = CheckInStates.Confirm;
                Session[CheckInConstants.SESS_STATE] = state;
                ResetError();
                ShowView();
            }
            else
                ShowError("Please select a service time from the list below.");
        }

        #endregion

        #region Confirm View

        private void ShowConfirm()
        {
            pnlConfirm.Visible = true;
            BuildConfirmTable();
        }

        private void BuildConfirmTable()
        {
            HtmlTable table = new HtmlTable();
            table.Attributes.Add("class", "resultTable");
            table.Attributes.Add("cellspacing", "0");
            Dictionary<int, FamilyMember> familyMembers = (Dictionary<int, FamilyMember>)Session[CheckInConstants.SESS_ATTENDEES];
            table.Rows.Add(BuildHeaderRow());
            int totalUnavailable = 0;
            int totalClasses = 0;
            divConfirmError.Visible = false;
            bool ignoreCheckInStart = bool.Parse(IgnoreCheckInStartSetting);

            foreach (KeyValuePair<int, FamilyMember> kvp in familyMembers)
            {
                int unavailableClasses;
                int total;
                HtmlTableRow row = BuildPersonRow(kvp.Value, out unavailableClasses, out total, ignoreCheckInStart);
                totalUnavailable += unavailableClasses;
                totalClasses += total;
                table.Rows.Add(row);
            }

            if (totalUnavailable > 0)
            {
                divConfirmError.Visible = true;
                lblConfirmError.Text = UnavailableOccurrencesTextSetting;
            }

            phConfirm.Controls.Add(table);
            Session[CheckInConstants.SESS_UNAVAILABLE_OCCURRENCES] = totalUnavailable;
            Session[CheckInConstants.SESS_TOTAL_OCCURRENCES] = totalClasses;
        }

        private HtmlTableRow BuildHeaderRow()
        {
            List<DateTime> serviceTimes = (List<DateTime>)Session[CheckInConstants.SESS_SERVICE_TIMES];
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell();
            cell.Controls.Add(new LiteralControl("&nbsp;"));
            row.Cells.Add(cell);  // Add blank cell for top left corner

            foreach (DateTime serviceTime in serviceTimes)
            {
                cell = new HtmlTableCell();
                Label label = new Label
                                  {
                                      Text = serviceTime.ToShortTimeString(), 
                                      CssClass = "confirmText"
                                  };

                cell.Style.Add("padding-bottom", "10px");
                cell.Controls.Add(label);
                row.Cells.Add(cell);
            }

            return row;
        }

        private HtmlTableRow BuildPersonRow(FamilyMember person, out int unavailableClassCount, out int totalClassCount, bool ignoreCheckInStart)
        {
            HtmlTableRow row = new HtmlTableRow();
            HtmlTableCell cell = new HtmlTableCell();
            Label label = new Label
                              {
                                  Text = person.NickName.Trim() != Constants.NULL_STRING ? person.NickName : person.FirstName,
                                  CssClass = "confirmText"
                              };

            label.Style.Add("padding-right", "20px");
            cell.Style.Add("padding-bottom", "10px");
            cell.Controls.Add(label);
            row.Cells.Add(cell);

            List<DateTime> serviceTimes = (List<DateTime>) Session[CheckInConstants.SESS_SERVICE_TIMES];
            List<CheckInServiceInfo> services = new List<CheckInServiceInfo>();
            
            for (int i = 0; i < serviceTimes.Count; i ++)
            {
                services.Add(new CheckInServiceInfo(serviceTimes[i], ignoreCheckInStart));
            }

            List<Occurrence> availableClasses = CheckInController.FilterOccurrences(occurrences, person, services, DateTime.Now, 
                int.Parse(AbilityLevelAttributeIDSetting), int.Parse(SpecialNeedsAttributeIDSetting));
            BuildFamilyMap(person, availableClasses);
            unavailableClassCount = 0;
            totalClassCount = 0;

            foreach (Occurrence occurrence in availableClasses)
            {
                cell = new HtmlTableCell();
                label = new Label
                            {
                                Text = occurrence.Location
                            };

                label.Style.Add("padding-right", "20px");
                cell.Style.Add("padding-bottom", "10px");

                if (occurrence is EmptyOccurrence)
                {
                    unavailableClassCount++;
                    cell.Attributes.Add("class", "fail");
                    label.CssClass = "fail";
                }
                else
                {
                    label.CssClass = "classroomText";
                }

                cell.Controls.Add(label);
                row.Cells.Add(cell);
                totalClassCount++;
            }

            return row;
        }

		private void BuildFamilyMap(FamilyMember person, IEnumerable<Occurrence> classes)
		{
			List<PersonCheckInRequest> requests;

			if (Session[CheckInConstants.SESS_KEY_PEOPLEMAP] != null)
			{
				requests = (List<PersonCheckInRequest>)Session[CheckInConstants.SESS_KEY_PEOPLEMAP];
			}
			else
			{
				requests = new List<PersonCheckInRequest>();
			}

			if (!requests.Any(p => p.PersonID == person.PersonID))
			{
				requests.Add(new PersonCheckInRequest
				             	{
				             		PersonID = person.PersonID,
									FamilyMember = person,
									Occurrences = classes.ToList()
				             	});

				Session[CheckInConstants.SESS_KEY_PEOPLEMAP] = requests;
			}
		}

        protected void btnConfirmContinue_Click(object sender, EventArgs e)
        {
            int unavailableClasses = int.Parse(Session[CheckInConstants.SESS_UNAVAILABLE_OCCURRENCES].ToString());
            int totalClasses = int.Parse(Session[CheckInConstants.SESS_TOTAL_OCCURRENCES].ToString());

            // submit registration and display result
            var requests = (List<PersonCheckInRequest>) Session[CheckInConstants.SESS_KEY_PEOPLEMAP];
			ComputerSystem kiosk = (ComputerSystem)Session[CheckInConstants.SESS_KIOSK];
			var results = CheckInController.CheckInFamily(int.Parse(PrintProviderSetting),
				((Family) Session[CheckInConstants.SESS_FAMILY]).FamilyID, requests, kiosk);
            Session[CheckInConstants.SESS_RESULTS] = results;
            state = unavailableClasses == totalClasses ? CheckInStates.Init : CheckInStates.Result;
            Session[CheckInConstants.SESS_STATE] = state;
            ResetError();
            ShowView();
        }

        #endregion

        #region Result View

        private void ShowResult()
        {
            pnlResults.Visible = true;
            var results = (List<PersonCheckInResult>) Session[CheckInConstants.SESS_RESULTS];
            StringBuilder resultTable = new StringBuilder();
            resultTable.Append("<br /><br /><table cellspacing=\"0\" class=\"resultTable\">\n");

            foreach (var result in results)
            {
                resultTable.AppendLine(BuildResultString(result));
            }

            resultTable.Append("</table>");
            phResults.Controls.Add(new LiteralControl(resultTable.ToString()));
        }

		private static string BuildResultString(PersonCheckInResult result)
		{
			var firstResult = result.CheckInResults.OrderBy(r => r.Occurrence.StartTime).First();
		    string formatString;

			if (firstResult.Occurrence is EmptyOccurrence)
			{
				// No matching occurrences found
				formatString = "<tr><td class=\"resultTable\">{0}</td><td colspan=\"2\" class=\"resultTable fail\"><span class=\"fail\">{1}</span></td></tr>";
			}
			else if (!firstResult.IsCheckInSuccessful)
			{
				// Check In Failure
				formatString = "<tr><td class=\"resultTable\">{0}</td><td class=\"resultTable fail\">{1}</td><td class=\"resultTable fail\"><span class=\"fail\">Check-In Failure</span></td></tr>";
			}
			else if (!result.IsPrintSuccessful)
			{
				// Print Failure
				formatString = "<tr><td class=\"resultTable\">{0}</td><td class=\"resultTable\">{1}</td><td class=\"resultTable fail\"><span class=\"fail\">Print Failure</span></td></tr>";
			}
			else
			{
				// Success
				formatString = "<tr><td class=\"resultTable\">{0}</td><td class=\"resultTable\"><span class=\"classroomText\">{1}</span></td><td class=\"resultTable success\">&nbsp;</td></tr>";
			}

            return string.Format(formatString, result.FamilyMember.NickName, firstResult.Occurrence.Location);
		}

        protected void btnResultsContinue_Click(object sender, EventArgs e)
        {
			state = CheckInStates.Init;
			Session[CheckInConstants.SESS_STATE] = state;

			if (!string.IsNullOrEmpty(PostCheckinRedirectPageSetting))
			{
				var results = (IEnumerable<PersonCheckInResult>) Session[CheckInConstants.SESS_RESULTS];
				var sessionID = results.First().SessionID;
				Response.Redirect(string.Format("default.aspx?page={0}&sessionid={1}", PostCheckinRedirectPageSetting, sessionID));
			}
			else
			{
				ShowView();
			}
        }

        #endregion

        #region Bad Kiosk View

        private void ShowBadKiosk()
        {
            pnlBadKiosk.Visible = true;
            lblBadKiosk.Text = BadKioskTextSetting;
        }

        protected void btnTryAgain_Click(object sender, EventArgs e)
        {
            state = CheckInStates.Init;
            Session[CheckInConstants.SESS_STATE] = state;
            ShowView();
        }

        #endregion
    }
}