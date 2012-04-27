$(function ()
{
    initPage();
});

Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args)
{
    if (args.get_error() != null)
    {
        if (args.get_response().get_statusCode() == '500')
        {
            args.set_errorHandled(true);
            alert($("input[id$='ihTimeoutError']").val());
            $("input[id$='btnRedirect']").click();
        }
    }

    initPage();
});

function initPage()
{
    getState();

    if (autoConfirmCancel || state == 'init')
    {
        refreshPage();
    }

    $('.scroll-pane').jScrollPane({
        showArrows : true,
        arrowButtonSpeed : 15,
        arrowRepeatFreq : 250,
        hideFocus : true
    });
}

function reloadBrowser()
{
    window.location.reload(true);
}

function resetTimer()
{
    startTime = new Date();
    startTime = startTime.getTime();
}

function refreshPage()
{
    startTime = new Date();
    startTime = startTime.getTime();
    refreshCountDown();
}

function refreshCountDown()
{
    if (shouldRefresh && (autoConfirmCancel || state == 'init'))
    {
        rightNow = new Date();
        rightNow = rightNow.getTime();
        secondsSinceLastRefresh = (rightNow - startTime) / 1000;
        refreshSeconds = Math.round(interval - secondsSinceLastRefresh);
        var timer;

        if (interval >= secondsSinceLastRefresh)
        {
            timer = setTimeout('refreshCountDown()', 1000);
            window.status = 'Page refresh: ' + refreshSeconds + ' State: ' + state;
        }
        else
        {
            clearTimeout(timer);

            if (state == 'confirm')
            {
                $("input[id$='btnConfirmContinue']").click();
            }
            else
            {
                $("input[id$='btnRedirect']").click();
            }
        }
    }
    else
    {
        window.status = '';
    }
}

function getState()
{
    var init = $("div[id$='pnlInit']");
    var familySearch = $("div[id$='pnlFamilySearch']");
    var selectFamilyMember = $("div[id$='pnlSelectFamilyMember']");
    var noEligible = $("div[id$='pnlNoEligiblePeople']");
    var selectAbility = $("div[id$='pnlSelectAbility']");
    var selectService = $("div[id$='pnlSelectService']");
    var confirm = $("div[id$='pnlConfirm']");
    var results = $("div[id$='pnlResults']");
    var badKiosk = $("div[id$='pnlBadKiosk']");
    shouldRefresh = true;
    $("#upProgress").children(":first").addClass("ajaxProgress");

    if (init.length > 0)
    {
        var timerLabel = $("span[id$='lblTimeRemaining']");

        if (timerLabel.length > 0)
        {
            shouldRefresh = false;
        }
        else
        {
            setupViewInfo("init", shortInterval, false);
            shouldRefresh = true;
        }

        setupCountdown();
    }
    else if (familySearch.length > 0)
    {
        txtPhone = $("input[id$='txtPhone']");
        searchButton = $("input[id$='btnFamilySearch']");
        messageLabel = $("span[id$='lblMessage']");
        setupViewInfo("familySearch", longInterval, true);
    }
    else if (selectFamilyMember.length > 0)
    {
        setupViewInfo("selectFamilyMember", longInterval, true);
        bindSelectFamilyMemberEvents();
    }
    else if (noEligible.length > 0)
    {
        setupViewInfo("noEligiblePeople", shortInterval, true);
    }
    else if (selectAbility.length > 0)
    {
        setupViewInfo("selectAbilityLevel", longInterval, true);
    }
    else if (selectService.length > 0)
    {
        setupViewInfo("selectService", longInterval, true);
    }
    else if (confirm.length > 0)
    {
        setupViewInfo("confirm", shortInterval, true);
    }
    else if (results.length > 0)
    {
        setupViewInfo("results", longInterval, true);
    }
    else if (badKiosk.length > 0)
    {
        setupViewInfo("badKiosk", longInterval, false);
    }
    else
    {
        setupViewInfo("undefined", longInterval, false);
    }
}

function setupViewInfo(stateName, intervalLength, shouldShowLoadingImage)
{
    state = stateName;
    interval = intervalLength;

    if (shouldShowLoadingImage === true)
    {
        $("#upProgress").removeClass("ajaxProgress").addClass("ajaxLargeProgress");
    }
}

function bindSelectFamilyMemberEvents()
{
    var imgEmpty = 'UserControls/Custom/Cccev/Checkin/images/empty_checkbox.png';
    var imgChecked = 'UserControls/Custom/Cccev/Checkin/images/checkbox.png';
    var selectedCount = 0;
    var nextButton = $("input[id$='btnSelectFamilyMemberContinue']");

    $("table[id$='dgFamilyMembers'] input:submit").click(function ()
    {
        if ($(this).hasClass('dataButton'))
        {
            addAttendee($(this).siblings(":last").val());
            $(this).removeClass().addClass('dataButtonSelected');
            $(this).parent().siblings(":first").children("input:first").attr('src', imgChecked);
            selectedCount++;
        }
        else
        {
            removeAttendee($(this).siblings(":last").val());
            $(this).removeClass().addClass('dataButton');
            $(this).parent().siblings(":first").children("input:first").attr('src', imgEmpty);
            selectedCount--;
        }

        if (selectedCount > 0)
        {
            $(nextButton).removeAttr('disabled');
            $(nextButton).removeClass('nextButtonInactive').addClass('nextButton');
        }
        else
        {
            $(nextButton).attr('disabled', 'disabled');
            $(nextButton).removeClass('nextButton').addClass('nextButtonInactive');
        }

        return false;
    });

    $("table[id$='dgFamilyMembers'] input:image").click(function (event)
    {
        if ($(this).attr('src').indexOf('empty_checkbox', 0) > 0)
        {
            addAttendee($(this).parent().siblings(":last").children("input:last").val());
            $(this).attr('src', imgChecked);
            $(this).parent().siblings(":last").children("input:first").removeClass().addClass('dataButtonSelected');
            selectedCount++;
        }
        else
        {
            removeAttendee($(this).parent().siblings(":last").children("input:last").val());
            $(this).attr('src', imgEmpty);
            $(this).parent().siblings(":last").children("input:first").removeClass().addClass('dataButton');
            selectedCount--;
        }

        if (selectedCount > 0)
        {
            $(nextButton).removeAttr('disabled');
        }
        else
        {
            $(nextButton).attr('disabled', 'disabled');
        }

        return false;
    });
}

if (!Array.prototype.indexOf)
{
    Array.prototype.indexOf = function (val)
    {
        var length = this.length;

        var from = Number(arguments[1]) || 0;
        from = (from < 0) ? Math.ceil(from) : Math.floor(from);

        if (from < 0)
        {
            from += length;
        }

        for (; from < length; from++)
        {
            if (from in this && this[from] === val)
            {
                return from;
            }
        }

        return -1;
    };
}

function addAttendee(id)
{
    var attendees = $("input[id$='ihAttendeeIDs']");
    var attendeeArray = new Array();
    var ids = attendees.val();
    attendeeArray = ids.split(',');

    if (attendeeArray.indexOf(id) == -1)
    {
        if (attendees.val().length > 0)
        {
            ids += ',';
        }

        ids += id;
        attendees.val(ids);
    }
}

function removeAttendee(id)
{
    var attendees = $("input[id$='ihAttendeeIDs']");
    var attendeeArray = new Array();
    var newList = new String();
    attendeeArray = attendees.val().split(',');

    if (attendeeArray.indexOf(id) > -1)
    {
        for (var i = 0; i < attendeeArray.length; i++)
        {
            if (attendeeArray[i] != id)
            {
                if (newList.length > 0)
                {
                    newList += ',';
                }

                newList += attendeeArray[i];
            }
        }

        attendees.val(newList);
    }
}

function FireDefaultButton(event, target)
{
    var key_Zero = 47;
    var key_Nine = 57;

    if ((event.keyCode == 13 || event.which == 13) && !(event.srcElement && (event.srcElement.tagName.toLowerCase() == 'textarea')))
    {
        var defaultButton = document.getElementById(target);
        if (defaultButton == 'undefined') defaultButton = document.all[target];

        if (defaultButton && typeof (defaultButton.click) != 'undefined')
        {
            Search();
            defaultButton.click();

            txtPhone.attr("disabled", "disabled");
            event.cancelBubble = true;
            if (event.stopPropagation) event.stopPropagation();
            return false;
        }
    }
    else if (event && event.keyCode >= key_Zero && event.keyCode <= key_Nine)
    {
        // if there are already 9 numbers then this is the 10th...
        if (txtPhone.val().length >= 9)
        {
            ClickDigit(String.fromCharCode(event.keyCode));
            txtPhone.attr("disabled", "disabled");
            event.cancelBubble = true;
            if (event.stopPropagation) event.stopPropagation();
            return false;
        }
    }
    return true;
}

function GetKeys()
{
    var key_Enter = 13;
    var key_Zero = 47;
    var key_Nine = 57;

    if (window.event && window.event.keyCode == key_Enter)
    {
        if (txtPhone.val().length >= maxDigits)
        {
            Search();
            txtPhone.attr("disabled", "disabled");
            ClickDigit('');
            window.event.cancelBubble = true;
            if (event.stopPropagation) event.stopPropagation();
            return false;
        }
        else
        {
            IncompleteEntry();
        }
    }
    else if (window.event && window.event.keyCode >= key_Zero && window.event.keyCode <= key_Nine)
    {
        // if there are already 9 numbers then this is the 10th...
        if (txtPhone.val().length >= 9)
        {
            ClickDigit(String.fromCharCode(event.keyCode));
        }
    }
}

function ClickDigit(digit)
{
    txtPhone.focus();
    if (parseInt(digit) >= 0)
    {
        txtPhone.val(txtPhone.val() + digit);
    }

    // once the person has entered all ten digits, submit by navigating to the
    // same page with the digits in the txtPhone name/value pair.
    if (txtPhone.val().length >= maxDigits)
    {
        Search();

        searchButton.click();
        txtPhone.attr("disabled", "disabled");
        searchButton.attr("disabled", "disabled");
    }

    return false;
}

function ClearDigit()
{
    var newVal = txtPhone.val().substring(0, txtPhone.val().length - 1);
    txtPhone.val(newVal);
    txtPhone.focus();
}

function ClearAll()
{
    txtPhone.val('');
    messageLabel.html("&nbsp;");
    $("#ScrollArea").html("");
    txtPhone.focus();
    return false;
}

function IncompleteEntry()
{
    messageLabel.html('Please enter all ' + maxDigits + ' digits.');
    messageLabel.removeClass().addClass('phoneError');
}

function Search()
{
    $("#ScrollArea").html("");
    messageLabel.html('Searching...');
    messageLabel.removeClass().addClass('phoneSearching');

    return true;
}

function disableButton(button)
{
    button.style.color = '#999999';

    if (button.getAttribute('requestSent') != 'true')
    {
        button.setAttribute('requestSent', 'true');
        return true;
    }
    else
    {
        button.disabled = true;
        return false;
    }

}

var isCtrlKey = false;
var isShiftKey = false;

document.onkeyup = function (e)
{
    var keyCode = (e && e.which ? e.which : event.keyCode);

    if (keyCode == 16)
    {
        isShiftKey = false;
    }

    if (keyCode == 17)
    {
        isCtrlKey = false;
    }
}

document.onkeydown = function (e)
{
    var keyCode = (e && e.which ? e.which : event.keyCode);

    if (keyCode == 16)
    {
        isShiftKey = true;
    }

    if (keyCode == 17)
    {
        isCtrlKey = true;
    }

    if (keyCode == 82 && (isCtrlKey && isShiftKey))
    {
        if ($("input[id$='ihFamilyRegistrationPage']").val() != "")
        {
            window.location = 'default.aspx?page=' + $("input[id$='ihFamilyRegistrationPage']").val();
            return false;
        }
    }

    // ctrl-shift-M; go to kiosk management page
    if (keyCode == 77 && isCtrlKey && isShiftKey)
    {
        if ($("input[id$='ihKioskManagementPage']").val() != "")
        {
            window.location = 'default.aspx?page=' + $("input[id$='ihKioskManagementPage']").val();
            return false;
        }
    }
}

function CalcAge(secs, num1, num2)
{
    s = ((Math.floor(secs / num1)) % num2).toString();
    if (s.length < 2)
    {
        s = "0" + s;
    }
    return s;
}

function Ticker(secs, pageID)
{
    if (secs < 0)
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
