using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ArenaWeb.UserControls.Custom.Cccev.Checkin
{
    public partial class BlankTemplate : Arena.Portal.PortalControl
    {
        public PlaceHolder Main { get { return phMain; } }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}