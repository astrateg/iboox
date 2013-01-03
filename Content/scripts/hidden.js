$(function () {
    $('#addition').bind('click', function () {
        link = $(this);
        image = link.find('#addition_token');

        if (link.hasClass('expanded')) {
            link.nextAll().each(function (i) {
                var element = $(this);
                setTimeout(function () {
                    element.slideUp(500);
                }, 100 * i);
            });
            link.removeClass('expanded');
            image.attr('src', '/content/images/centerstage/img05.gif');
        } else {
            link.nextAll().each(function (i) {
                var element = $(this);
                setTimeout(function () {
                    element.slideDown(100);
                }, 100 * i);
            });
            link.addClass('expanded');
            image.attr('src', '/content/images/centerstage/img06.gif');
        }
    });
});
