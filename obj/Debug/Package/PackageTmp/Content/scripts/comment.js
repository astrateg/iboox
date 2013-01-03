function CreateCommentSuccess(data, textStatus) {
	$(".new-comment").removeClass("new-comment").show("normal");

	var commentText = "";
	commentText += "<div id=\"comment-" + data.commentId + "\" class=\"comment new-comment\">";
	commentText += "<div class=\"comment-header\">Комментарий опубликовал " + data.name + " 0 секунд назад</div>";
	commentText += "<blockquote>" + data.body + "</blockquote>";
	commentText += "</div>";

	var comment = $(commentText);

	// clear the body box
	$("#comment-body").val("");

	// add the new comment to the other comments
	comment.hide().appendTo("#book-comments").slideDown("slow");
}


$(function () {

    $("form.comment-create").submit(function () {
        $.post(
		    "/Book/CreateComment",
		    { bookId: $("#bookId").val(),
		        body: $("#comment-body").val()
		    },
		    CreateCommentSuccess,
		    "json"
	    );
        // don't allow submit because this is an ajax request	
        return false;
    });

    $(".remove-comment").click(function () {
        var id = $(this).attr("id");

        $.post(
			"/Book/RemoveComment", { id: id },
			function (data) {
			    $("#comment-" + data.commentId).fadeOut();
			},
			"json"
		);
        return false;
    });

    $(".edit-comment").click(function () {
        var id = $(this).attr("id").replace("edit-comment-", ""),
            buttons = $(this).parent();
			comment = buttons.prev(),
			bodyText = comment.text(),
			nameText = comment.prev().find(".name").text();

        // hide all the children	
        comment.hide();
        buttons.hide();

        var commentText = "<form><div class=\"field\"><textarea class=\"edit-body\" id=\"body-" + id + "\">" + bodyText +
                        "</textarea><br/><button type=\"button\" class=\"update\" id=\"" + "comment-" + id +
                        "\">OK</button>&nbsp;<button type=\"button\" class=\"cancel\">Отмена</button></div></form>";

        var commentForm = $(commentText);
        comment.parent().append(commentForm);

        // update the form
        commentForm.find(".update").click(function () {
            bodyText = commentForm.find(".edit-body").val();

            $.post(
				"/Book/EditComment",
				{ id: id, body: bodyText },
				function (data) {
				    comment.text(data.body);
				    comment.show();
				    buttons.show();
				    commentForm.remove();
				},
				"json"
			);
        });

        // cancel the update
        commentForm.find(".cancel").click(function () {
            comment.show();
            buttons.show();
            commentForm.remove();
        });

        return false;
    });

});