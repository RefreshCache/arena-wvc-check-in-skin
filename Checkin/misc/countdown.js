/**********************************************************************
* Description:	Counts down time between now and a "start time" in the future and will
*               and will render it to a <span> tag with the id of 'CountDown'.
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	TBD
*
* $Id: $
**********************************************************************/

/* Simply set StartTime to a date time */
/* Example:
<script language="JavaScript">
StartTime = "5/5/2005 4:00 PM";
</script>
<script language="JavaScript" src="countdown.js"></script>
*/
function CalcAge(secs, num1, num2)
{
	s = ((Math.floor(secs/num1))%num2).toString();
	if (s.length < 2)
	{
		s = "0" + s;
	}
	return s;
}

function Ticker(secs, pageID)
{
	if ( secs < 0 )
	{
	    window.location = "default.aspx?page=" + pageID;
	}
	else
	{
	    var hours = CalcAge(secs, 3600, 24);
	    var minutes = CalcAge(secs, 60, 60);
	    var seconds = CalcAge(secs, 1, 60);

	    var timeString = new String();

	    if (hours != '00')
	    {
	        timeString += hours + ':';
	    }

	    if (minutes != '00')
	    {
	        timeString += minutes + ':';
	    }
	    else
	    {
	        if (hours != '00')
	        {
	            timeString += minutes + ':';
	        }
	    }

	    timeString += seconds;
	    document.getElementById('CountDown').innerHTML = timeString;
		setTimeout("Ticker(" + (secs - 1) + ", " + pageID + ")", 990);
		
	}
}

function StartTimer(pageID)
{
    if (!(typeof (StartTime) == "undefined" && typeof (NowTime) == "undefined"))
    {        
        var startTime = new Date(StartTime);
        var rightNow = new Date(NowTime);
        ddiff = new Date(startTime - rightNow);
        gsecs = Math.floor(ddiff.valueOf() / 990);
        Ticker(gsecs, pageID);
    }
}