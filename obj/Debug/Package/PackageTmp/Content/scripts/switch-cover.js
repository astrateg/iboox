$(function () {
    $('#switch-cover').click(function () {
        var newsrc = '';
        var isbn = '';
        var cover_type = '';

        if ($(this).hasClass('selected')) {
            isbn = $('#switch-cover').attr('isbn1');
            cover_type = $('#switch-cover').attr('cover1');
            $(this).removeClass('selected');
        } else {
            isbn = $('#switch-cover').attr('isbn2');
            cover_type = $('#switch-cover').attr('cover2');
            $(this).addClass('selected');
        }

        newsrc = '/content/books/covers/300/' + isbn + '.jpg'
        $('#cover').attr('src', newsrc);
        $('#isbn').html(isbn);
        $('#cover_type').html(cover_type);
    });
    return false;
});