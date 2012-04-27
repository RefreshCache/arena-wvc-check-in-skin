/*
 * Disable dragging on iOS devices.
 */
// document.addEventListener("touchmove", function(e){ e.preventDefault(); }, false);

/*
 * NoClickDelay disables the .5 second click delay on iOS devices.
 * From: http://cubiq.org/remove-onclick-delay-on-webkit-for-iphone
 */
function NoClickDelay(el) {
    this.element = typeof el == 'object' ? el : document.getElementById(el);
    if (window.Touch && this.element)
        this.element.addEventListener('touchstart', this, false);
}

NoClickDelay.prototype = {
    handleEvent: function(e) {
        switch(e.type) {
            case 'touchstart': this.onTouchStart(e); break;
            case 'touchmove': this.onTouchMove(e); break;
            case 'touchend': this.onTouchEnd(e); break;
        }
    },

    onTouchStart: function(e) {
        e.preventDefault();
        this.moved = false;
        this.startX = e.touches[0].pageX;
        this.startY = e.touches[0].pageY;

        this.theTarget = document.elementFromPoint(e.targetTouches[0].clientX, e.targetTouches[0].clientY);
        if(this.theTarget.nodeType == 3) this.theTarget = theTarget.parentNode;
        this.theTarget.className+= ' pressed';

        this.element.addEventListener('touchmove', this, false);
        this.element.addEventListener('touchend', this, false);
    },

    onTouchMove: function(e) {
        if (Math.abs(this.startX - e.touches[0].pageX) > 40 || Math.abs(this.startY - e.touches[0].pageY) > 40) {
            this.moved = true;
            this.theTarget.className = this.theTarget.className.replace(/ ?pressed/gi, '');
        }
    },
                
    onTouchEnd: function(e) {
        this.element.removeEventListener('touchmove', this, false);
        this.element.removeEventListener('touchend', this, false);

        if( !this.moved && this.theTarget ) {
            this.theTarget.className = this.theTarget.className.replace(/ ?pressed/gi, '');
            var theEvent = document.createEvent('MouseEvents');
            theEvent.initEvent('click', true, true);
            this.theTarget.dispatchEvent(theEvent);
        }

        this.theTarget = undefined;
    }
};

/*
 * On page load/postback, set the NoClickDelay.
 */
Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function(sender, args)
{
    $('*[onclick]').each(function(index) {
        if ($(this).text().length < 100)
            new NoClickDelay($(this)[0]);
    });
    $('.divButton').each(function(index) {
        new NoClickDelay($(this)[0]);
    });
    $('input[type=submit]').each(function(index) {
        new NoClickDelay($(this)[0]);
    });
    $('input[type=image]').each(function(index) {
        new NoClickDelay($(this)[0]);
    });
});
