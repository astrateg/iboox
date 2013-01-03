$(document).ready(function () {
    function set_votes(widget) {
        var rate_widget = $(widget);
        var rate_info = rate_widget.next();

        var avg = rate_widget.data('fsr').whole_avg;
        var votes = rate_widget.data('fsr').number_votes;
        var exact = rate_widget.data('fsr').dec_avg;
        var mark = rate_widget.attr('data-mark');

        rate_widget.find('.star_' + avg).prevAll().andSelf().addClass('ratings_vote');
        rate_widget.find('.star_' + avg).nextAll().removeClass('ratings_vote');
        
        rate_info.find('#total_votes').html('Оценок: <strong>' + votes + '</strong> (средняя: <strong>' + exact + '</strong>)');

        if (mark != "0") {
            // Если юзер ставил оценку, добавляем div (#your_vote) с этой оценкой, но только если этого div еще нет.
            if (rate_info.find('#your_vote').length == 0) {
                var html = '<div class="total_votes" id="your_vote"></div>';
                rate_info.append(html);
                var your_vote = rate_info.find('#your_vote');
                your_vote.html('Ваша оценка: <strong>' + mark + '</strong>');
            }
        }
    }

    // Подсветка звезд (выполняется при наведении мыши на любую звезду)
    $('div.ratings_stars').hover(
    // handlerIn (подсвечиваем звезды)
		function () {
		    var star = $(this);
		    var widget = star.parent();
		    if (widget.hasClass('no_vote')) return;
		    var mark = widget.attr('data-mark');
		    if (mark != "0") return;

		    star.prevAll().andSelf().addClass('ratings_over');
		    star.nextAll().removeClass('ratings_vote');
		},
    // handlerOut (удаляем подсветку звезд)
		function () {
		    var star = $(this);
		    star.prevAll().andSelf().removeClass('ratings_over');
		    set_votes(star.parent());    // когда мышь убрана, восстанавливаем оценку
		}
	);

    // Отображение блока с оценкой для каждой книги: звезды + инфа (выполняется при загрузке страницы)
    $('div.rate_widget').each(function (i) {
        var widget = this;
        var info = {
            widget_id: $(widget).attr('data-id'),
            number_votes: $(widget).attr('data-votes'),
            dec_avg: $(widget).attr('data-double-avg'),
            whole_avg: $(widget).attr('data-int-avg')
        };
        $(widget).data('fsr', info);    // .data - это jQuery-метод, "прикручивающий" данные к указанному объекту DOM
        set_votes(widget);              // А здесь мы эти данные обработаем
    });

    // Обработка щелчка мышью на любой звезде 
    // Если это список книг, то ничего не делаем (в списке только отображаются рейтинги, но не задаются)
    //(или если юзер уже поставил оценку по этой книге, то ничего не делаем)
    $('div.ratings_stars').bind('click', function () {
        var star = $(this);
        var widget = star.parent();
        if (widget.hasClass('no_vote')) return;
        var mark = widget.attr('data-mark');
        if (mark != "0") return;

        var clicked_data = {
            clicked_on: star.attr('class'),
            widget_id: widget.attr('data-id')
        };
        $.post(
			'/Book/Vote',
			clicked_data,
			function (info) {
			    widget.attr('data-mark', info.mark);
			    widget.data('fsr', info);
			    set_votes(widget);
			    SetCookie("rate_" + info.widget_id, info.mark, 30);
			},
			'json'
		);
    });
});

/************************************************************************************************************
* Set Cookies
***********************************************************************************************************/
function SetCookie(cookieName, cookieValue, nDays) {
	var today = new Date(),
	expire = new Date();

	if (nDays == null || nDays == 0)
		nDays = 1;

	expire.setTime(today.getTime() + 3600000 * 24 * nDays);
	document.cookie = cookieName + "=" + escape(cookieValue) + ";expires=" + expire.toUTCString(); // escape - это стандартная функция Javascript
}
