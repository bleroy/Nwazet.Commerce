
(function ($) {

    var populate = function (el, parentId) {

        // direct children
        var children = $(el).children('li').each(function (i, child) {
            child = $(child);

            // apply positions to all siblings
            child.find('.territory-parent > input').attr('value', parentId);
            
            // recurse position for children
            child.children('ol').each(function (i, item) {
                populate(item, child.attr('data-index'))
            });
        });
    };

    $('.territories-list > ol').nestedSortable({
        disableNesting: 'no-nest',
        forcePlaceholderSize: true,
        handle: 'div',
        helper: 'clone',
        items: 'li',
        maxLevels: 6,
        opacity: 1,
        placeholder: 'territory-placeholder',
        revert: 50,
        tabSize: 30,
        rtl: window.isRTL,
        tolerance: 'pointer',
        toleranceElement: '> div',

        stop: function (event, ui) {
            // update all positions whenever a menu item was moved
            populate(this, "0");
            $('#save-message').show();

            // display a message on leave if changes have been made
            window.onbeforeunload = function (e) {
                return $("<div/>").html(leaveConfirmation).text();
            };

            // cancel leaving message on save
            $('#saveButton').click(function (e) {
                window.onbeforeunload = function () { };
            });
        }
    });



})(jQuery);
