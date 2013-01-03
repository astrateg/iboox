$(document).ready(function () {
    $('#search-input').keyup(function () {
        var search = $(this).val();
        if ($(this).val() != '') {
            $('#search-button').removeAttr('disabled');
        } else {
            $('#search-button').attr('disabled', 'disabled');
        }
    });
});

$('form').submit(function () {
    if ($(this).has('#search-input').length > 0) {
        var search = $('#search-input').val();
        search = $.trim(search);
        search = encodeURIComponent(search);
        $(this).attr('search', search);
        $(this).attr('action', '/books/search/' + search);
    }
});
